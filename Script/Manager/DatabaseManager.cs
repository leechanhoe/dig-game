using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

public class DatabaseManager : MonoBehaviour
{
    static public DatabaseManager instance;
    public int mineralTypeNum;
    public int StageNum; // 스테이지 개수
    static public bool preventDoubleClick { get; set; }

    private PlayerStat playerStat;
    Player player;
    Inventory inventory;

    public bool[][] isGroundsOn { get; set; }
    public Ground[][] grounds { get; set; }
    public bool[][] isMineralsOn { get; set; }
    public Mineral[][] minerals { get; set; }
    public Dictionary<int, Vector3>[] mineralsPosition;//{ get; set; }
    public bool[][] isChestsOn { get; set; }
    public Chest[][] chests { get; set; }
    public Dictionary<int, Vector3>[] chestsPosition { get; set; }
    public bool[][] isRocksOn { get; set; }
    public GameObject[][] rocks { get; set; }
    public bool[][] isMonstersOn { get; set; }
    public Monster[][] monsters { get; set; }
    public Dictionary<int, Vector3>[] monstersPosition { get; set; }

    public Dictionary<int, int> totalMineralCount { get; set; } = new Dictionary<int, int>(); // 맵에 존재하는 광물의 각각 합
    public Dictionary<int, int> getMineralCount { get; set; } = new Dictionary<int, int>(); // 이제까저 먹은 광물의 각각 합
    public Dictionary<int, bool[]> isGetUpgradeReward { get; set; } = new Dictionary<int, bool[]>();

    public List<Item> itemList = new List<Item>();
    public List<Item> mineralList = new List<Item>();
    List<Item> inCoolTimeItem = new List<Item>();
    public int stage { get; set; }

    private void Awake()
    {
        if (instance == null) // 파괴방지
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
        else
            Destroy(this.gameObject);

        player = FindObjectOfType<Player>();
        inventory = FindObjectOfType<Inventory>();
        InitializeUpgradeReward();

        isGroundsOn = new bool[StageNum + 1][];
        grounds = new Ground[StageNum + 1][];
        isMineralsOn = new bool[StageNum + 1][];
        minerals = new Mineral[StageNum + 1][];
        isChestsOn = new bool[StageNum + 1][];
        chests = new Chest[StageNum + 1][];
        isRocksOn = new bool[StageNum + 1][];
        rocks = new GameObject[StageNum + 1][];
        isMonstersOn = new bool[StageNum + 1][];
        monsters = new Monster[StageNum + 1][];
        monstersPosition = new Dictionary<int, Vector3>[StageNum + 1];
        for (int i = 0; i < StageNum + 1; i++)
            monstersPosition[i] = new Dictionary<int, Vector3>();
        mineralsPosition = new Dictionary<int, Vector3>[StageNum + 1];
        for (int i = 0; i < StageNum + 1; i++)
            mineralsPosition[i] = new Dictionary<int, Vector3>();
        chestsPosition = new Dictionary<int, Vector3>[StageNum + 1];
        for (int i = 0; i < StageNum + 1; i++)
            chestsPosition[i] = new Dictionary<int, Vector3>();
    }

    private void Update()
    {
        if (inCoolTimeItem.Count > 0)
            RunCoolTime();
    }
    
    void RunCoolTime()
    {
        for (int i = 0; i < inCoolTimeItem.Count; i++)
        {
            inCoolTimeItem[i].curCoolTime -= Time.deltaTime;
            if(inCoolTimeItem[i].curCoolTime <= 0)
            {
                inCoolTimeItem[i].curCoolTime = 0;
                inCoolTimeItem.RemoveAt(i);
            }
        }
    }

    public void InitializeUpgradeReward()
    {
        for (int i = 0; i < mineralTypeNum; i++)
        {
            if (isGetUpgradeReward.Count == 0)
                isGetUpgradeReward.Add(40001 + i, new bool[5]);
            else
                isGetUpgradeReward[40001 + i] = new bool[5];
        }
    }

    public void LoadGround()
    {
        grounds[stage] = FindObjectsOfType<Ground>();
    }

    public void LoadMineral()
    {
        minerals[stage] = FindObjectsOfType<Mineral>();
        for (int i = 0; i < minerals[stage].Length; i++)
            minerals[stage][i].mineralIndex = i;
    }

    public void LoadChest()
    {
        chests[stage] = FindObjectsOfType<Chest>();
        for (int i = 0; i < chests[stage].Length; i++)
            chests[stage][i].chestIndex = i;
    }

    public void LoadMonster()
    {
        monsters[stage] = FindObjectsOfType<Monster>();
        for (int i = 0; i < monsters[stage].Length; i++)
            monsters[stage][i].enemyIndex = i;
    }

    public void LoadRock()
    {
        rocks[stage] = GameObject.FindGameObjectsWithTag("Rock");
    }

    public void CheckMineralNum() // 맵에 존재하는 광물 종류별로 개수 세기
    {
        for (int i = 0; i < minerals.Length; i++)
        {
            totalMineralCount[minerals[stage][i].itemID]++;
        }
    }

    public void InitializeMineralNum()
    {
        for (int i = 0; i < mineralTypeNum; i++)
        {
            totalMineralCount[40001 + i] = 0;
            getMineralCount[40001 + i] = 0;
        }
    }

