using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using System;

[System.Serializable]
public class IntFloat : SerializableDictionary<int, float>{ }

public class Monster : MonoBehaviour
{
    public int enemyID;
    public float speed;
    public float atkSpeed; // 공격속도
    public float projectileSpeed; // 발사체속도
    public int hp;
    public int currentHp;
    public int atk;
    public int exp;
    public bool isThink; // 벽에 안부딫혀도 방향을 돌릴수 있나
    public bool isFire; // 돌을 던지는 몹인가
    public bool isBoss;
    public string deathSound; // 죽을떄 효과음
    [Tooltip("아이템의 아이디와 드롭확률(0 ~ 1)")]public IntFloat dropItem; // 아이템 아이디, 드롭확률
    [Tooltip("몬스터와 관련된 오브젝트(몬스터가 죽을때 생기는 포탈등)")] public GameObject relatedObj;

    ObjectManager objectManager;
    Rigidbody2D rigid;
    CapsuleCollider2D colli;
    Animator animator;
    Player player;
    int nextMove;

    public bool isDead { get; set; }
    public int enemyIndex { get; set; } // 데이터베이스에 monsters로 저장된 인덱스
    bool isAttacking;
    [SerializeField] float bossAttackReloadTime;

    GameObject summonMonster1, summonMonster2, summonMonster3;

