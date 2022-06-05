using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    public int groundId;
    public Sprite[] groundPhase; // 땅이 깨지는 모습
    public int def;
    public int maxhealth;
    public int curHealth;
    public int recoverValue;
    public float recoverInterval;
    [Tooltip("오염된 땅을 건드렸을 시 받는 뎀지(위에서 아래로 팔떄만 데미지입음)")] public int pollutionDmg; // 오염된 땅을 건드렸을시 받는 데미지
    public bool autoColorSet; // 자동으로 지하로 내려갈수록 색갈 탁해지는거
    public bool autoHpSet; // 자동으로 지하로 내려갈수록 체력 많아지는거
    public bool ice; // 얼음이면 체크
    [Tooltip("퀘스트와 관련있는 오브젝트면 해당 퀘스트인덱스를 입력")]public int questObject;
    public Mineral mineral { get; set; } // 땅이 가지고 있는 광물
    public Chest chest { get; set; } // 땅이 가지고 있는 광물
    public GameObject rock { get; set; } // 땅이 가지고있는 바위
    public bool haveMineral{ get;set; }
    public bool haveChest { get; set; }
    public bool haveRock { get; set; }
    public bool haveHardRock { get; set; }
    public bool haveSpecialRock { get; set; }
    public bool haveSomething { get; set; }
    public bool isAttacked { get; set; }

    [SerializeField] GameObject minimapGround1; // 땅 부셔지면 미니맵에있는 땅도 부셔지게 하기위함
    //public GameObject minimapGroundPf2;
    //GameObject minimapGround2; // 땅 부셔지면 미니맵에있는 땅도 부셔지게 하기위함

    //DatabaseManager databaseManager;
    SpriteRenderer spriteRenderer;
    //PlayerStat playerStat;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if(autoColorSet)
            SetColor();
        if (autoHpSet)
            SetHp();
        if (questObject > 0)
            QuestManager.instance.AddQuestObjInfo(questObject, gameObject);
    }

    private void OnEnable()
    {
        curHealth = maxhealth;
    }

    void SetColor()
    {
        float value = 1 - (transform.position.y * -0.0007f); 
        spriteRenderer.color = new Color(value,value,value);
    }

    void SetHp()
    {
        maxhealth += (int)(transform.position.y * -0.00125f * maxhealth);
        curHealth = maxhealth;
    }

    void Recover()
    {
        curHealth += recoverValue;
        if (curHealth > maxhealth)
            curHealth = maxhealth;
        SpriteControl();
        if(curHealth < maxhealth)
            Invoke("Recover", recoverInterval);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            switch (groundId)
            {
                case 13:
                    Player.instance.friction = 0.02f;
                    break;
                case 14:
                    Player.instance.friction = 0.001f;
                    StopAllCoroutines();
                    StartCoroutine(IceDamage());
                    break;
                default:
                    Player.instance.friction = 0.1f;
                    break;
            }
        }
    }

    IEnumerator IceDamage()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);
            Player.instance.Damaged(10);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && groundId.Equals(14))
        {
            StopAllCoroutines();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Mineral") && !haveSomething)
        {
            mineral = collision.GetComponent<Mineral>();
            if (mineral.randomPosRadius.Equals(0))
                collision.GetComponent<CapsuleCollider2D>().enabled = false;
            else
            {
                mineral.RandomArrange();
                return;
            }
            haveMineral = true;
        }
        else if (collision.gameObject.CompareTag("Chest") && !haveSomething)
        {
            chest = collision.GetComponent<Chest>();
            if(chest.randomPosRadius.Equals(0))
                collision.GetComponent<CapsuleCollider2D>().enabled = false;
            else
            {
                chest.RandomArrange();
                return;
            }
            haveChest = true;
        }
        else if (collision.gameObject.CompareTag("Rock") && !haveSomething)
        {
            haveRock = true;
            rock = collision.gameObject;
            collision.GetComponent<CircleCollider2D>().enabled = false;
            maxhealth = int.MaxValue;
            curHealth = maxhealth;
        }
        else if (collision.gameObject.CompareTag("Hard Rock") && !haveSomething)
        {
            haveHardRock = true;
            rock = collision.gameObject;
            collision.GetComponent<CircleCollider2D>().enabled = false;
            maxhealth = int.MaxValue;
            curHealth = maxhealth;
        }
        else if (collision.gameObject.CompareTag("Rock3") && !haveSomething)
        {
            haveSpecialRock = true;
            rock = collision.gameObject;
            collision.GetComponent<CircleCollider2D>().enabled = false;
            maxhealth = int.MaxValue;
            curHealth = maxhealth;
        }
        if (haveChest || haveHardRock || haveMineral || haveRock || haveSpecialRock)
            haveSomething = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Mineral") && haveMineral)
        {
            haveMineral = false;
            mineral = null;
        }
        else if (collision.gameObject.CompareTag("Chest") && haveChest)
        {
            haveChest = false;
            chest = null;
        }
        if (!haveChest && !haveMineral && !haveHardRock && !haveRock && !haveSpecialRock)
            haveSomething = false;
    }

    public void Attacked(int dmg)
    {
        if (haveRock || haveHardRock)
            curHealth = maxhealth;
        if (mineral)
        {
            if (!isAttacked)
            {
                DatabaseManager.instance.getMineralCount[mineral.itemID]++;
                isAttacked = true;
            }
            mineral.ChangePhase();
        }
        else if(chest)
            chest.ChangePhase();

        dmg -= def;
        if (dmg > maxhealth && curHealth == maxhealth) // 원킬이 안나게
            dmg = maxhealth - 1;
        if (ice) // 얼음꺨떄 불의 원석이 없으면 
        {
            if (!Inventory.instance.SearchItem(30108) || QuestManager.instance.questIndex < 1100)
            {
                dmg = 0;
                InformationPanel.instance.EnableDownInfoPanel("열기를 내뿜는 무언가를 가지고있어야 얼음이 부셔질 것 같다.");
            }
        }

        curHealth -= dmg;
        SpriteControl();

        if (pollutionDmg > 0)
            ObjectManager.instance.MakeObj("gas", transform.position + Vector3.up);

        if (curHealth <= 0)
        {
            if (mineral)
                mineral.GroundOff();

            PlayerStat.instance.GetExp((int)Mathf.Log(transform.position.y * -1, 2f) * 10);
            Debug.Log(Mathf.Abs((int)Mathf.Log(transform.position.y, 2f) * 10));
            gameObject.SetActive(false);
            return;
        }
        if(recoverValue > 0)
        Recover();
    }

    void SpriteControl()
    {
        float healthRate = (float)curHealth / maxhealth;
        if (curHealth >= maxhealth)
            spriteRenderer.sprite = groundPhase[0];
        else if ((healthRate < 0.99f && healthRate > 0.75f))
            spriteRenderer.sprite = groundPhase[1];
        else if (healthRate <= 0.75f && healthRate > 0.5f)
            spriteRenderer.sprite = groundPhase[2];
        else if (healthRate <= 0.5f && healthRate >= 0.25f)
            spriteRenderer.sprite = groundPhase[3];
        else if (healthRate <= 0.25f && healthRate > 0f)
            spriteRenderer.sprite = groundPhase[4];
    }

    public void MinimapGroundOff()
    {
        minimapGround1.SetActive(false);
    }

    private void OnDisable()
    {
        MinimapGroundOff();
    }
}