    public void BeginningSetting() // 게임 맨처음 시작했을때
    {
        inventory.RemoveAllItem();
        for (int i = 0; i < inventory.QuickSlots.Length; i++)
        {
            inventory.QuickSlots[i].RemoveItem();
        }
        Equipment equipment = FindObjectOfType<Equipment>();
        equipment.RemoveAllEquipItem();
        
        LoadGround();
        LoadMineral();
        LoadChest();
        LoadRock();
        LoadMonster();

        InitializeIsOn(); ;
       
        CheckMineralNum();
        monstersPosition[stage].Clear();
        CheckIsMonsterOn();
    }

    void InitializeIsOn()
    {
        isGroundsOn[stage] = new bool[grounds[stage].Length];
        isMineralsOn[stage] = new bool[minerals[stage].Length];
        isChestsOn[stage] = new bool[chests[stage].Length];
        isRocksOn[stage] = new bool[rocks[stage].Length];
        isMonstersOn[stage] = new bool[monsters[stage].Length];
    }

    public void CheckIsGroundOn() // 땅이 파여졌나 체크
    {
        if (grounds[stage].Length != isGroundsOn.Length)
            isGroundsOn[stage] = new bool[grounds[stage].Length];
        for (int i = 0; i < grounds[stage].Length; i++)
        {
            if (grounds[stage][i].gameObject.activeSelf)
                isGroundsOn[stage][i] = true;
            else
                isGroundsOn[stage][i] = false;
        }
    }

    public void CheckIsMineralOn()
    {
        if (minerals[stage].Length != isMineralsOn.Length)
            isMineralsOn[stage] = new bool[minerals[stage].Length];
        for (int i = 0; i < minerals[stage].Length; i++)
        {
            if (minerals[stage][i].gameObject.activeSelf)
            {
                if (!minerals[stage][i].isdigged)
                    isMineralsOn[stage][i] = true;
                else
                    isMineralsOn[stage][i] = false;
            }
            else
                isMineralsOn[stage][i] = false;
            if (mineralsPosition[stage].ContainsKey(i))
                mineralsPosition[stage][i] = minerals[stage][i].transform.position;
        }
    }

    public void CheckIsChestOn()
    {
        if (chests[stage].Length != isChestsOn.Length)
            isChestsOn[stage] = new bool[chests[stage].Length];
        for (int i = 0; i < chests[stage].Length; i++)
        {
            if (chests[stage][i].gameObject.activeSelf)
            {
                if (!chests[stage][i].isdigged)
                    isChestsOn[stage][i] = true;
                else
                    isChestsOn[stage][i] = false;
            }
            else
                isChestsOn[stage][i] = false;
            if (chestsPosition[stage].ContainsKey(i))
                chestsPosition[stage][i] = chests[stage][i].transform.position;
        }
    }

    public void CheckIsRockOn()
    {
        if (rocks[stage].Length != isRocksOn.Length)
            isRocksOn[stage] = new bool[isRocksOn[stage].Length];
        for (int i = 0; i < rocks[stage].Length; i++)
        {
            if (rocks[stage][i].activeSelf)
                isRocksOn[stage][i] = true;
            else
                isRocksOn[stage][i] = false;
        }
    }

    public void CheckIsMonsterOn()
    {
        if (monsters[stage].Length != isMonstersOn.Length)
            isMonstersOn[stage] = new bool[monsters[stage].Length];
        for (int i = 0; i < monsters[stage].Length; i++)
        {
            if (monsters[stage][i].gameObject.activeSelf)
            {
                isMonstersOn[stage][i] = true;
                monstersPosition[stage][i] = monsters[stage][i].transform.position;
            }
            else
            {
                if (monsters[stage][i].isDead)
                    isMonstersOn[stage][i] = false;
                else
                    isMonstersOn[stage][i] = true;
            }
        }
        if (!monstersPosition[stage].Count.Equals(monsters[stage].Length))
            Debug.LogError("몬스터 포지션이랑 몬스터랑 개수 다름" + monstersPosition[stage].Count + "/" + monsters[stage].Length);
    }

    public void arrangeGround() 
    {
        if (isGroundsOn[stage] == null)
            return;
        
        for (int i = 0; i < isGroundsOn[stage].Length; i++)
        {
            if (isGroundsOn[stage][i])
                grounds[stage][i].gameObject.SetActive(true);
            else
            {
                grounds[stage][i].MinimapGroundOff();
                grounds[stage][i].gameObject.SetActive(false);
            }
        }
    }

    public void arrangeMineral()
    {
        if (isMineralsOn[stage] == null)
            return;
        for (int i = 0; i < isMineralsOn[stage].Length; i++)
        {
            if (isMineralsOn[stage][i])
            {
                minerals[stage][i].gameObject.SetActive(true);
                if (mineralsPosition[stage].ContainsKey(i))
                    minerals[stage][i].transform.position = mineralsPosition[stage][i];
            }
            else
                minerals[stage][i].gameObject.SetActive(false);
        }
    }

    public void arrangeChest()
    {
        if (isChestsOn[stage] == null)
            return;
        for (int i = 0; i < isChestsOn[stage].Length; i++)
        {
            if (isChestsOn[stage][i])
            {
                chests[stage][i].gameObject.SetActive(true);
                if (chestsPosition[stage].ContainsKey(i))
                    chests[stage][i].transform.position = chestsPosition[stage][i];
            }
            else
                chests[stage][i].gameObject.SetActive(false);
        }
    }