    // Start is called before the first frame update
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        colli = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
        player = Player.instance;
        objectManager = ObjectManager.instance;
    }

    private void OnEnable()
    {
        if (Player.instance == null)
            player = FindObjectOfType<Player>();
        else
            player = Player.instance;
            currentHp = hp;
        if (isThink)
            Think();
        else
        {
            nextMove = UnityEngine.Random.Range(0, 2);
            if (nextMove == 0)
                nextMove = -1;
        }
        if (isFire)
            StartCoroutine(FireC());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isAttacking)
            Move();
    }

    private void Move()
    {
        rigid.velocity = new Vector2(nextMove * speed, rigid.velocity.y);

        Vector2 frontVec = new Vector2(rigid.position.x + colli.size.x / 2 * nextMove, rigid.position.y);
        Vector2 downVec = new Vector2(0, (colli.size.y / -2f) - 0.5f);

        //Debug.DrawRay(frontVec, downVec, new Color(0, 1, 0)); // 앞이 절벽인지 체크
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector2.down, (colli.size.y / 2f) + 0.5f, LayerMask.GetMask("Ground"));

        Vector2 vec = new Vector2(colli.size.x / 2 * nextMove, 0);
        //Debug.DrawRay(transform.position, vec, new Color(0, 1, 0));//앞에 땅으로 막혀있나 체크
        RaycastHit2D rayHitXaxis = Physics2D.Raycast(transform.position, Vector2.right * nextMove, colli.size.x / 2 + 0.05f, LayerMask.GetMask("Ground"));

        if ((rayHit.collider == null || rayHitXaxis.collider != null) && !isAttacking)
            Turn();
    }

    void Think()
    {
        if (isBoss)
        {
            animator.SetInteger("Attack Type", 0);
            isAttacking = false;
            animator.SetBool("Is Attacking", isAttacking);
        }
        if (!isAttacking)
            nextMove = UnityEngine.Random.Range(0, 2);
        if (nextMove == 0)
            nextMove = -1;
        if (isBoss && player != null)
        {
            if (transform.position.x >= player.transform.position.x)
                nextMove = -1;
            else
                nextMove = 1;
        }
        animator.SetInteger("Next Move", nextMove);
        if (isThink)
            Invoke("Think", UnityEngine.Random.Range(1f, 5f));
    }

    void Turn()
    {
        nextMove *= -1;
        animator.SetInteger("Next Move", nextMove);
        CancelInvoke();
        if (isThink)
            Invoke("Think", UnityEngine.Random.Range(1f, 5f));
    }

    IEnumerator FireC()
    {
        yield return new WaitForSeconds(atkSpeed);
        Fire();
        if (isBoss)
            ChooseBossAttack();
    }

    IEnumerator FireOnC()
    {
        if (!isFire)
            yield break;
        switch (enemyID)
        {
            case 4:
                GameObject stone4 = objectManager.MakeObj("monster4_attack", transform.position);
                stone4.GetComponent<Rigidbody2D>().AddForce(new Vector2(nextMove * projectileSpeed, 0.1f), ForceMode2D.Impulse);
                break;
            case 5:
                GameObject stone5_1 = objectManager.MakeObj("monster5_attack", transform.position);
                GameObject stone5_2 = objectManager.MakeObj("monster5_attack", transform.position);
                stone5_1.GetComponent<Rigidbody2D>().AddForce(new Vector2(nextMove * projectileSpeed, 0.1f), ForceMode2D.Impulse);
                stone5_2.GetComponent<Rigidbody2D>().AddForce(new Vector2(nextMove * projectileSpeed * -1, 0.1f), ForceMode2D.Impulse);
                break;
            case 6:
                GameObject stone6_1 = objectManager.MakeObj("monster6_attack", transform.position);
                GameObject stone6_2 = objectManager.MakeObj("monster6_attack", transform.position);
                GameObject stone6_3 = objectManager.MakeObj("monster6_attack", transform.position);
                stone6_1.GetComponent<Rigidbody2D>().AddForce(new Vector2(nextMove * projectileSpeed, 0.1f), ForceMode2D.Impulse);
                stone6_2.GetComponent<Rigidbody2D>().AddForce(new Vector2(nextMove * projectileSpeed * -1f, 0.1f), ForceMode2D.Impulse);
                stone6_3.GetComponent<Rigidbody2D>().AddForce(new Vector2(nextMove * projectileSpeed * UnityEngine.Random.Range(0, 2), 2f), ForceMode2D.Impulse);
                break;
            case 7:
                GameObject stone7_1 = objectManager.MakeObj("monster7_attack1", new Vector2(transform.position.x, transform.position.y + 1));
                stone7_1.GetComponent<Rigidbody2D>().AddForce((player.transform.position - transform.position - new Vector3(0, 1f)).normalized * projectileSpeed, ForceMode2D.Impulse);
                if ((float)currentHp / hp <= 0.3f)
                {
                    GameObject stone7_2 = objectManager.MakeObj("monster7_attack1", new Vector2(transform.position.x, transform.position.y - 0.5f));
                    stone7_2.GetComponent<Rigidbody2D>().AddForce((player.transform.position - transform.position + new Vector3(0, 0.5f)).normalized * projectileSpeed, ForceMode2D.Impulse);
                }
                break;
            case 8:
                GameObject stone8_1 = objectManager.MakeObj("monster8_attack1", new Vector2(transform.position.x, transform.position.y + colli.size.y / 2));
                stone8_1.GetComponent<Rigidbody2D>().AddForce((player.transform.position - transform.position + new Vector3(0, colli.size.y / -2)).normalized * projectileSpeed, ForceMode2D.Impulse);
                if ((float)currentHp / hp <= 0.5f)
                {
                    GameObject stone8_2 = objectManager.MakeObj("monster8_attack1", new Vector2(transform.position.x, transform.position.y - colli.size.y / 3));
                    stone8_2.GetComponent<Rigidbody2D>().AddForce((player.transform.position - transform.position + new Vector3(0, colli.size.y / 3)).normalized * projectileSpeed, ForceMode2D.Impulse);
                }
                if ((float)currentHp / hp <= 0.25f)
                {
                    GameObject stone8_3 = objectManager.MakeObj("monster8_attack1", new Vector2(transform.position.x, transform.position.y + colli.size.y / 6));
                    stone8_3.GetComponent<Rigidbody2D>().AddForce((player.transform.position - transform.position + new Vector3(0, colli.size.y / -6)).normalized * projectileSpeed, ForceMode2D.Impulse);
                }
                break;
            case 9:
                GameObject stone9_1 = objectManager.MakeObj("monster9_attack", new Vector2(transform.position.x, transform.position.y + 1));
                stone9_1.GetComponent<Rigidbody2D>().AddForce((player.transform.position - transform.position - new Vector3(0, 1f)).normalized * projectileSpeed, ForceMode2D.Impulse);
                if ((float)currentHp / hp <= 0.5f)
                {
                    GameObject stone9_2 = objectManager.MakeObj("monster9_attack", new Vector2(transform.position.x, transform.position.y));
                    stone9_2.GetComponent<Rigidbody2D>().AddForce((player.transform.position - transform.position).normalized * projectileSpeed, ForceMode2D.Impulse);
                }
                if (UnityEngine.Random.Range(0, 5).Equals(0))
                {
                    if (summonMonster1 == null)
                    {
                        summonMonster1 = objectManager.MakeObj("monster" + UnityEngine.Random.Range(0, 3), transform.position);
                        summonMonster2 = objectManager.MakeObj("monster" + UnityEngine.Random.Range(0, 3), transform.position);
                        summonMonster3 = objectManager.MakeObj("monster" + UnityEngine.Random.Range(0, 3), transform.position);
                    }
                    else
                    {
                        if (!summonMonster1.activeSelf)
                            summonMonster1 = objectManager.MakeObj("monster" + UnityEngine.Random.Range(0, 3), transform.position);
                        if (!summonMonster2.activeSelf)
                            summonMonster2 = objectManager.MakeObj("monster" + UnityEngine.Random.Range(0, 3), transform.position);
                        if (!summonMonster3.activeSelf)
                            summonMonster3 = objectManager.MakeObj("monster" + UnityEngine.Random.Range(0, 3), transform.position);
                    }
                }
                break;
            default:
                break;
        }
        yield return new WaitForSeconds(atkSpeed);
        Fire();
    }

    void Fire()
    {
        StartCoroutine(FireOnC());
    }

    public void Damaged(int dmg)
    {
        var clone = Instantiate(Resources.Load("Prefabs/FloatingText"), transform.position, Quaternion.Euler(Vector3.zero)) as GameObject; // 정확한 형식을 모를 떄 var , instantiate 는 프리펩 생성해서 클론에 넘
                                                                                                                                           // instantiate(대상,위치,각도)
        clone.GetComponent<FloatingText>().text.text = dmg.ToString();
        clone.GetComponent<FloatingText>().text.color = Color.yellow;
        clone.transform.SetParent(FindObjectOfType<Player>().GetComponentInChildren<RectTransform>());
        clone.transform.localScale = new Vector3(1f, 1f, 1f);
        currentHp -= dmg;
        if (currentHp <= 0)
            StartCoroutine(deathC());
    }

    bool bossDeath;
    void BossDeathAnimationEnd()
    {
        bossDeath = true;
    }

    void ObjControlWhenDeath()
    {
        if (enemyID.Equals(9) && DatabaseManager.instance.stage.Equals(1))
            relatedObj.SetActive(true);
    }

    IEnumerator deathC()
    {
        isFire = false;
        isDead = true;
        nextMove = 0;
        animator.SetInteger("Next Move", nextMove);
        rigid.constraints = RigidbodyConstraints2D.FreezeAll;
        colli.enabled = false;
        AudioManager.instance.Play(deathSound);
        if (isBoss)
        {
            gameObject.tag = "Structure";
            animator.SetTrigger("Death");
            yield return new WaitUntil(() => bossDeath);
        }
        if(relatedObj != null)
            ObjControlWhenDeath();

        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        Color color = sprite.color;
        while (true)
        {
            color.a -= 0.02f;
            sprite.color = color;
            if (color.a <= 0)
                break;
            yield return new WaitForSeconds(0.03f);
        }
        DropItem();
        player.GetComponent<PlayerStat>().GetExp(exp);
        gameObject.SetActive(false);
    }

    void DropItem()
    {
        if (dropItem.Count < 1)
            return;

        float[] proOfEach = new float[dropItem.Count + 1];
        int[] itemID = new int[dropItem.Count + 1];
        float proOfSum = 0;
        int i = 1;
        int decidedItem = 0;
        proOfEach[0] = 0;
        foreach (KeyValuePair<int, float> item in dropItem)
        {
            proOfSum += item.Value;
            proOfEach[i] = proOfSum;
            itemID[i++] = item.Key;
        }

        if (proOfSum > 1)
            Debug.LogError("드랍템들의 확률을 모두 더한값이 1이넘음");

        float randomNum = UnityEngine.Random.Range(0f, 1f);
        Debug.Log(randomNum);
        for (int j = 0; j < proOfEach.Length - 1; j++)
        {
            if (proOfEach[j] <= randomNum && randomNum < proOfEach[j + 1])
            {
                decidedItem = itemID[j + 1];
                break;
            }
        }

        if (decidedItem == 0)
        {
            Debug.Log("드롭되지 않음");
            return;
        }

        GameObject drop = Instantiate(Resources.Load("Prefabs/DropableItem"), transform.position, Quaternion.Euler(Vector3.zero)) as GameObject;
        drop.GetComponent<DropableItem>().Set(decidedItem);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" && !isDead)
            Player.instance.Damaged((int)(atk * UnityEngine.Random.Range(0.8f, 1.2f)));
    }

    void ChooseBossAttack()
    {
        StartCoroutine(ChooseBossAttackC());
    }

    IEnumerator ChooseBossAttackC()
    {
        int attackType = 0;
        isAttacking = true;
        if (enemyID == 7)
        {
            if (summonMonster1 || summonMonster2)
                attackType = UnityEngine.Random.Range(1, 4);
            else
                attackType = UnityEngine.Random.Range(1, 5);
        }
        if (enemyID == 8)
        {
            if (summonMonster1 || summonMonster2)
                attackType = UnityEngine.Random.Range(1, 5);
            else
                attackType = UnityEngine.Random.Range(1, 6);
        }
        animator.SetInteger("Attack Type", attackType);
        animator.SetBool("Is Attacking", true);
        CancelInvoke();
        yield return new WaitForSeconds(bossAttackReloadTime);
        ChooseBossAttack();
    }
    public void BossAttack(int type)
    {
        if (enemyID == 7)
        {
            switch (type)
            {
                case 1:
                    GameObject stone7_2 = objectManager.MakeObj("monster7_attack2", transform.position);
                    stone7_2.GetComponent<Rigidbody2D>().AddForce(new Vector2(nextMove * 5, 0.1f), ForceMode2D.Impulse);
                    break;
                case 2:
                    GameObject stone7_2_1 = objectManager.MakeObj("monster7_attack2", transform.position);
                    GameObject stone7_2_2 = objectManager.MakeObj("monster7_attack2", transform.position);
                    GameObject stone7_2_3 = objectManager.MakeObj("monster7_attack2", transform.position);
                    GameObject stone7_2_4 = objectManager.MakeObj("monster7_attack2", transform.position);
                    GameObject stone7_2_5 = objectManager.MakeObj("monster7_attack2", transform.position);
                    stone7_2_1.GetComponent<Rigidbody2D>().AddForce(new Vector2(1, 0.1f).normalized * 3, ForceMode2D.Impulse);
                    stone7_2_2.GetComponent<Rigidbody2D>().AddForce(new Vector2(1, 1).normalized * 3, ForceMode2D.Impulse);
                    stone7_2_3.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 1).normalized * 3, ForceMode2D.Impulse);
                    stone7_2_4.GetComponent<Rigidbody2D>().AddForce(new Vector2(-1, 1).normalized * 3, ForceMode2D.Impulse);
                    stone7_2_5.GetComponent<Rigidbody2D>().AddForce(new Vector2(-1, 0.1f).normalized * 3, ForceMode2D.Impulse);

                    break;
                case 3:
                    GameObject stone7_3 = objectManager.MakeObj("monster7_attack3", transform.position);
                    stone7_3.GetComponent<Rigidbody2D>().AddForce(new Vector2(nextMove * 3, 0.1f), ForceMode2D.Impulse);
                    break;
                case 4:
                    summonMonster1 = Instantiate(Resources.Load("Prefabs/Monster/Monster4"), transform.position, Quaternion.Euler(Vector3.zero)) as GameObject;
                    if (currentHp / hp <= 0.5f)
                        summonMonster2 = Instantiate(Resources.Load("Prefabs/Monster/Monster4"), transform.position, Quaternion.Euler(Vector3.zero)) as GameObject;
                    break;
            }
        }
        else if (enemyID == 8)
        {
            switch (type)
            {
                case 1:
                    GameObject stone8_21 = objectManager.MakeObj("monster8_attack2", transform.position + new Vector3(0, 1));
                    stone8_21.GetComponent<Rigidbody2D>().AddForce(new Vector2(nextMove * 5, 0.1f), ForceMode2D.Impulse);
                    GameObject stone8_22 = objectManager.MakeObj("monster8_attack2", transform.position);
                    stone8_22.GetComponent<Rigidbody2D>().AddForce(new Vector2(nextMove * 5, -0.2f), ForceMode2D.Impulse);
                    break;
                case 2:
                    GameObject stone8_2_1 = objectManager.MakeObj("monster8_attack2", transform.position);
                    GameObject stone8_2_2 = objectManager.MakeObj("monster8_attack2", transform.position);
                    GameObject stone8_2_3 = objectManager.MakeObj("monster8_attack2", transform.position);
                    GameObject stone8_2_4 = objectManager.MakeObj("monster8_attack2", transform.position);
                    GameObject stone8_2_5 = objectManager.MakeObj("monster8_attack2", transform.position);
                    stone8_2_1.GetComponent<Rigidbody2D>().AddForce(new Vector2(1, 0.1f).normalized * 3, ForceMode2D.Impulse);
                    stone8_2_2.GetComponent<Rigidbody2D>().AddForce(new Vector2(1, 1).normalized * 3, ForceMode2D.Impulse);
                    stone8_2_3.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 1).normalized * 3, ForceMode2D.Impulse);
                    stone8_2_4.GetComponent<Rigidbody2D>().AddForce(new Vector2(-1, 1).normalized * 3, ForceMode2D.Impulse);
                    stone8_2_5.GetComponent<Rigidbody2D>().AddForce(new Vector2(-1, 0.1f).normalized * 3, ForceMode2D.Impulse);

                    break;
                case 3:
                    GameObject stone8_3_1 = objectManager.MakeObj("monster8_attack3", transform.position + new Vector3(0, 1));
                    stone8_3_1.GetComponent<Rigidbody2D>().AddForce(new Vector2(nextMove * 4, 0.1f), ForceMode2D.Impulse);
                    GameObject stone8_3_2 = objectManager.MakeObj("monster8_attack3", transform.position + new Vector3(0, 0.2f));
                    stone8_3_2.GetComponent<Rigidbody2D>().AddForce(new Vector2(nextMove * 4, -0.1f), ForceMode2D.Impulse);
                    break;
                case 4:
                    for (float i = 0; i <= 0.5f; i += 0.1f)
                    {
                        GameObject stone8_4 = objectManager.MakeObj("monster8_attack3", transform.position + new Vector3(0, 0.2f));
                        stone8_4.GetComponent<Rigidbody2D>().AddForce(new Vector2((float)Math.Cos(i) * nextMove, (float)Math.Sin(i)) * 4, ForceMode2D.Impulse);
                    }
                    break;
                case 5:
                    summonMonster1 = Instantiate(Resources.Load("Prefabs/Monster/Monster6"), transform.position, Quaternion.Euler(Vector3.zero)) as GameObject;
                    if (currentHp / hp <= 0.5f)
                        summonMonster2 = Instantiate(Resources.Load("Prefabs/Monster/Monster6"), transform.position, Quaternion.Euler(Vector3.zero)) as GameObject;
                    break;
            }
        }
    }

    private void OnDisable()
    {
        CancelInvoke();
        StopCoroutine(FireC());
        if (DatabaseManager.instance != null && PlayerStat.instance != null)
        {
            if (DatabaseManager.instance.monstersPosition[PlayerStat.instance.stage].Count <= enemyIndex)
                Debug.LogError("에너미 인덱스가 비정상임" + PlayerStat.instance.stage +"스테이지의 "+ enemyIndex);
            DatabaseManager.instance.monstersPosition[PlayerStat.instance.stage][enemyIndex] = transform.position;
        }
    }
}