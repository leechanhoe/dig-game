using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStat : MonoBehaviour
{
    static public PlayerStat instance;
    public float playTime;
    public bool hardMode;

    public int score;
    public int level;
    public int currentExp;
    public int[] neededExp;
    public int maxItemSlot;

    public int atk; // 공격력
    public int def; // 방어력
    public float momentum; // 추진력
    public int cri; // 크리티컬
    public float atkSpeed; // 공격속도
    public int criDmg; // 크리티컬 데미지
    public float speedX; // x축 이동속도
    public float skilling; // 숙련도
    public int explosivePower; // 폭발력
    
    public float maxHealth; // 최대체력
    public float curHealth; // 현재체력
    public float maxOxygen; // 최대산소
    public float curOxygen; // 현재산소

    public int depth;
    public int maxDepth; // 최대로 내려간 깊이

    public int stage; // 현재 스테이지
    public int maxStage; // 최대로 간 스테이지
    public List<int> visitedStage = new List<int>(); // 마지막으로 저장한 이후! 방문해서 수정된 스테이지들

    public int money;
    [SerializeField] string deathSound;
    public List<int> grade1Mineral { get; set; } = new List<int>();
    public List<int> grade2Mineral { get; set; } = new List<int>();
    public List<int> grade3Mineral { get; set; } = new List<int>();
    public List<int> grade4Mineral { get; set; } = new List<int>();

    [SerializeField] Slider H2OBar;
    [SerializeField] Slider HPBar;
    [SerializeField] Text H2OText;
    [SerializeField] Text HPText;
    [SerializeField] Slider ExpBar;

    [SerializeField] Text depthText;
    [SerializeField] Text levelText;

    [SerializeField] GameObject deathUI;
    [SerializeField] GameObject preventRevival;
    [SerializeField] GameObject preventLoad;
    [SerializeField] GameObject levelUpEffect;

    DatabaseManager database;
    SpriteRenderer spriteRenderer;
    Player player;
    GameManager gameManager;
    private void Awake()
    {
        if (instance == null) // 파괴방지
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
        else
            Destroy(this.gameObject);
    }
    void Start()
    {
        score = 0;
        level = 1;
        currentExp = 0;
        curHealth = maxHealth;
        curOxygen = maxOxygen;
        database = FindObjectOfType<DatabaseManager>();
        gameManager = FindObjectOfType<GameManager>();
        player = GetComponent<Player>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!player.isDeath)
        {
            Breathing();
            CheckH2O();
            CheckHP();
            CheckDepth();
        }

        if (gameManager.isPlaying)
            playTime += Time.deltaTime;
    }

    public void InitializeStat()
    {      
        money = 10000;
        maxItemSlot = 18;
        maxDepth = 0;
        maxStage = 0;
        maxHealth = 100;
        maxOxygen = 100;
        score = 0;
        level = 1;
        currentExp = 0;
        atk = 10;
        def = 0;
        momentum = 2.5f;
        cri = 10;
        atkSpeed = 4f;
        speedX = 2.5f;
        skilling = 0.6f;
        explosivePower = 0;
        MineralGradeEffectOff();

        grade1Mineral.Clear();
        grade2Mineral.Clear();
        grade3Mineral.Clear();
        grade4Mineral.Clear();

        playTime = 0;
        hardMode = false;
    }

    void Breathing()
    {
        if (player.notMove)
            return;
        switch (stage)
        {
            case 1:
                if (depth >= 0)
                {
                    curOxygen += Time.deltaTime * 100;
                    if (curOxygen >= maxOxygen)
                        curOxygen = maxOxygen;
                }
                else
                {
                    if (curOxygen <= 0)
                        curOxygen = 0;

                    if (hardMode)
                        curOxygen -= Time.deltaTime * (0.3f + (float)depth / -300f) * 1.2f;
                    else
                        curOxygen -= Time.deltaTime * (0.3f + (float)depth / -300f);
                }
                break;
            case 2:
                if (depth >= -400)
                {
                    curOxygen += Time.deltaTime * 100;
                    if (curOxygen >= maxOxygen)
                        curOxygen = maxOxygen;
                }
                else
                {
                    if (curOxygen <= 0)
                        curOxygen = 0;

                    if (hardMode)
                        curOxygen -= Time.deltaTime * (0.3f + (float)(depth + 200) / -300f) * 1.2f;
                    else
                        curOxygen -= Time.deltaTime * (0.3f + (float)(depth + 200) / -300f);
                }
                break;
        }
        H2OBar.value = curOxygen / maxOxygen;
        H2OText.text = (int)curOxygen + "/" + maxOxygen;
    }

    void CheckH2O()
    {
        if (curOxygen / maxOxygen >= 0.5)
            H2OBar.fillRect.GetComponent<Image>().color = Color.white;
        else if (curOxygen / maxOxygen >= 0.15)
            H2OBar.fillRect.GetComponent<Image>().color = new Color(215f/255,215f/255,215f/255);
        else
            H2OBar.fillRect.GetComponent<Image>().color = new Color(175f/255,175f/255,175f/255);

        if (curOxygen <= 0)
            curHealth -= Time.deltaTime * 10f;
    }

    void CheckHP()
    {
        HPBar.value = curHealth / maxHealth;

        if (curHealth <= 0 && !player.isDeath)
        {
            curHealth = 0;
            Death();
        }
        HPText.text = (int)curHealth + "/" + maxHealth;
    }

    void CheckDepth()
    {
        if (player.notMove)
            return;
        switch (stage)
        {
            case 1:
                depth = Mathf.FloorToInt(transform.position.y);
                if (0 < depth)
                    depth = 0;
                break;
            case 2:
                depth = Mathf.FloorToInt(transform.position.y) - 400;
                break;
        }
        depthText.text = depth.ToString() + "m";
        if (depth < maxDepth)
            maxDepth = depth;
    }

    public void Death()
    {
        DeathState();
        AudioManager.instance.Play(deathSound);
        Inventory.instance.CloseInventory();
        PlayCanvas.instance.ExitAction();
        FindObjectOfType<DataSlotUI>().CloseDataSlotUI();

        deathUI.SetActive(true);
        if (Inventory.instance.SearchItem(10205))
            preventRevival.SetActive(false);
        else
            preventRevival.SetActive(true);

        if (hardMode)
            preventLoad.SetActive(true);
        else
            preventLoad.SetActive(false);
    }

    public void DeathState()
    {
        player.isDeath = true;
        player.isInvincible = true;
        gameObject.layer = 11;
    }

    public void AliveState()
    {
        player.isDeath = false;
        player.isInvincible = false;
        gameObject.layer = 12;
    }

    public void DeathChoose(int type)
    {
        StartCoroutine(DeathChooseC(type));
    }

    IEnumerator DeathChooseC(int type)
    {
        deathUI.SetActive(false);
        yield return new WaitUntil(() => !deathUI.activeSelf);

        const int RIVIVAL = 0, LOAD = 1, GO_MAIN = 2;
        switch (type)
        {
            case RIVIVAL:
                StartCoroutine(RevivalC());
                break;
            case LOAD:
                FindObjectOfType<DataSlotUI>().OpenDataSlotUI(1);
                break;
            case GO_MAIN:
                GameManager.instance.GoToMain();
                break;
            default:
                break;
        }
    }

    IEnumerator RevivalC()
    {
        FadeManager fadeManager = FindObjectOfType<FadeManager>();
        fadeManager.FadeOutIn();
        yield return new WaitUntil(() => fadeManager.completedBlack);
        player.isDeath = false;
        player.isInvincible = true;
        gameObject.layer = 11;
        curHealth = maxHealth;

        for (int i = 0; i < 4; i++)
        {
            Color color = spriteRenderer.color;
            color.a = 0f;
            spriteRenderer.color = color;
            yield return new WaitForSeconds(0.3f);
            color.a = 1f;
            spriteRenderer.color = color;
            yield return new WaitForSeconds(0.3f);
        }
        player.isInvincible = false;
        gameObject.layer = 12;
    }

    public (int dmg, bool cri) Damage()
    {
        float damage = atk * Random.Range(skilling, 1.1f); // 여기서 데미지 범위 조정
        int criNum = Random.Range(1, 101);
        bool isCri = false;
        if (damage < 1)
            damage = 1;
        if (criNum <= cri)
        {
            damage *= criDmg / 100f;
            isCri = true;
        }
        return ((int)damage, isCri);
    }

    public void UpdateScore(int value = 0)
    {
        score += value;
        //scoreText.text = score.ToString();
    }

    public void AddGrade1Mineral(int id)
    {
        if (!grade1Mineral.Contains(id))
            grade1Mineral.Add(id);
        else
            Debug.Log("이미 있음");
    }

    public void AddGrade2Mineral(int id)
    {
        if (!grade2Mineral.Contains(id))
            grade2Mineral.Add(id);
        else
            Debug.Log("이미 있음");
    }

    public void AddGrade3Mineral(int id)
    {
        if (!grade3Mineral.Contains(id))
            grade3Mineral.Add(id);
        else
            Debug.Log("이미 있음");
    }

    public void AddGrade4Mineral(int id)
    {
        if (!grade4Mineral.Contains(id))
            grade4Mineral.Add(id);
        else
            Debug.Log("이미 있음");
    }

    public void MineralGradeEffectOn()
    {
        Shop[] shops = FindObjectsOfType<Shop>();
        DatabaseManager database = FindObjectOfType<DatabaseManager>();

        foreach (int itemID in grade2Mineral)
            database.ReturnItem(itemID).maxItemCount = 6;

        foreach (int itemID in grade3Mineral)
            database.ReturnItem(itemID).itemSalePrice = (int)(database.ReturnItem(itemID).itemSalePrice * 1.5f);

        foreach (Shop shop in shops)
        {
            foreach (int itemID in grade4Mineral)
            {
                if(!shop.saleEtcItems.Contains(itemID))
                    shop.saleEtcItems.Add(itemID);
            }
        }
    }

    public void MineralGradeEffectOff()
    {
        FindObjectOfType<DatabaseManager>().LoadItemList();
        Shop[] shops = FindObjectsOfType<Shop>();

        foreach (Shop shop in shops)
        {
            foreach (int itemID in grade4Mineral)
                shop.saleEtcItems.Remove(itemID);
        }

        grade1Mineral.Clear();
        grade2Mineral.Clear();
        grade3Mineral.Clear();
        grade4Mineral.Clear();
    }

    public void GetExp(int value)
    {
        if (value < 1)
            value = 1;
        currentExp += value;
        ExpBar.value = (float)currentExp / neededExp[level];
        if (currentExp >= neededExp[level])
            LevelUp();
    }

    public void LevelUp()
    {
        currentExp -= neededExp[level];
        if (levelUpEffect.activeSelf)
            levelUpEffect.SetActive(false);
        levelUpEffect.SetActive(true);
        level++;
        levelText.text = "Level " + level;
        maxHealth += 5;
        explosivePower++;
        ExpBar.value = (float)currentExp / neededExp[level];
    }

    public void ChangeStage(int value)
    {
        if (!visitedStage.Contains(stage) && stage != 0)
            visitedStage.Add(stage);
        if (!visitedStage.Contains(value) && value != 0)
            visitedStage.Add(value);

        stage = value;
        database.stage = value;

        if (visitedStage.Count < 1)
            Debug.LogError("방문한 스테이지가없음 ㄷㄷ");

        if (value > maxStage)
            maxStage = value;
    }
}