    public void arrangeRock()
    {
        if (isRocksOn[stage] == null)
            return;
        for (int i = 0; i < isRocksOn[stage].Length; i++)
        {
            if (isRocksOn[stage][i])
                rocks[stage][i].SetActive(true);
            else
                rocks[stage][i].SetActive(false);
        }
    }

    public void arrangeMonster()
    {
        if (isMonstersOn[stage] == null)
            return;
        for (int i = 0; i < isMonstersOn[stage].Length; i++)
        {
            if (isMonstersOn[stage][i])
            {
                monsters[stage][i].gameObject.SetActive(true);
                monsters[stage][i].transform.position = monstersPosition[stage][i];
            }
            else
            {
                monsters[stage][i].isDead = true;
                monsters[stage][i].gameObject.SetActive(false);
            }
        }
    }

    public bool UseItem(int _itemID,bool soundEffectOn = false) // 아이템 사용시 효과
    {
        Item item = ReturnItem(_itemID);
        bool failUse = false;
        if (item.curCoolTime > 0) // 쿨타임이 남아있으면
        {
            if (soundEffectOn)
                AudioManager.instance.Play("Beep");
            return false;
        }
        switch(_itemID)
        {        
            case 10001:
                if (playerStat.maxOxygen >= playerStat.curOxygen + 100)
                    playerStat.curOxygen += 100;
                else
                    playerStat.curOxygen = playerStat.maxOxygen;
                break;
            case 10002:
                if (playerStat.maxOxygen >= playerStat.curOxygen + 200)
                    playerStat.curOxygen += 200;
                else
                    playerStat.curOxygen = playerStat.maxOxygen;
                break;
            case 10003:
                if (playerStat.maxOxygen >= playerStat.curOxygen + 300)
                    playerStat.curOxygen += 300;
                else
                    playerStat.curOxygen = playerStat.maxOxygen;
                break;
            case 10004:
                if (playerStat.curHealth >= playerStat.curHealth + 50)
                    playerStat.curHealth += 50;
                else
                    playerStat.curHealth = playerStat.maxHealth;
                break;
            case 10005:
                if (playerStat.curHealth >= playerStat.curHealth + 100)
                    playerStat.curHealth += 100;
                else
                    playerStat.curHealth = playerStat.maxHealth;
                break;
            case 10006:
                if (playerStat.curHealth >= playerStat.curHealth + 200)
                    playerStat.curHealth += 200;
                else
                    playerStat.curHealth = playerStat.maxHealth;
                break;

            case 10101:
                if(!player.DropBomb())
                    failUse = true;
                break;
            case 10102:
                if (!player.DropDynamite())
                    failUse = true;
                break;
            case 10103:
                if (!player.DropDynamite(true))
                    failUse = true;
                break;
            case 10201:
                player.transform.position = new Vector3(0, 0.5f, player.transform.position.z);
                break;
            case 10202:
                if (!player.UseAdTeleportStone1())
                    failUse = true;
                break;
            case 10203:
                if (!player.UseAdTeleportStone2())
                    failUse = true;
                break;
            case 10204:
                playerStat.maxItemSlot++;
                break;
            default:
                if(_itemID % 10000 >= 1000) // 상자를 깠으면
                {
                    inventory.RemoveItem(_itemID);
                    inventory.GetAnItem(ReturnItem(_itemID).production, floatText: false);
                }
                failUse = true;
                break;
        }

        item.curCoolTime = item.coolTime;
        inCoolTimeItem.Add(item);

        if(failUse && soundEffectOn)
            AudioManager.instance.Play("Beep");

        return !failUse;
    }

    public Sprite GetItemIcon(int itemID)
    {
        for (int i = 0;i < itemList.Count;i++)
        {
            if (itemList[i].itemID == itemID)
                return itemList[i].itemIcon;
        }
        return itemList[0].itemIcon;
    }

