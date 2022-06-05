using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Chest : MonoBehaviour
{
    public int itemID;
    public int randomPosRadius;
    public Sprite[] chestPhase;// 드릴로 건드리면 약간 밝아지기위함
    public bool isdigged { get; set; }
    public int chestIndex { get; set; } // 데이터베이스에 chests로 저장된 인덱스
    Item item;

    SpriteRenderer spriteRenderer;
    Rigidbody2D rigid;
    CapsuleCollider2D colli;
    DatabaseManager database;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        colli = GetComponent<CapsuleCollider2D>();
        database = DatabaseManager.instance;

        for (int i = 0; i < database.itemList.Count; i++)
        {
            if (database.itemList[i].itemID == itemID)
            {
                item = database.itemList[i];
                break;
            }
        }
    }

    private void Start()
    {
        if (database.chestsPosition[database.stage].ContainsKey(chestIndex))
            randomPosRadius = 0;
    }

    public void RandomArrange()
    {
        if (database.chestsPosition[database.stage].ContainsKey(chestIndex) || randomPosRadius.Equals(0))
        {
            randomPosRadius = 0;
            return;
        }
        StartCoroutine(RandomArrangeC());
    }

    IEnumerator RandomArrangeC()
    {
        yield return new WaitForSeconds(1f);
        List<Collider2D> colliders = new List<Collider2D>(Physics2D.OverlapCircleAll(transform.position, randomPosRadius, LayerMask.GetMask("Ground")));
        int num = 0;
        for (int i = 0; i < colliders.Count; i++)
        {
            if (colliders[i].GetComponent<Ground>().haveSomething)
                num++;
        }
        if (num.Equals(colliders.Count))
        {
            Debug.LogError("랜덤으로 배치하는데 주변에 공간이없음");
            yield break;
        }
        while (true)
        {
            int i = Random.Range(0, colliders.Count);
            Ground Target = colliders[i].GetComponent<Ground>();
            if (!Target.haveSomething)
            {
                randomPosRadius = 0;
                transform.position = colliders[i].transform.position;
                database.chestsPosition[database.stage][chestIndex] = transform.position;
                break;
            }
        }
    }

    public void ChangePhase()
    {
        spriteRenderer.sprite = chestPhase[1];
        isdigged = true;
    }

    public void GroundOff() // 땅이 부셔지고 광물이 노출됐을때
    {
        rigid.gravityScale = 1f;
        colli.enabled = true;
        colli.isTrigger = false;
        StartCoroutine(DisappearC());
    }

    IEnumerator DisappearC() // 광물 캐고 안먹으면 일정시간 지나면 증발
    {    // 캔상태일떄만 실행
        yield return new WaitForSeconds(10f);
        if(gameObject.activeSelf)
            gameObject.SetActive(false);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (Inventory.instance.IsInventoryFull(itemID))
            {
                AudioManager.instance.Play("Beep");
                return;
            }
            Inventory.instance.GetAnItem(itemID, floatText: false);
            FindObjectOfType<PlayerStat>().UpdateScore(item.itemSalePrice);
            gameObject.SetActive(false);
            StopCoroutine(DisappearC());
        }
    }
}
