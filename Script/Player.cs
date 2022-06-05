using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class Player : MonoBehaviour
{
    static public Player instance;
    public float friction; // 마찰력
    public float maxDownVelocity;

    public bool isDigging { get; set; } // 땅파는중인가?
    public enum ActionState // enum = 열거
    {
        Normal,
        Shop,
        Smithy,
        Npc,
        SavePoint,
        Storage,
        Cart,
        Portal
    }
    public ActionState actionState { get; set; } = ActionState.Normal;

    public Sprite diggingSprite;
    Sprite idleSprite;
    public GameObject scanObject { get; set; }
    public GameObject scanObject2 { get; set; } // 벽에 끼는거 방지용 변수
    public GameObject scanDownObject { get; set; } // 발에 닿은거
    Ground scanGround;
    public Shop shop { get; set; }
    public Sprite damagedSprite;

    public bool isInvincible { get; set; } // 무적인가
    public bool isDeath { get; set; }

    #region // 이동관련
    Vector2 moveVec;
    Vector2 dirVec;
    Vector2 scanObjectDir;

    public bool right { get; set; }
    public bool left { get; set; }
    public bool up { get; set; }
    public bool down { get; set; }

    public bool notMove { get; set; }
    public bool isFlying { get; set; }

    public bool isMobileMove;
    #endregion

    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    AudioManager audioManager;
    CapsuleCollider2D colli;
    PlayerStat playerStat;
    JoyPad joyPad;
    PlayCanvas playCanvas;
    TalkManager talkManager;
    Equipment equipment;
    DataSlotUI dataSlotUI;
    Storage storage;
    TransferMap transferMap;
    Portal portal;
    DatabaseManager databaseManager;

    public GameObject bombPrefab;
    public GameObject dynamitePrefab;
    public GameObject playerOnMinimapPF;
    GameObject playerOnMinimap;

    public GameObject prefab_floating_Text;

    [SerializeField] Text text;

    private void Awake()
    {
        if (instance == null) // 파괴방지
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
        else
            Destroy(this.gameObject);
        playerStat = GetComponent<PlayerStat>();
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioManager = FindObjectOfType<AudioManager>();
        colli = GetComponent<CapsuleCollider2D>();
        joyPad = FindObjectOfType<JoyPad>();
        playCanvas = FindObjectOfType<PlayCanvas>();
        talkManager = FindObjectOfType<TalkManager>();
        equipment = FindObjectOfType<Equipment>();
        dataSlotUI = FindObjectOfType<DataSlotUI>();
        storage = FindObjectOfType<Storage>();
        transferMap = FindObjectOfType<TransferMap>();
        databaseManager = FindObjectOfType<DatabaseManager>();

        ShowPlayerOnMinimap();
        CantMove();
    }

    public void ShowPlayerOnMinimap()
    {
        playerOnMinimap = Instantiate(playerOnMinimapPF, transform.position, Quaternion.Euler(Vector3.zero));
    }

    private void Update()
    { 
        //단위벡터
        if (left)
            dirVec = Vector2.left;
        else if (right)
            dirVec = Vector2.right;
        else if (down && !isFlying)
            dirVec = Vector2.down;

        if ((left || right || down) && scanObject)
            Dig();
        if(playerOnMinimap != null)
            MarkOnMinimap();
        if (databaseManager.monsters[playerStat.stage] != null && !playerStat.stage.Equals(0) && !notMove)
            CheckNearbyMonster();
        text.text = Time.deltaTime.ToString();
    }
    
    #region
    private void FixedUpdate()
    {
        if(!notMove)
            Move();

        //Ray 물체탐색
        float distance = 0;
        if (right || left)
            distance = 0.33f;
        else if (down)
            distance = 0.55f;
        //Debug.DrawRay(rigid.position, dirVec , new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, dirVec, distance, LayerMask.GetMask("Ground"));
        RaycastHit2D rayHitU = Physics2D.Raycast(rigid.position + new Vector2(0, colli.size.y / 2 * 0.9f), dirVec, distance, LayerMask.GetMask("Ground"));
        RaycastHit2D rayHitD = Physics2D.Raycast(rigid.position - new Vector2(0, colli.size.y / 2 * 0.9f), dirVec, distance, LayerMask.GetMask("Ground"));

        if (rayHitU.collider != null) // 이상하게 벽에 끼는거 방지
            scanObject2 = rayHitU.collider.gameObject;
        if (rayHitD.collider != null) // 이상하게 벽에 끼는거 방지
            scanObject2 = rayHitD.collider.gameObject;
        if (rayHit.collider != null) // 이상하게 벽에 끼는거 방지
            scanObject2 = rayHit.collider.gameObject;
        if (!rayHitU.collider && !rayHitD.collider && !rayHit.collider)
            scanObject2 = null;

        if (rayHit.collider)
        {
            scanObject = rayHit.collider.gameObject;
            scanObjectDir = dirVec;
        }
        else
        {
            scanObject = null;
            scanObjectDir = Vector2.zero;
        }

        //Debug.DrawRay(rigid.position, Vector2.down * 0.5f, new Color(0, 1, 0)); // 땅에 닿았나 확인하는거
        RaycastHit2D rayHitDown = Physics2D.Raycast(rigid.position, Vector2.down, 0.6f, LayerMask.GetMask("Ground"));
        if(rayHitDown.collider != null)
            scanDownObject = rayHitDown.collider.gameObject;
        RaycastHit2D rayHitDownL = Physics2D.Raycast(rigid.position - new Vector2(colli.size.x / 2, 0), Vector2.down, 0.6f, LayerMask.GetMask("Ground")); ;
        RaycastHit2D rayHitDownR = Physics2D.Raycast(rigid.position + new Vector2(colli.size.x / 2, 0), Vector2.down, 0.6f, LayerMask.GetMask("Ground"));
        if (rayHitDown.collider != null || rayHitDownL.collider != null || rayHitDownR.collider != null)
        {
            isFlying = false;
            CheckFall();
        }

        else
            isFlying = true;
    }

    float xAxisValue { get; set; }
    float MoveX(sbyte direction, float increment = 0.1f) // getAxis같은 함수
    {
        float friction = this.friction;
        if (direction == -1)
            xAxisValue -= increment;
        else if (direction == 0)
        {
            if (!isFlying)
            {
                if (friction * -1 < xAxisValue && xAxisValue < friction)
                {
                    xAxisValue = 0;
                }
                else if (xAxisValue <= -friction)
                    xAxisValue += friction;
                else if (xAxisValue >= friction)
                    xAxisValue -= friction;
            }
            else
            {
                this.friction = 0.05f;
                friction = 0.05f;
                if (friction * -1 < xAxisValue && xAxisValue < friction)
                {
                    xAxisValue = 0;
                    return xAxisValue;
                }
                else if (xAxisValue <= -friction)
                    xAxisValue += friction;
                else if (xAxisValue >= friction)
                    xAxisValue -= friction;
            }
        }
        else if (direction == 1)
            xAxisValue += increment;

        if (xAxisValue < -1)
            xAxisValue = -1;
        if (xAxisValue > 1)
            xAxisValue = 1;

        return xAxisValue;
    }

    void MoveUp()
    {
        rigid.velocity = new Vector2(rigid.velocity.x, rigid.velocity.y + Time.deltaTime * 4);
        if (rigid.velocity.y >= playerStat.momentum)
            rigid.velocity = new Vector2(rigid.velocity.x, playerStat.momentum);
    }

    public void DirectionInitialize()
    {
        left = false;
        right = false;
        down = false;
    }

    float h { get; set; }

    void MobileMove()
    {
        DirectionInitialize();
        float angle = joyPad.angle;

        if (45 < angle && angle <= 135 && joyPad.isTouch)
        {
            left = true;
            h = MoveX(-1);
        }
        else if (135 < angle && angle <= 225 && joyPad.isTouch)
        {
            down = true;
            h = MoveX(0);
        }
        else if (225 < angle && angle <= 315 && joyPad.isTouch)
        {
            right = true;
            h = MoveX(1);
        }
        else
            h = MoveX(0);
    }

    void PcMove()
    {
        right = Input.GetKey(KeyCode.RightArrow);
        up = Input.GetKey(KeyCode.UpArrow);
        left = Input.GetKey(KeyCode.LeftArrow);
        down = Input.GetKey(KeyCode.DownArrow);

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            h = MoveX(-1);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
            h = MoveX(1);
        else
            h = MoveX(0);
    }
    private void Move()
    {
        if (isMobileMove)
            MobileMove();
        else
            PcMove();

     //   if (!right && !up && !down && !left) { h = 0;}

        if (scanObject2) // 이상하게 벽에 끼는거 방지
        {
            if (transform.position.x - scanObject2.transform.position.x > 0 && left)
                h = 0.01f;
            if (transform.position.x - scanObject2.transform.position.x < 0 && right)
                h = -0.01f;
        }
        if (up)
        {
            MoveUp();
            //transform.Translate(0, 0.01f, 0);
        }
        else if (isFlying && !up) //가만히있을때
        {
            if (isDigging)
            {
                rigid.velocity = new Vector2(rigid.velocity.x, -4 - Time.deltaTime * 8);
            }
            else
                rigid.velocity = new Vector2(rigid.velocity.x, rigid.velocity.y - Time.deltaTime * 4);

            if (rigid.velocity.y <= maxDownVelocity)
                rigid.velocity = new Vector2(rigid.velocity.x, maxDownVelocity);
        }
        if (!isFlying && (!up))
            rigid.gravityScale = 0.1f;
        else
            rigid.gravityScale = 0;
        moveVec.Set(h * playerStat.speedX, rigid.velocity.y);
        rigid.velocity = moveVec;
    }

    public void CantMove()
    {
        notMove = true;
        rigid.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    public void CanMove()
    {
        notMove = false;
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    bool upA = true; // 알파값을 올리는가?
    void MarkOnMinimap() // 플레이어를 미니맵에 표시
    {
        playerOnMinimap.transform.position = transform.position;
        Color color = playerOnMinimap.GetComponent<SpriteRenderer>().color;
        if (color.a < 0.2f && !upA)
            upA = true; 
        else if(color.a > 0.99f && upA)
            upA = false;

        if(upA)
            color.a += Time.deltaTime;
        else if (!upA)
            color.a -= Time.deltaTime;

        playerOnMinimap.GetComponent<SpriteRenderer>().color = color;

    }
    #endregion// 이동관련

    ObData obData;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Shop")
        {
            actionState = ActionState.Shop;
            shop = collision.gameObject.GetComponent<Shop>();
            playCanvas.OnShop();
        }
        else if (collision.gameObject.tag == "Smithy")
        {
            actionState = ActionState.Smithy;
            playCanvas.EntryAction();
        }
        else if(collision.gameObject.tag == "Npc")
        {
            actionState = ActionState.Npc;
            obData = collision.gameObject.GetComponent<ObData>();
            playCanvas.EntryAction();
        }
        else if (collision.gameObject.tag == "Save Point")
        {
            actionState = ActionState.SavePoint;
            obData = collision.gameObject.GetComponent<ObData>();
            playCanvas.EntryAction();
        }
        else if (collision.gameObject.tag == "Storage")
        {
            actionState = ActionState.Storage;
            playCanvas.EntryAction();
        }
        else if(collision.gameObject.CompareTag("Cart"))
        {
            actionState = ActionState.Cart;
            playCanvas.EntryAction();
        }
        else if (collision.gameObject.CompareTag("Portal"))
        {
            actionState = ActionState.Portal;
            portal = collision.gameObject.GetComponent<Portal>();
            playCanvas.EntryAction();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Shop")
        {
            actionState = ActionState.Normal;
            shop = null;
            playCanvas.ExitAction();
        }
        if (collision.gameObject.tag == "Smithy")
        {
            actionState = ActionState.Normal;
            playCanvas.ExitAction();
        }
        if (collision.gameObject.tag == "Npc")
        {
            obData = null;
            actionState = ActionState.Normal;
            playCanvas.ExitAction();
        }
        if (collision.gameObject.tag == "Save Point")
        {
            actionState = ActionState.Normal;
            playCanvas.ExitAction();
        }
        if (collision.gameObject.tag == "Storage")
        {
            actionState = ActionState.Normal;
            playCanvas.ExitAction();
        }
        if(collision.gameObject.CompareTag("Cart"))
        {
            actionState = ActionState.Normal;
            playCanvas.ExitAction();
        }
        if (collision.gameObject.CompareTag("Portal"))
        {
            actionState = ActionState.Normal;
            playCanvas.ExitAction();
        }
    }

    void Dig() // 땅파기
    {
        if (!scanObject || scanObjectDir != dirVec) // 스캔한 땅이 없거나 스캔한 땅이 지금 내가 바라보는 방향하고 다를떄
            return;
        if (up || isFlying || transform.position.y + 0.1f < scanObject.transform.position.y 
            || Mathf.Abs(transform.position.x - scanObject.transform.position.x) > 0.8f) // 플레이어로부터의 거리
            return;
        if (isInvincible)
            return;
        if (isDigging)
            return;
        if (!equipment.IsEquipedDrill())
        {
            InformationPanel.instance.EnableDownInfoPanel("드릴을 장착해주세요.");
            return;
        }
        scanGround = scanObject.GetComponent<Ground>();

        if (!scanGround.isAttacked)
            StartCoroutine(DigTerm());
        else
            StartCoroutine(DigC());
    }
    IEnumerator DigTerm()// 바로 파지않고 텀 한번줌
    {
        isDigging = true;
        yield return new WaitForSeconds(0.05f);
        scanGround.isAttacked = true;
        isDigging = false;
    }
    IEnumerator DigC()
    {
        if (scanGround.curHealth > scanGround.maxhealth)
            scanGround.curHealth = scanGround.maxhealth;
        xAxisValue = 0f;
        isDigging = true;
        rigid.velocity = new Vector2(0, -3f);

        var damage = playerStat.Damage();

        var clone = Instantiate(prefab_floating_Text, scanGround.transform.position, Quaternion.Euler(Vector3.zero)); // 정확한 형식을 모를 떄 var , instantiate 는 프리펩 생성해서 클론에 넘                                                                                              // instantiate(대상,위치,각도)
        clone.GetComponent<FloatingText>().text.text = damage.dmg.ToString();
        clone.transform.SetParent(GetComponentInChildren<RectTransform>());
        clone.transform.localScale = new Vector3(1f, 1f, 1f);

        if (damage.cri)
        {
            clone.GetComponent<FloatingText>().text.color = Color.red;
            clone.GetComponent<FloatingText>().text.fontSize = 35;
        }
        else
        {
            clone.GetComponent<FloatingText>().text.color = Color.white;
            clone.GetComponent<FloatingText>().text.fontSize = 30;
        }

        scanGround.Attacked(damage.dmg);
        if (scanGround.pollutionDmg > 0 && down)
            Damaged(scanGround.pollutionDmg);
        idleSprite = spriteRenderer.sprite;
        spriteRenderer.sprite = diggingSprite; // 드릴스프라이트

        audioManager.Play("Drill Sound");
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.02f, transform.position.z);
        yield return new WaitForSeconds(1 / playerStat.atkSpeed * 0.5f);
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.02f, transform.position.z);
        yield return new WaitForSeconds(1 / playerStat.atkSpeed * 0.5f);
        audioManager.Stop("Drill Sound");

        isDigging = false;
        spriteRenderer.sprite = idleSprite;
    }

    public void ActButtonDown() // 특정 상황에서 액션버튼을 눌렀을 때
    {
        if (actionState == ActionState.Shop)
        {
            shop.EnterShop();
            return;
        }
        else if (actionState == ActionState.Smithy)
        {
            FindObjectOfType<Smithy>().EnterSmithy();
            return;
        }
        else if (actionState == ActionState.Npc)
            talkManager.Talk(obData);
        else if (actionState == ActionState.SavePoint)
            dataSlotUI.OpenDataSlotUI(0);
        else if (actionState == ActionState.Storage)
            storage.EnterStorage();
        else if (actionState == ActionState.Cart)
            transferMap.OpenTfMapUI();
        else if (actionState.Equals(ActionState.Portal))
            InformationPanel.instance.EnableOOC(Vector2.zero, "들어가시겠습니까?", "확인", "취소", GoPortal, CancelGoPortal);
        else
            up = true;
    }

    public void ActButtonUp()
    {
        up = false;
        if (rigid.velocity.y > 0)
            rigid.velocity = new Vector2(rigid.velocity.x, rigid.velocity.y / 2);
    }

    void CheckFall()
    {
        if (!isFlying && rigid.velocity.y <= -8)
            Damaged((int)(rigid.velocity.y + 6) * -5);
    }
    public void Damaged(int value)
    {
        if (isInvincible || isDeath)
            return;
        StartCoroutine(DamagedC(value));
    }

    public Sprite idle;
    IEnumerator DamagedC(int value)
    { 
        spriteRenderer.sprite = damagedSprite;
        isInvincible = true;
        gameObject.layer = 11;
        value -= playerStat.def;
        if (value < 1)
            value = 1;
        playerStat.curHealth -= value;
        audioManager.Play("Damaged");
        var clone = Instantiate(prefab_floating_Text, transform.position, Quaternion.Euler(Vector3.zero)); // 정확한 형식을 모를 떄 var , instantiate 는 프리펩 생성해서 클론에 넘
                                                                                                                      // instantiate(대상,위치,각도)
        clone.GetComponent<FloatingText>().text.text = value.ToString();
        clone.GetComponent<FloatingText>().text.color = Color.magenta;
        clone.transform.SetParent(GetComponentInChildren<RectTransform>());
        clone.transform.localScale = new Vector3(1f, 1f, 1f);

        for (int i = 0;i < 4;i++)
        {
            Color color = spriteRenderer.color;
            color.a = 0f;
            spriteRenderer.color = color;
            yield return new WaitForSeconds(0.3f);
            color.a = 1f;
            spriteRenderer.color = color;
            yield return new WaitForSeconds(0.3f);
        }
        spriteRenderer.sprite = idle;
        isInvincible = false;
        gameObject.layer = 12;
    }

    void CancelGoPortal()
    {
        playCanvas.ExitAction();
        portal = null;
    }

    void GoPortal()
    {
        StartCoroutine(GoPortalC(portal));
    }

    IEnumerator GoPortalC(Portal portal)
    {
        FadeManager.instance.FadeOut(0.01f);
        yield return new WaitUntil(() => FadeManager.instance.completedBlack);
        if (!portal.stageMove)
        {
            transform.position = portal.destination.position;
            portal.targetBound.SetBound();
            portal.ObjectControl();
            FadeManager.instance.FadeIn(0.01f);
            if (portal.minimap)
                GameManager.instance.MinimapUiOn();
            else
                GameManager.instance.MinimapUiOff();
        }
        else
        {
            LoadingSceneManager.LoadScene("Stage" + portal.stage);
        }
        playCanvas.ExitAction();
        portal = null;
    }

    public void CheckNearbyMonster()
    {
        if(!databaseManager.monstersPosition[playerStat.stage].Count.Equals(databaseManager.monsters[playerStat.stage].Length))
        {
            Debug.LogError("몬스터 포지션" + databaseManager.monstersPosition[playerStat.stage].Count + "이랑 몬스터들값" + databaseManager.monsters[playerStat.stage].Length + " 개수가 다름");
        }
        for (int i = 0; i < databaseManager.monstersPosition[playerStat.stage].Count; i++)
        {
            if (databaseManager.monstersPosition[playerStat.stage].ContainsKey(i))
            {
                if ((databaseManager.monstersPosition[playerStat.stage][i] - transform.position).magnitude < 80 && !databaseManager.monsters[playerStat.stage][i].isDead && !databaseManager.monsters[playerStat.stage][i].gameObject.activeSelf)
                    databaseManager.monsters[playerStat.stage][i].gameObject.SetActive(true);
                else if ((databaseManager.monstersPosition[playerStat.stage][i] - transform.position).magnitude >= 80 || databaseManager.monsters[playerStat.stage][i].isDead)
                    databaseManager.monsters[playerStat.stage][i].gameObject.SetActive(false);
            }
        }
    }
}
