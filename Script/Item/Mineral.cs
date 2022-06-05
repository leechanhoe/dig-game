using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mineral : MonoBehaviour
{
    public int itemID;
    public Sprite[] mineralPhase; // 드릴로 건드리면 약간 밝아지기위함
    public float randomPosRadius;
    public bool isdigged { get; set; }
    public int mineralIndex { get; set; } // 데이터베이스에 minerals로 저장된 인덱스
    [SerializeField]
    float disappearTime = 10;

    PlayerStat playerStat;
    DatabaseManager database;

    private void Start()
    {
        playerStat = PlayerStat.instance;
        database = DatabaseManager.instance;
        if (database.mineralsPosition[database.stage].ContainsKey(mineralIndex))
        {
            randomPosRadius = 0;
        }
    }

    public void RandomArrange() // 위치 랜덤
    {
        if (database.mineralsPosition[database.stage].ContainsKey(mineralIndex) || randomPosRadius.Equals(0))
        {
            randomPosRadius = 0;
            return;
        }
        else
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

                database.mineralsPosition[database.stage][mineralIndex] = transform.position;
                break;
            }
        }
    }

    IEnumerator DisappearC() // 광물 캐고 안먹으면 일정시간 지나면 증발
    {    // 캔상태일떄만 실행
        yield return new WaitForSeconds(disappearTime);
        if(gameObject.activeSelf)
            gameObject.SetActive(false);
    }

    public void ChangePhase()
    {
        GetComponent<SpriteRenderer>().sprite = mineralPhase[1];
        isdigged = true;
    }

    public void GroundOff() // 땅이 부셔지고 광물이 노출됐을때
    {

        GetComponent<Rigidbody2D>().gravityScale = 1f;
        CapsuleCollider2D colli = GetComponent<CapsuleCollider2D>();
        colli.enabled = true;
        colli.isTrigger = false;
        StartCoroutine(DisappearC());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (Inventory.instance.IsInventoryFull(itemID))
            {
                AudioManager.instance.Play("Beep");
                return;
            }
            Item item = database.ReturnItem(itemID);
            Inventory.instance.GetAnItem(itemID);

            playerStat.UpdateScore(item.itemSalePrice);
            playerStat.GetExp(item.exp);
            gameObject.SetActive(false);
            StopCoroutine(DisappearC());
        }
    }
}