    public Item ReturnItem(int _itemID)
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemList[i].itemID == _itemID)
                return itemList[i];
        }
        Debug.LogError("해당 아이템이 데이터베이스에 없습니다.");
        return null;
    }

    public void LoadItemList()
    {
        itemList.Clear();
        
        itemList.Add(new Item(40001, "석탄", "지질시대의 식물이 퇴적, 매몰된 후 열과 압력의 작용을 받아 변질 생성된 흑갈색의 가연성 암석. 현대에서 발전에 가장 많이 사용되는 원자재이다.", Item.ItemType.Mineral, 5, 100, _mineralDisribution: "땅 전체"));
        itemList.Add(new Item(40002, "철광석", "철을 제조할 수 있는 광물을 뜻한다.  지구 전체에서는 가장 흔한 금속으로, 질량의 32%를 철이 차지한다. 심지어 우주 전체에서도 모든 금속 가운데 가장 흔하다. 산업분야에서 매우 유용하게 쓰이며 가장 많이 사용되는 물질로, '산업의 쌀'이라는 별명이 있다.", Item.ItemType.Mineral, 5, 120));
        itemList.Add(new Item(40003, "구리", "구리는 붉은 빛을 띠는 금속으로 전기와 열의 전도성이 뛰어나다. 한자로는 동(銅)이며, 올림픽 등에서 1~3등에게 주는 메달 중 3등에게 주는 동메달의 주 재료이다.", Item.ItemType.Mineral, 5, 150));
        itemList.Add(new Item(40004, "은", "귀금속에 들어가는 은백색 금속으로, 가공성이 좋으면서도 금에 비해 가격이 낮기 때문에 공예 재료로 많이 사용된다. 최고급 식기의 재료로도 사용되었다.", Item.ItemType.Mineral, 5, 200));
        itemList.Add(new Item(40005, "황수정", "보석의 한 종류이며 수정의 변종이다. 일반적으로 주황색이나 레몬색을 띈다. 수정종류 중에서 가장 비싼 보석이며 가장 인기있는 수정류 보석이기도 하다. 시중의 주황색을 띄는 황수정은 자수정을 200~400도로 가열해서 만든 변색 황수정이다.", Item.ItemType.Mineral, 5, 250));
        itemList.Add(new Item(40006, "자수정", "자색의 보석으로, 2월의 탄생석이다. 보석 계열의 광물이지만 다른 고가의 보석에 비하면 그렇게 비싼 편은 아니다. 손톱만한 낮은 등급의 원석은 천 원 정도면 살 수 있다고 한다. 여담으로, 영하 40℃의 얼음과 강도가 같다고 한다.", Item.ItemType.Mineral, 5, 300));
        itemList.Add(new Item(40007, "아메트린", "아메트린은 돌에 보라색과 노란색의 4분의 3정도의 독특한 색상 조합이 있다. 자수정과 황수정과 같은 석영이기 때문에 함께 발견될 수 있는 곳에서만 발견된다. 긴장을 지우고, 마음을 편하게 해준다는 얘기가 있다.", Item.ItemType.Mineral, 5, 3000));
        itemList.Add(new Item(40008, "오팔", "오팔은 10월의 탄생석으로 희망과 결백을 상징한다. 특유의 무지개색의 광택색을 내는데 보는 방향에 따라 색깔이 다양하게 변한다. 루비 등 일반 보석과는 달리 물러서 흠집이 나기 쉽고 충격을 받으면 깨지기 쉬운 약한 보석이라 취급에 주의해야한다.", Item.ItemType.Mineral, 5, 30000));
        itemList.Add(new Item(40009, "금", "인류 문명에 큰 영향을 미친 금속들 중 하나이며, 역사적으로 어떤 시대에도 환금성을 보장받을 수 있는 귀금속의 제왕. 금은 반짝거리고 치밀하며, 아름다운 노란 빛깔의 황색을 띈다.", Item.ItemType.Mineral, 5, 300));
        itemList.Add(new Item(40010, "흑수정", "흑수정은 자신에게 붙은 사악한 힘이나 불행을 쫒아내어 영혼을 안정시키는데 도움을 준다고 알려져있다. 그래서 중세시대에는 성자가 악마를 퇴치할 때 쓰이기도 했다. 흑수정의 색은 방사능의 영향에 의해 만들어진 결정구조 속의 적자결함 때문이다.", Item.ItemType.Mineral, 5, 400));
        itemList.Add(new Item(40011, "에메랄드", "보석의 일종으로서, 녹주석 중 청록색을 띄는 종을 일컫는다. 세계 4대 보석이라고 말하는 사람이 있을 만큼 인지도와 가격이 대단한 보석이다. 신록의 계절에 걸맞게 5월의 탄생석으로 알려져 있다. 생명의 색으로, 영원불변을 의미하기도 한다.", Item.ItemType.Mineral, 5, 500));
        itemList.Add(new Item(40012, "루비", "빨간색 강옥으로, 7월의 탄생석이다. 루비는 예나 지금이나 최고급의 보석으로 손꼽히며, 브릴리언트 컷의 등장으로 제 빛을 보기 이전에는 다이아몬드도 루비에 한수 접을 정도였고, 루비는 여전히 다이아몬드에 견줄 만한 엄청난 값을 자랑하는, 고평가되는 보석이다.", Item.ItemType.Mineral, 5, 600));
        itemList.Add(new Item(40013, "문 스톤", "월장석이라고도 불리는 문 스톤은 달빛처럼 은은한 청백색을 가지고 있다. 월장석은 진주와 함께 6월의 탄생석으로 치며 결혼 13주년을 기념하기 위해 추천되는 보석이다.", Item.ItemType.Mineral, 5, 5000));
        itemList.Add(new Item(40014, "티타늄", "가볍고 단단하며, 거의 부식되지 않는다. 강철보다 훨씬 뛰어난 강도를 지닌다. 전설 속 광물인 미스릴과 비슷하다고 여겨진다..", Item.ItemType.Mineral, 5, 500));
        itemList.Add(new Item(40015, "아쿠아마린", "3월의 탄생석으로 영원한 젊음, 행복, 침착, 총명, 용감 등의 의미를 지녔다. 전설로는 인어가 몸을 치장하기 위해 보석함을 열다가 떨어뜨린게 아쿠아마린이라는 설도 있고,인어의 눈물이라는 설도 있다.또한 결혼 19주년을 기념하기 위해 추천되는 보석이다.", Item.ItemType.Mineral, 5, 550));
        itemList.Add(new Item(40016, "로즈 쿼츠", "분홍색 석영인 로즈 쿼츠는 마음의 돌,무조건적인 사랑의 의미를 가지고 있다. 하트 스톤(Heart Stone)이라고도 불리는 로즈 쿼츠는 가지고 있으면 사랑이 이루어지거나 유대관계를 유지하는데 도움을 준다고 여겨진다.", Item.ItemType.Mineral, 5, 200000));
        itemList.Add(new Item(40017, "핑크 다이아몬드", "핑크 다이아몬드는 오스트레일리아에서 밖에 나오지 않는다고 잘못 알려져 있지만, 사실 20%는 브라질, 러시아, 남아공 등 다른 몇몇 곳에서 산출된다. 핑크색이라는 이유만으로 일반 다이아몬드의 수십~수백 배의 가격을 받기도 한다", Item.ItemType.Mineral, 5, 4000));
        itemList.Add(new Item(40018, "스타 사파이어", "사파이어 중에서도 빛을 받을 경우 6방향으로 꼬리를 뻗은 별모양 무늬가 나타나는 보석을 스타 사파이어라고 한다. 이런 광채를 띠는 이유는 내부에 미세한 금홍석 결정이 방사형으로 분포되어 있기 때문이다.", Item.ItemType.Mineral, 5, 150000));
        itemList.Add(new Item(40019, "터키석", "12월의 탄생석으로도 알려져 있는 하늘색, 청록색 보석으로 기원전 5000년전부터 인류가 사용해왔던 역사가 깊은 보석 가운데 하나이다. 성공과 번영을 이끌어주고 액운을 막고, 성적 정열을 고조시켜 준다고도 알려져 있다. 한자로는 녹송석이라고 읽는다.", Item.ItemType.Mineral, 5, 200000));
        itemList.Add(new Item(40020, "석회석", "많은 생물은 저마다 외/내골격을 성장시키기 위해 탄산칼슘을 활용한다. 이들이 죽은 뒤 유해가 퇴적되어 단단하게 고결되면 석회암이 된다. 그중 희귀한 석회암 보석은 각도에 따라 색이 다르게 보인다.", Item.ItemType.Mineral, 5, 200000));
        itemList.Add(new Item(40021, "토르말린", "10월의 탄생석으로, '전기석'이라고 불린다. 광물 자체가 전기(에너지)를 발생한다. 지구상의 광물 중 유일하게 영구적인 전기 특성을 가지고 있다. 토르말린은 희망,행복,안락의 의미를 가지고 있다.", Item.ItemType.Mineral, 5, 50000));
        itemList.Add(new Item(40022, "캣츠아이", "고양이 눈을 닮은 돌이며, 한자로는 묘안석이라고도 불린다. 게임이나 만화 등 서브컬처의 영향으로 캣츠 아이라는 보석이 독립적으로 존재한다는 식으로 오해가 많이 퍼져있지만 이는 사실과 다르다. 보석의 표면에 고양이 눈과 같은 단백광을 내는 효과를 말한다.", Item.ItemType.Mineral, 5, 1300));
        itemList.Add(new Item(40023, "호박", "진주, 산호와 함께 정의상 광물은 아니지만 보석으로 취급된다. 보통 송진(수액)이 굳어서 100만년정도 지나면 호박이 된다. 벌레나 전갈 등이 들어 있는 호박은 유사 이래 나온 것을 다 모아도 벽장 하나에 들어갈 양 만큼밖에 되지 않는다고 한다.", Item.ItemType.Mineral, 5, 2200));
        itemList.Add(new Item(40024, "블러드 스톤", "3월의 탄생석이며 혈석이라고도 불린다. 말 그대로 암녹색 또는 청록색의 석영 위에 빨간 산화철이 흩어져 있는 것이 꼭 피가 묻은 것 같이 생겨서 붙은 이름이다. 중세 사람들은 붉은 점이 예수의 피라고 믿고 신통한 효험이 있는 돌이라고 생각했다고 한다.", Item.ItemType.Mineral, 5, 30000));
        itemList.Add(new Item(40025, "사파이어", "9월의 탄생석으로 지혜, 자애, 성실, 덕망을 뜻한다. 루비와 마찬가지로 강옥의 일종으로, 섞인 것이 달라 색깔이 다르게 나왔을 뿐이다. 푸른빛 뿐만 아니라 그린, 옐로우, 핑크 등 다양한 색상을 가질 수 있다.", Item.ItemType.Mineral, 5, 2500));
        itemList.Add(new Item(40026, "페리도트", "8월의 탄생석으로, 올리브색의 감람석이다. 부부의 행복과 친구와의 화합, 지혜, 혁명, 성실, 덕방 등의 의미를 지녔다. 또한 결혼 16주년을 기념하기 위해 추천되는 보석이다.", Item.ItemType.Mineral, 5, 2500));
        itemList.Add(new Item(40027, "가넷", "1월의 탄생석으로, 석류석이라고도 불린다. 가장 전형적인 보석 석류석의 색은 적포도주색 내지는 핏빛에 가까운 붉은색이 상징적이다. 진실과 우정을 상징한다. 또한 남녀 사이에서 이 장신구를 주고받는 것은 약혼이나 결혼의 의미를 지닌다.", Item.ItemType.Mineral, 5, 200000));
        itemList.Add(new Item(40028, "다이아몬드", "4월의 탄생석. 대표적인 보석 광물로서, 루비나 에메랄드, 금처럼 값비싼 물질의 상징으로 여겨진다. 순수, 영원 불변의 사랑의 의미를 가지고 있다. 또한, 여러 우수한 물성 때문에 매우 다양한 분야, 즉 공학, 과학, 예술 분야에서 빠질 수 없는 재료로 여겨진다.", Item.ItemType.Mineral, 5, 3000));
        itemList.Add(new Item(40029, "블랙 다이아몬드", "블랙 다이아몬드는 초월적인 힘의 상징으로 권력과 권위로 상징되어 왔고, 신비한 힘을 가지고 있다고 여겨져 왔다. 블랙 다이아몬드는 자체의 색이 검은색인 것이 아니라, 작은 다이아몬드 알갱이들과 흑연, 비정질 탄소가 공존하여 뭉쳐 있기에 검은 것이다.", Item.ItemType.Mineral, 5, 5000));
        itemList.Add(new Item(40030, "레드 다이아몬드", "다이아몬드의 끝판왕. 0.2g당 10억원이다. 지금까지 세계에서 발견된 레드 다이아몬드는 30개가 채 되지 않고, 대부분 0.1g미만이다. 안습하게도 루비로 판별되어 제 값을 받지 못하고 헐값에 팔리는 경우가 있고 인지도 또한 낮다는 속설이 있다.", Item.ItemType.Mineral, 5, 3000));
        itemList.Add(new Item(40031, "미스틱 쿼츠", "미스틱 쿼츠는 자연 무색으로 코딩되어 독특하고 신비한 무지개색을 내뿜는다. 이 흥미로운 무지개색은 기울어짐에 따라 변한다. 미스틱 쿼츠는 매력적인 변하는 색이 신비스럽고 드물기 때문에 가치가 높다.", Item.ItemType.Mineral, 5, 200000));
        itemList.Add(new Item(40032, "블랙 오팔", "블랙 오팔은 진회색부터 제트 블랙까지 어두운 색을 배경으로 하며, 배경색 위에 강렬하고 밝은 색상을 가지고 있어 여러 오팔중에서도 단연 최고급으로 분류된다. 붉은기가 강할수록 희소하다.", Item.ItemType.Mineral, 5, 200000));
        
        itemList.Add(new Item(10001, "하늘", "산소를 100만큼 회복한다", Item.ItemType.Use, 1, 300, 1000));
        itemList.Add(new Item(10002, "하늘하늘", "산소를 200만큼 회복한다", Item.ItemType.Use, 1, 900, 3000));
        itemList.Add(new Item(10003, "하늘하늘하늘", "산소를 400만큼 회복한다", Item.ItemType.Use, 1, 1800, 6000));
        itemList.Add(new Item(10004, "체리", "체력을 50만큼 회복한다", Item.ItemType.Use, 1, 300, 1000));
        itemList.Add(new Item(10005, "오렌지", "체력을 100만큼 회복한다", Item.ItemType.Use, 1, 900, 3000));
        itemList.Add(new Item(10006, "수박", "체력을 200만큼 회복한다", Item.ItemType.Use, 1, 1800, 6000));

        itemList.Add(new Item(10101, "폭탄", "적을 없앨 수 있는 기본 폭탄이다. 폭발력 + 10", Item.ItemType.Use, 1, 90, 300, _power: 10,coolTime:4.25f));
        itemList.Add(new Item(10102, "다이너마이트", "일반 바위 또는 적을 없앨 수 있다. 폭발력 + 15", Item.ItemType.Use, 1, 900, 3000, _power: 15, coolTime: 4.25f));
        itemList.Add(new Item(10103, "메가 다이너마이트", "강력한 다이너마이트. 단단한 바위 또는 적을 없앨 수 있다. 폭발력 + 20", Item.ItemType.Use, 1, 9000, 30000, _power: 20, coolTime: 4.25f));

        itemList.Add(new Item(10201, "텔레포트 스톤", "땅 위로 순간이동한다", Item.ItemType.Use, 1,9000, 30000));
        itemList.Add(new Item(10202, "어드밴스드 텔레포트 스톤", "현재 위치를 저장한다. 다시 저장한 위치로 이동할 수 있다. 저장된 위치 : 없음", Item.ItemType.Use, 1, 18000, 60000));
        itemList.Add(new Item(10203, "어드밴스드 텔레포트 스톤(위치 저장됨)", "현재 위치를 저장한다. 다시 저장한 위치로 이동할 수 있다. 저장된 위치 : 지하 ", Item.ItemType.Use, 1, 18000, 60000));
       
        itemList.Add(new Item(10204, "슬롯 증가권", "인벤토리 슬롯을 하나 증가시킨다.", Item.ItemType.Use, 1,_productMaterial: new int[] { 30002 }, _productMaterialCount: new int[] { 2 }));
        itemList.Add(new Item(10205, "부활의 돌", "죽었을 때 부활할 수 있다", Item.ItemType.Use, 1, 30000, 100000));

        itemList.Add(new Item(13401, "낡은 상자", "안에 무엇이 들었을까?", Item.ItemType.Use, 1, _production: 30401));
        itemList.Add(new Item(13104, "낡은 상자", "안에 무엇이 들었을까?", Item.ItemType.Use, 1, _production: 30104));
        itemList.Add(new Item(13106, "낡은 상자", "안에 무엇이 들었을까?", Item.ItemType.Use, 1, _production: 30106));
        itemList.Add(new Item(13108, "낡은 상자", "안에 무엇이 들었을까?", Item.ItemType.Use, 1, _production: 30108));

        itemList.Add(new Item(20101, "구리 드릴", "굴착력 + 2 / 크리티컬 + 3%", Item.ItemType.Equip, 1, 1500, 5000, 2, _cri: 3));
        itemList.Add(new Item(20102, "은 드릴", "굴착력 + 4 / 크리티컬 + 6%", Item.ItemType.Equip, 1, 3000, 10000, 4, _cri: 6));
        itemList.Add(new Item(20103, "금 드릴", "굴착력 + 6 / 크리티컬 + 9%", Item.ItemType.Equip, 1, 6000, 20000, 6, _cri: 9));
        itemList.Add(new Item(20104, "자수정 드릴", "굴착력 + 8 / 크리티컬 + 12%", Item.ItemType.Equip, 1, 15000, 50000, 8, _cri: 12));
        itemList.Add(new Item(20105, "에메랄드 드릴", "굴착력 + 10 / 크리티컬 + 15%", Item.ItemType.Equip, 1, 30000, 100000, 10, _cri: 15));
        itemList.Add(new Item(20106, "루비 드릴", "굴착력 + 12 / 크리티컬 + 18%", Item.ItemType.Equip, 1, 60000, 200000, 12, _cri: 18));
        itemList.Add(new Item(20107, "사파이어 드릴", "굴착력 + 15 / 크리티컬 + 21%", Item.ItemType.Equip, 1, 90000, 300000, 15, _cri: 21));
        itemList.Add(new Item(20108, "다이아몬드 드릴", "굴착력 + 20 / 크리티컬 + 25%", Item.ItemType.Equip, 1, 150000, 500000, 20, _cri: 25));

        itemList.Add(new Item(20201, "구리 헬멧", "최대산소 + 100 / 방어력 + 2", Item.ItemType.Equip, 1, 1500, 4500, _oxy: 100, _def:2));
        itemList.Add(new Item(20202, "은 헬멧", "최대산소 + 200 / 방어력 + 4", Item.ItemType.Equip, 1, 2700, 9000, _oxy: 200, _def:4));
        itemList.Add(new Item(20203, "금 헬멧", "최대산소 + 300 / 방어력 + 6", Item.ItemType.Equip, 1, 4500, 15000, _oxy: 300, _def:6));
        itemList.Add(new Item(20204, "자수정 헬멧", "최대산소 + 400 / 방어력 + 8", Item.ItemType.Equip, 1, 12000, 40000, _oxy: 400, _def:8));
        itemList.Add(new Item(20205, "에메랄드 헬멧", "최대산소 + 500 / 방어력 + 10", Item.ItemType.Equip, 1, 24000, 80000, _oxy: 500, _def: 10));
        itemList.Add(new Item(20206, "루비 헬멧", "최대산소 + 600 / 방어력 + 12", Item.ItemType.Equip, 1, 60000, 200000, _oxy: 600, _def: 12));
        itemList.Add(new Item(20207, "사파이어 헬멧", "최대산소 + 700 / 방어력 + 15", Item.ItemType.Equip, 1, 90000, 300000, _oxy: 700, _def: 15));
        itemList.Add(new Item(20208, "다이아몬드 헬멧", "최대산소 + 900 / 방어력 + 20", Item.ItemType.Equip, 1, 150000, 500000, _oxy: 900, _def: 20));

        itemList.Add(new Item(20301, "구리 슈즈", "추진력 + 0.5 / 이동속도 + 0.1", Item.ItemType.Equip, 1, 1500, 3500, _mom: 0.5f, _speedX: 0.1f));
        itemList.Add(new Item(20302, "은 슈즈", "추진력 + 1 / 이동속도 + 0.2", Item.ItemType.Equip, 1, 3000, 10000, _mom: 1f, _speedX: 0.2f));
        itemList.Add(new Item(20303, "금 슈즈,", "추진력 + 1.5 / 이동속도 + 0.3", Item.ItemType.Equip, 1, 6000, 20000, _mom: 1.5f, _speedX: 0.3f));
        itemList.Add(new Item(20304, "자수정 슈즈", "추진력 + 2 / 이동속도 + 0.4", Item.ItemType.Equip, 1, 12000, 40000, _mom: 2f, _speedX: 0.4f));
        itemList.Add(new Item(20305, "에메랄드 슈즈", "추진력 + 2.5 / 이동속도 + 0.5", Item.ItemType.Equip, 1, 24000, 80000, _mom: 2.5f, _speedX: 0.5f));
        itemList.Add(new Item(20306, "루비 슈즈", "추진력 + 3 / 이동속도 + 0.6", Item.ItemType.Equip, 1, 60000, 200000, _mom: 3f, _speedX: 0.6f));
        itemList.Add(new Item(20307, "사파이어 슈즈", "추진력 + 3.5 / 이동속도 + 0.8", Item.ItemType.Equip, 1, 90000, 300000, _mom: 3.5f, _speedX: 0.8f));
        itemList.Add(new Item(20308, "다이아몬드 슈즈", "추진력 + 4 / 이동속도 + 1", Item.ItemType.Equip, 1, 150000, 500000, _mom: 4f, _speedX: 1f));

        itemList.Add(new Item(20401, "구리 반지", "공격속도 + 0.5", Item.ItemType.Equip, 1, 1500, 3500, _atkSpeed: 0.5f, _productMaterial: new int[3] { 40001, 40002, 40003 }, _productMaterialCount: new int[3] { 2, 2, 2 }));
        itemList.Add(new Item(20402, "은 반지", "공격속도 + 1", Item.ItemType.Equip, 1, 3000, 10000, _atkSpeed: 1f));
        itemList.Add(new Item(20403, "금 반지,", "공격속도 + 1.5", Item.ItemType.Equip, 1, 6000, 20000, _atkSpeed: 1.5f));
        itemList.Add(new Item(20404, "자수정 반지", "공격속도 + 2", Item.ItemType.Equip, 1, 12000, 40000, _atkSpeed: 2f));
        itemList.Add(new Item(20405, "에메랄드 반지", "공격속도 + 2.5", Item.ItemType.Equip, 1, 24000, 80000, _atkSpeed: 2.5f));
        itemList.Add(new Item(20406, "루비 반지", "공격속도 + 3", Item.ItemType.Equip, 1, 60000, 200000, _atkSpeed: 3f));
        itemList.Add(new Item(20407, "사파이어 반지", "공격속도 + 3.5", Item.ItemType.Equip, 1, 90000, 300000, _atkSpeed: 3.5f));
        itemList.Add(new Item(20408, "다이아몬드 반지", "공격속도 + 4", Item.ItemType.Equip, 1, 150000, 500000, _atkSpeed: 4f));

        itemList.Add(new Item(20411, "구리 목걸이", "공격속도 + 0.25 / 숙련도 + 3%", Item.ItemType.Equip, 1, 1500, 3500, _atkSpeed: 0.25f, _skilling: 0.03f));
        itemList.Add(new Item(20412, "은 목걸이", "공격속도 + 0.5 / 숙련도 + 6%", Item.ItemType.Equip, 1, 3000, 10000, _atkSpeed: 0.5f, _skilling: 0.06f));
        itemList.Add(new Item(20413, "금 목걸이,", "공격속도 + 0.75 / 숙련도 + 9%", Item.ItemType.Equip, 1, 6000, 20000, _atkSpeed: 0.75f, _skilling: 0.09f));
        itemList.Add(new Item(20414, "자수정 목걸이", "공격속도 + 1 / 숙련도 + 12%", Item.ItemType.Equip, 1, 12000, 40000, _atkSpeed: 1f, _skilling: 0.12f));
        itemList.Add(new Item(20415, "에메랄드 목걸이", "공격속도 + 1.25 / 숙련도 15%", Item.ItemType.Equip, 1, 24000, 80000, _atkSpeed: 1.25f, _skilling: 0.15f));
        itemList.Add(new Item(20416, "루비 목걸이", "공격속도 + 1.5 / 숙련도 18%", Item.ItemType.Equip, 1, 60000, 200000, _atkSpeed: 1.5f, _skilling: 0.18f));
        itemList.Add(new Item(20417, "사파이어 목걸이", "공격속도 + 1.75 / 숙련도 21%", Item.ItemType.Equip, 1, 90000, 300000, _atkSpeed: 1.75f, _skilling: 0.21f));
        itemList.Add(new Item(20418, "다이아몬드 목걸이", "공격속도 + 2 / 숙련도 25%", Item.ItemType.Equip, 1, 150000, 500000, _atkSpeed: 2f, _skilling: 0.25f));

        itemList.Add(new Item(30101, "달팽이의 껍질", "달팽이의 껍질이다.", Item.ItemType.Etc, 1));
        itemList.Add(new Item(30102, "쪼개진 슬롯 증가권", "쪼개진 슬롯 증가권이다. 나머지 반쪽을 찾아보자.", Item.ItemType.Etc,2));
        itemList.Add(new Item(30103, "광부의 증표", "광부의 증표이다. 주로 아이템 제작에 쓰인다.", Item.ItemType.Etc, 1));
        itemList.Add(new Item(30104, "낡은 목장갑", "담임선생님이 아끼는 낡은 목장갑이다.", Item.ItemType.Etc, 1));
        itemList.Add(new Item(30105, "오염된 돌조각", "보통의 돌조각이 아닌 것 같다. 맨손으로 만지면 안될 것 같은 느낌이 든다.", Item.ItemType.Etc, 1));
        itemList.Add(new Item(30106, "마스크", "외부의 해로운 공기로부터 몸을 보호할 수 있다.", Item.ItemType.Etc, 1));
        itemList.Add(new Item(30107, "열쇠꾸러미", "누군가가 잃어버린 열쇠이다. 어디에 쓰이는 열쇠일까?", Item.ItemType.Etc, 1));
        itemList.Add(new Item(30108, "불의 원석", "뜨거운 불꽃이 담겨져있는 원석이다. 화상을 입지 않도록 조심히 다루자.", Item.ItemType.Etc, 1,_productMaterial: new int[3] { 40001, 40009, 40012 }, _productMaterialCount: new int[3] { 10, 10, 10 }));

        itemList.Add(new Item(31204, "슬롯 증가권 제조법", "", Item.ItemType.Etc, 1, _production: 10204));
        itemList.Add(new Item(32401, "구리 반지 제조법", "", Item.ItemType.Etc, 1,_production: 20401));
        itemList.Add(new Item(33108, "불의 원석 제조법", "", Item.ItemType.Etc, 1, _production: 30108));


        var minerals = from item in itemList
                      where item.itemID > 40000
                      select item;
        foreach (var mineral in minerals)
            mineralList.Add(mineral);
    }

    void Start()
    {
        playerStat = FindObjectOfType<PlayerStat>();
        LoadItemList();
        MineralCountSetting();
    }

    void MineralCountSetting() // 도감을 위한 광물 총개수와 이제까지 획득한 거 알수있는 변수 설정
    {
        int mineralNum = 0; // 광물 종류 개수
        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemList[i].itemType == Item.ItemType.Mineral)
                mineralNum++;
        }

        for (int i = 1; i < mineralNum + 1; i++)
        {
            totalMineralCount.Add(40000 + i, 0);
            getMineralCount.Add(40000 + i, 0);
        }
    }
}
