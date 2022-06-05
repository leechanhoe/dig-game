using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveNLoad : MonoBehaviour
{
    public int playDataSlotIndex; // 현재 플레이중인 데이터슬롯 인덱스
    static public SaveNLoad instance;

    PlayerStat playerStat;
    DatabaseManager database;
    Inventory inventory;
    Equipment equipment;
    Smithy smithy;
    QuestManager questManager;
    Storage storage;

    int nextStage;
    int stageNum;

    public int selectedSlot { get; set; }
    private void Awake()
    {
        if (instance == null) // 파괴방지
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
        else
            Destroy(this.gameObject);

        database = DatabaseManager.instance;
        equipment = FindObjectOfType<Equipment>();
        smithy = FindObjectOfType<Smithy>();
        questManager = FindObjectOfType<QuestManager>();
        storage = FindObjectOfType<Storage>();
    }

    private void Start()
    {
        playerStat = PlayerStat.instance;
        inventory = FindObjectOfType<Inventory>();
        stageNum = database.StageNum;
    }

    public void CallSave()
    {
        if (selectedSlot == 0)
        {
            Debug.LogError("슬롯인덱스가 0임 1~3이여야함");
            return;
        }

        int stage = playerStat.stage;

        PlayerPrefs.SetFloat(selectedSlot + "playTime", playerStat.playTime);
        PlayerPrefs.SetInt(selectedSlot + "maxDepth", playerStat.maxDepth);
        PlayerPrefs.SetString(selectedSlot + "hardMode", playerStat.hardMode.ToString());

        PlayerPrefs.SetInt(selectedSlot + "stage", stage);
        PlayerPrefs.SetInt(selectedSlot + "maxStage", playerStat.maxStage);

        PlayerPrefs.SetInt(selectedSlot + "level", playerStat.level);
        PlayerPrefs.SetInt(selectedSlot + "currentExp", playerStat.currentExp);

        PlayerPrefs.SetInt(selectedSlot + "maxItemSlot", playerStat.maxItemSlot);
        PlayerPrefs.SetInt(selectedSlot + "score", playerStat.score);
        PlayerPrefs.SetInt(selectedSlot + "atk", playerStat.atk - equipment.added_atk);
        PlayerPrefs.SetInt(selectedSlot + "def", playerStat.def - equipment.added_def);
        PlayerPrefs.SetFloat(selectedSlot + "maxHealth", playerStat.maxHealth);
        PlayerPrefs.SetFloat(selectedSlot + "maxOxygen", playerStat.maxOxygen - equipment.added_oxy);
        PlayerPrefs.SetInt(selectedSlot + "money", playerStat.money);
        PlayerPrefs.SetInt(selectedSlot + "cri", playerStat.cri - equipment.added_cri);
        PlayerPrefs.SetFloat(selectedSlot + "momentum", playerStat.momentum - equipment.added_mom);
        PlayerPrefs.SetFloat(selectedSlot + "atkSpeed", playerStat.atkSpeed - equipment.added_atkSpeed);
        PlayerPrefs.SetFloat(selectedSlot + "speedX", playerStat.speedX - equipment.added_speedX);
        PlayerPrefs.SetFloat(selectedSlot + "skilling", playerStat.skilling - equipment.added_skilling);
        PlayerPrefs.SetInt(selectedSlot + "explosivePower", playerStat.explosivePower - equipment.added_explosivePower);

        PlayerPrefs.SetInt(selectedSlot + "questIndex", questManager.questIndex);
        PlayerPrefs.SetString(selectedSlot + "isProgress", questManager.isProgress.ToString());

        string storageItemList = "", storageItemCount = "";
        for (int i = 0; i < storage.storageItemList.Count; i++)
        {
            storageItemList += storage.storageItemList[i].itemID;
            storageItemCount += storage.storageItemList[i].storageItemCount;
            if (i < storage.storageItemList.Count - 1)
            {
                storageItemList += ",";
                storageItemCount += ",";
            }
        }
        PlayerPrefs.SetString(selectedSlot + "storageItemList", storageItemList);
        PlayerPrefs.SetString(selectedSlot + "storageItemCount", storageItemCount);

        string itemList = "", itemCount = "";
        for (int i = 0; i < inventory.inventoryItemList.Count; i++)
        {
            itemList += inventory.inventoryItemList[i].itemID;
            itemCount += inventory.inventoryItemList[i].itemCount;
            if (i < inventory.inventoryItemList.Count - 1)
            {
                itemList += ",";
                itemCount += ",";
            }
        }
        PlayerPrefs.SetString(selectedSlot + "inventoryItemList", itemList);
        PlayerPrefs.SetString(selectedSlot + "inventoryItemCount", itemCount);

        string equipItem = "";
        for (int i = 0; i < equipment.equipItemList.Length; i++)
        {
            equipItem += equipment.equipItemList[i].itemID;
            if (i < equipment.equipItemList.Length - 1)
                equipItem += ",";
        }
        PlayerPrefs.SetString(selectedSlot + "equipItem", equipItem);

        foreach (int visitedStage in playerStat.visitedStage)
        {
            if (visitedStage.Equals(stage))
            {
                database.CheckIsGroundOn(); // 땅 정보 저장
                StringBuilder isGroundsOn = new StringBuilder();
                for (int i = 0; i < database.isGroundsOn[visitedStage].Length; i++)
                {
                    isGroundsOn.Append(database.isGroundsOn[visitedStage][i]);
                    if (i < database.isGroundsOn[visitedStage].Length - 1)
                        isGroundsOn.Append(",");
                }
                PlayerPrefs.SetString(selectedSlot + "isGroundsOn" + visitedStage, isGroundsOn.ToString());

                database.CheckIsMineralOn(); // 광물 정보 저장
                StringBuilder isMineralsOn = new StringBuilder();
                for (int i = 0; i < database.isMineralsOn[visitedStage].Length; i++)
                {
                    isMineralsOn.Append(database.isMineralsOn[visitedStage][i]);
                    if (i < database.isMineralsOn[visitedStage].Length - 1)
                        isMineralsOn.Append(",");
                }
                PlayerPrefs.SetString(selectedSlot + "isMineralsOn" + visitedStage, isMineralsOn.ToString());

                database.CheckIsChestOn(); // 상자 정보 저장
                StringBuilder isChestsOn = new StringBuilder();
                for (int i = 0; i < database.isChestsOn[visitedStage].Length; i++)
                {
                    isChestsOn.Append(database.isChestsOn[visitedStage][i]);
                    if (i < database.isChestsOn[visitedStage].Length - 1)
                        isChestsOn.Append(",");
                }
                PlayerPrefs.SetString(selectedSlot + "isChestsOn" + visitedStage, isChestsOn.ToString());

                database.CheckIsRockOn(); // 바위 정보 저장
                StringBuilder isRocksOn = new StringBuilder();
                for (int i = 0; i < database.isRocksOn[visitedStage].Length; i++)
                {
                    isRocksOn.Append(database.isRocksOn[visitedStage][i]);
                    if (i < database.isRocksOn[visitedStage].Length - 1)
                        isRocksOn.Append(",");
                }
                PlayerPrefs.SetString(selectedSlot + "isRocksOn" + visitedStage, isRocksOn.ToString());

                database.CheckIsMonsterOn(); // 몹 정보 저장
                StringBuilder isMonstersOn = new StringBuilder();
                for (int i = 0; i < database.isMonstersOn[visitedStage].Length; i++)
                {
                    isMonstersOn.Append(database.isMonstersOn[visitedStage][i]);
                    if (i < database.isMonstersOn[visitedStage].Length - 1)
                        isMonstersOn.Append(",");
                }
                PlayerPrefs.SetString(selectedSlot + "isMonstersOn" + visitedStage, isMonstersOn.ToString());

                PlayerPrefs.SetInt(selectedSlot + "monsterNum" + visitedStage, database.monstersPosition[visitedStage].Count);
                int index = 0;
                foreach (var monsterPos in database.monstersPosition[visitedStage])
                {
                    PlayerPrefs.SetInt(selectedSlot + "monster" + index + visitedStage, monsterPos.Key);
                    PlayerPrefs.SetFloat(selectedSlot + "monster" + monsterPos.Key + "PosX" + visitedStage, monsterPos.Value.x);
                    PlayerPrefs.SetFloat(selectedSlot + "monster" + monsterPos.Key + "PosY" + visitedStage, monsterPos.Value.y);
                    PlayerPrefs.SetFloat(selectedSlot + "monster" + monsterPos.Key + "PosZ" + visitedStage, monsterPos.Value.z);
                    index++;
                }
            }
            else
            {
                PlayerPrefs.SetString(selectedSlot + "isGroundsOn" + visitedStage, PlayerPrefs.GetString("(Temp)isGroundsOn" + visitedStage));
                PlayerPrefs.SetString(selectedSlot + "isMineralsOn" + visitedStage, PlayerPrefs.GetString("(Temp)isMineralsOn" + visitedStage));
                PlayerPrefs.SetString(selectedSlot + "isChestsOn" + visitedStage, PlayerPrefs.GetString("(Temp)isChestsOn" + visitedStage));
                PlayerPrefs.SetString(selectedSlot + "isRocksOn" + visitedStage, PlayerPrefs.GetString("(Temp)isRocksOn" + visitedStage));
                PlayerPrefs.SetString(selectedSlot + "isMonstersOn" + visitedStage, PlayerPrefs.GetString("(Temp)isMonstersOn" + visitedStage));

                PlayerPrefs.SetInt(selectedSlot + "monsterNum" + visitedStage, PlayerPrefs.GetInt("(Temp)monsterNum" + visitedStage));
                for (int i = 0; i < PlayerPrefs.GetInt("(Temp)monsterNum" + visitedStage); i++)
                {
                    int monsterIndex = PlayerPrefs.GetInt("(Temp)monster" + i + visitedStage);
                    PlayerPrefs.SetInt(selectedSlot + "monster" + i + visitedStage, monsterIndex);

                    PlayerPrefs.SetFloat(selectedSlot + "monster" + monsterIndex + "PosX" + visitedStage, PlayerPrefs.GetFloat("(Temp)monster" + monsterIndex + "PosX" + visitedStage));
                    PlayerPrefs.SetFloat(selectedSlot + "monster" + monsterIndex + "PosY" + visitedStage, PlayerPrefs.GetFloat("(Temp)monster" + monsterIndex + "PosY" + visitedStage));
                    PlayerPrefs.SetFloat(selectedSlot + "monster" + monsterIndex + "PosZ" + visitedStage, PlayerPrefs.GetFloat("(Temp)monster" + monsterIndex + "PosZ" + visitedStage));
                }
            }
        }
        for (int stagei = 1; stagei < database.StageNum + 1; stagei++) // 랜덤으로 배치됐던 오브젝트들 저장
        {
            int k = 0;
            PlayerPrefs.SetInt(selectedSlot + "mineralNum" + stagei, database.mineralsPosition[stagei].Count);
            foreach (var mineral in database.mineralsPosition[stagei])
            {
                PlayerPrefs.SetInt(selectedSlot + "mineral" + k + stagei,mineral.Key);
                PlayerPrefs.SetFloat(selectedSlot + "mineral" + mineral.Key + "PosX" + stagei, mineral.Value.x);
                PlayerPrefs.SetFloat(selectedSlot + "mineral" + mineral.Key + "PosY" + stagei, mineral.Value.y);
                PlayerPrefs.SetFloat(selectedSlot + "mineral" + mineral.Key + "PosZ" + stagei, mineral.Value.z);
                k++;
            }
        }

        for (int stagei = 1; stagei < database.StageNum + 1; stagei++) // 랜덤으로 배치됐던 오브젝트들 저장
        {
            int k = 0;
            PlayerPrefs.SetInt(selectedSlot + "chestNum" + stagei, database.chestsPosition[stagei].Count);
            foreach (var chest in database.chestsPosition[stagei])
            {
                PlayerPrefs.SetInt(selectedSlot + "chest" + k + stagei, chest.Key);
                PlayerPrefs.SetFloat(selectedSlot + "chest" + chest.Key + "PosX" + stagei, chest.Value.x);
                PlayerPrefs.SetFloat(selectedSlot + "chest" + chest.Key + "PosY" + stagei, chest.Value.y);
                PlayerPrefs.SetFloat(selectedSlot + "chest" + chest.Key + "PosZ" + stagei, chest.Value.z);
                k++;
            }
        }

        for (int i = 0; i < database.mineralTypeNum; i++) // 도감 정보 저장
            PlayerPrefs.SetInt(selectedSlot + "MineralNum" + (40001 + i), database.getMineralCount[40001 + i]);

        for (int i = 0; i < database.mineralTypeNum; i++)
        {
            for (int j = 0; j < database.isGetUpgradeReward[40001 + i].Length; j++)
                PlayerPrefs.SetString(selectedSlot + ("isGetUpgradeReward" + (40001 + i)) + j, database.isGetUpgradeReward[40001 + i][j].ToString());
        }

        PlayerPrefs.SetInt(selectedSlot + "grade1MineralCount", playerStat.grade1Mineral.Count);
        for (int i = 0; i < playerStat.grade1Mineral.Count; i++)
            PlayerPrefs.SetInt(selectedSlot + "grade1Mineral" + i, playerStat.grade1Mineral[i]);
        PlayerPrefs.SetInt(selectedSlot + "grade2MineralCount", playerStat.grade2Mineral.Count);
        for (int i = 0; i < playerStat.grade2Mineral.Count; i++)
            PlayerPrefs.SetInt(selectedSlot + "grade2Mineral" + i, playerStat.grade2Mineral[i]);
        PlayerPrefs.SetInt(selectedSlot + "grade3MineralCount", playerStat.grade3Mineral.Count);
        for (int i = 0; i < playerStat.grade3Mineral.Count; i++)
            PlayerPrefs.SetInt(selectedSlot + "grade3Mineral" + i, playerStat.grade3Mineral[i]);
        PlayerPrefs.SetInt(selectedSlot + "grade4MineralCount", playerStat.grade4Mineral.Count);
        for (int i = 0; i < playerStat.grade4Mineral.Count; i++)
            PlayerPrefs.SetInt(selectedSlot + "grade4Mineral" + i, playerStat.grade4Mineral[i]);

        for (int i = 0; i < smithy.production.Count; i++) // 대장간 정보 저장
            PlayerPrefs.SetInt(selectedSlot + "smithyProduction" + i, smithy.production[i]);
        PlayerPrefs.SetInt(selectedSlot + "productionCount", smithy.production.Count);

        selectedSlot = 0;
        GetComponentInChildren<DataSlotUI>().ShowSlot();

        playerStat.visitedStage.Clear();
        playerStat.visitedStage.Add(stage);
        RemoveTemporaryData();

        StartCoroutine(CompleteSaveC());
    }
    IEnumerator CompleteSaveC()
    {
        yield return new WaitForSeconds(0.05f);
        InformationPanel.instance.EnableOK(Vector2.zero, "저장이 완료되었습니다.", "확인");
    }

    public void CallLoad()
    {
        if (!PlayerPrefs.HasKey(selectedSlot + "playTime") || selectedSlot == 0)
        {
            Debug.LogError("불러올 데이터가 없음");
            return;
        }
        playDataSlotIndex = selectedSlot;
        GetComponentInChildren<DataSlotUI>().CloseDataSlotUI();

        playerStat.playTime = PlayerPrefs.GetFloat(selectedSlot + "playTime");
        playerStat.level = PlayerPrefs.GetInt(selectedSlot + "level");
        playerStat.hardMode = Convert.ToBoolean(PlayerPrefs.GetString(selectedSlot + "hardMode"));
        playerStat.currentExp = PlayerPrefs.GetInt(selectedSlot + "currentExp");
        playerStat.maxDepth = PlayerPrefs.GetInt(selectedSlot + "maxDepth");

        nextStage = PlayerPrefs.GetInt(selectedSlot + "stage");
        playerStat.maxStage = PlayerPrefs.GetInt(selectedSlot + "maxStage");

        playerStat.maxItemSlot = PlayerPrefs.GetInt(selectedSlot + "maxItemSlot");
        playerStat.score = PlayerPrefs.GetInt(selectedSlot + "score");
        playerStat.atk = PlayerPrefs.GetInt(selectedSlot + "atk");
        playerStat.def = PlayerPrefs.GetInt(selectedSlot + "def");
        playerStat.maxHealth = PlayerPrefs.GetFloat(selectedSlot + "maxHealth");
        playerStat.maxOxygen = PlayerPrefs.GetFloat(selectedSlot + "maxOxygen");
        playerStat.money = PlayerPrefs.GetInt(selectedSlot + "money");
        playerStat.curOxygen = playerStat.maxOxygen;
        playerStat.cri = PlayerPrefs.GetInt("cri");
        playerStat.momentum = PlayerPrefs.GetFloat(selectedSlot + "momentum");
        playerStat.atkSpeed = PlayerPrefs.GetFloat(selectedSlot + "atkSpeed");
        playerStat.speedX = PlayerPrefs.GetFloat(selectedSlot + "speedX");
        playerStat.skilling = PlayerPrefs.GetFloat(selectedSlot + "skilling");
        playerStat.explosivePower = PlayerPrefs.GetInt(selectedSlot + "explosivePower");

        questManager.questIndex = PlayerPrefs.GetInt(selectedSlot + "questIndex");
        questManager.isProgress = Convert.ToBoolean(PlayerPrefs.GetString(selectedSlot + "isProgress"));

        string[] inventoryItemList = PlayerPrefs.GetString(selectedSlot + "inventoryItemList").Split(',');
        string[] inventoryItemCount = PlayerPrefs.GetString(selectedSlot + "inventoryItemCount").Split(',');
        inventory.inventoryItemList.Clear();
        if (inventoryItemList[0] != "")
        {
            for (int i = 0; i < inventoryItemList.Length; i++)
            {
                for (int j = 0; j < database.itemList.Count; j++)
                {
                    if (database.itemList[j].itemID == Convert.ToInt32(inventoryItemList[i]))
                    {
                        inventory.inventoryItemList.Add(database.itemList[j]);
                        inventory.inventoryItemList[inventory.inventoryItemList.Count - 1].itemCount = Convert.ToInt32(inventoryItemCount[i]);
                        break;
                    }
                }
            }
        }

        string[] storageItemList = PlayerPrefs.GetString(selectedSlot + "storageItemList").Split(',');
        string[] storageItemCount = PlayerPrefs.GetString(selectedSlot + "storageItemCount").Split(',');
        storage.storageItemList.Clear();
        if (storageItemList[0] != "")
        {
            for (int i = 0; i < storageItemList.Length; i++)
            {
                for (int j = 0; j < database.itemList.Count; j++)
                {
                    if (database.itemList[j].itemID == Convert.ToInt32(storageItemList[i]))
                    {
                        storage.storageItemList.Add(database.itemList[j]);
                        storage.storageItemList[storage.storageItemList.Count - 1].storageItemCount = Convert.ToInt32(storageItemCount[i]);
                        break;
                    }
                }
            }
        }

        string[] equipItem = PlayerPrefs.GetString(selectedSlot + "equipItem").Split(',');
        for (int i = 0; i < equipment.equipItemList.Length; i++)
            equipment.equipItemList[i] = new Item();
        if (equipItem[0] != "")
        {
            for (int i = 0; i < equipItem.Length; i++)
            {
                for (int j = 0; j < database.itemList.Count; j++)
                {
                    if (database.itemList[j].itemID == Convert.ToInt32(equipItem[i]))
                    {
                        equipment.EquipItem(database.itemList[j]);
                        break;
                    }
                }
            }
        }

        for (int stage = 1; stage <= playerStat.maxStage; stage++)
        {

            string[] isGroundsOn = PlayerPrefs.GetString(selectedSlot + "isGroundsOn" + stage).Split(','); // 땅 정보 불러오기
            database.isGroundsOn[stage] = new bool[isGroundsOn.Length];
            for (int i = 0; i < isGroundsOn.Length; i++)
                database.isGroundsOn[stage][i] = Convert.ToBoolean(isGroundsOn[i]);

            string[] isMineralsOn = PlayerPrefs.GetString(selectedSlot + "isMineralsOn" + stage).Split(','); // 광물 정보 불러오기
            database.isMineralsOn[stage] = new bool[isMineralsOn.Length];
            for (int i = 0; i < isMineralsOn.Length; i++)
                database.isMineralsOn[stage][i] = Convert.ToBoolean(isMineralsOn[i]);
            database.mineralsPosition[stage].Clear();
            for (int i = 0; i < PlayerPrefs.GetInt(selectedSlot + "mineralNum" + stage); i++)
            {
                int mineralIndex = PlayerPrefs.GetInt(selectedSlot + "mineral" + i + stage);
                database.mineralsPosition[stage][mineralIndex] = new Vector3(PlayerPrefs.GetFloat(selectedSlot + "mineral" + mineralIndex + "PosX" + stage), PlayerPrefs.GetFloat(selectedSlot + "mineral" + mineralIndex + "PosY" + stage), PlayerPrefs.GetFloat(selectedSlot + "mineral" + mineralIndex + "PosZ" + stage));
            }

            string[] isChestsOn = PlayerPrefs.GetString(selectedSlot + "isChestsOn" + stage).Split(','); // 상자 정보 불러오기
            database.isChestsOn[stage] = new bool[isChestsOn.Length];
            for (int i = 0; i < isChestsOn.Length; i++)
                database.isChestsOn[stage][i] = Convert.ToBoolean(isChestsOn[i]);
            database.chestsPosition[stage].Clear();
            for (int i = 0; i < PlayerPrefs.GetInt(selectedSlot + "chestNum" + stage); i++)
            {
                int chestIndex = PlayerPrefs.GetInt(selectedSlot + "chest" + i + stage);
                database.chestsPosition[stage][chestIndex] = new Vector3(PlayerPrefs.GetFloat(selectedSlot + "chest" + chestIndex + "PosX" + stage), PlayerPrefs.GetFloat(selectedSlot + "chest" + chestIndex + "PosY" + stage), PlayerPrefs.GetFloat(selectedSlot + "chest" + chestIndex + "PosZ" + stage));
            }

            string[] isRocksOn = PlayerPrefs.GetString(selectedSlot + "isRocksOn" + stage).Split(','); // 바위 정보 불러오기
            database.isRocksOn[stage] = new bool[isRocksOn.Length];
            for (int i = 0; i < isRocksOn.Length; i++)
                database.isRocksOn[stage][i] = Convert.ToBoolean(isRocksOn[i]);

            string[] isMonstersOn = PlayerPrefs.GetString(selectedSlot + "isMonstersOn" + stage).Split(','); // 바위 정보 불러오기
            database.isMonstersOn[stage] = new bool[isMonstersOn.Length];
            for (int i = 0; i < isMonstersOn.Length; i++)
                database.isMonstersOn[stage][i] = Convert.ToBoolean(isMonstersOn[i]);
            database.monstersPosition[stage].Clear();
            for (int i = 0; i < PlayerPrefs.GetInt(selectedSlot + "monsterNum" + stage); i++)
            {
                int monsterIndex = PlayerPrefs.GetInt(selectedSlot + "monster" + i + stage);
                database.monstersPosition[stage][monsterIndex] = new Vector3(PlayerPrefs.GetFloat(selectedSlot + "monster" + monsterIndex + "PosX" + stage), PlayerPrefs.GetFloat(selectedSlot + "monster" + monsterIndex + "PosY" + stage), PlayerPrefs.GetFloat(selectedSlot + "monster" + monsterIndex + "PosZ" + stage));
            }
        }

        for (int i = 0; i < database.mineralTypeNum; i++) // 도감 정보 불러오기
            database.getMineralCount[40001 + i] = PlayerPrefs.GetInt(selectedSlot + "MineralNum" + (40001 + i));

        for (int i = 0; i < database.mineralTypeNum; i++)
        {
            for (int j = 0; j < database.isGetUpgradeReward[40001 + i].Length; j++)
                database.isGetUpgradeReward[40001 + i][j] = Convert.ToBoolean(PlayerPrefs.GetString(((selectedSlot + "isGetUpgradeReward" + (40001 + i)) + j)));
        }
        //도감효과
        for (int i = 0; i < PlayerPrefs.GetInt(selectedSlot + "grade1MineralCount"); i++)
            playerStat.grade1Mineral.Add(PlayerPrefs.GetInt(selectedSlot + "grade1Mineral" + i));
        for (int i = 0; i < PlayerPrefs.GetInt(selectedSlot + "grade2MineralCount"); i++)
            playerStat.grade2Mineral.Add(PlayerPrefs.GetInt(selectedSlot + "grade2Mineral" + i));
        for (int i = 0; i < PlayerPrefs.GetInt(selectedSlot + "grade3MineralCount"); i++)
            playerStat.grade3Mineral.Add(PlayerPrefs.GetInt(selectedSlot + "grade3Mineral" + i));
        for (int i = 0; i < PlayerPrefs.GetInt(selectedSlot + "grade4MineralCount"); i++)
            playerStat.grade4Mineral.Add(PlayerPrefs.GetInt(selectedSlot + "grade4Mineral" + i));

        for (int i = 0; i < PlayerPrefs.GetInt(selectedSlot + "productionCount"); i++) // 대장간 정보 불러오기
            smithy.production.Add(PlayerPrefs.GetInt(selectedSlot + "smithyProduction" + i));

        StartCoroutine(WaitC());
    }

    IEnumerator WaitC()
    {
        FadeManager fadeManager = FindObjectOfType<FadeManager>();
        fadeManager.FadeOut();
        yield return new WaitUntil(() => fadeManager.completedBlack);
        if (nextStage < 1)
            Debug.LogError("스테이지값이 0임. 1~5여야함");
        LoadingSceneManager.LoadScene("Stage" + nextStage);
    }

    public void RemoveData()
    {
        if (selectedSlot == 0)
            return;
        PlayerPrefs.DeleteKey(selectedSlot + "playTime");

        InformationPanel.instance.EnableOK(Vector2.zero, "삭제가 완료되었습니다.");
        GetComponentInChildren<DataSlotUI>().ShowSlot();
    }

    public void SaveTemporaryMapData(int stage) // 스테이지를 이동할 때 기존 맵 저장하는거
    {
        database.CheckIsGroundOn(); // 땅 정보 저장
        StringBuilder isGroundsOn = new StringBuilder();
        for (int i = 0; i < database.isGroundsOn[stage].Length; i++)
        {
            isGroundsOn.Append(database.isGroundsOn[stage][i]);
            if (i < database.isGroundsOn[stage].Length - 1)
                isGroundsOn.Append(",");
        }
        PlayerPrefs.SetString("(Temp)isGroundsOn" + stage, isGroundsOn.ToString());

        database.CheckIsMineralOn(); // 광물 정보 저장
        StringBuilder isMineralsOn = new StringBuilder();
        for (int i = 0; i < database.isMineralsOn[stage].Length; i++)
        {
            isMineralsOn.Append(database.isMineralsOn[stage][i]);
            if (i < database.isMineralsOn[stage].Length - 1)
                isMineralsOn.Append(",");
        }
        PlayerPrefs.SetString("(Temp)isMineralsOn" + stage, isMineralsOn.ToString());

        database.CheckIsChestOn(); // 상자 정보 저장
        StringBuilder isChestsOn = new StringBuilder();
        for (int i = 0; i < database.isChestsOn[stage].Length; i++)
        {
            isChestsOn.Append(database.isChestsOn[stage][i]);
            if (i < database.isChestsOn[stage].Length - 1)
                isChestsOn.Append(",");
        }
        PlayerPrefs.SetString("(Temp)isChestsOn" + stage, isChestsOn.ToString());

        database.CheckIsRockOn(); // 바위 정보 저장
        StringBuilder isRocksOn = new StringBuilder();
        for (int i = 0; i < database.isRocksOn[stage].Length; i++)
        {
            isRocksOn.Append(database.isRocksOn[stage][i]);
            if (i < database.isRocksOn[stage].Length - 1)
                isRocksOn.Append(",");
        }
        PlayerPrefs.SetString("(Temp)isRocksOn" + stage, isRocksOn.ToString());

        database.CheckIsMonsterOn(); // 몹 정보 저장
        StringBuilder isMonstersOn = new StringBuilder();
        for (int i = 0; i < database.isMonstersOn[stage].Length; i++)
        {
            isMonstersOn.Append(database.isMonstersOn[stage][i]);
            if (i < database.isMonstersOn[stage].Length - 1)
                isMonstersOn.Append(",");
        }
        PlayerPrefs.SetString("(Temp)isMonstersOn" + stage, isMonstersOn.ToString());

        PlayerPrefs.SetInt("(Temp)monsterNum" + stage, database.monstersPosition[stage].Count);
        int index = 0;
        foreach (var monsterPos in database.monstersPosition[stage])
        {
            PlayerPrefs.SetInt("(Temp)monster" + index + stage, monsterPos.Key);
            PlayerPrefs.SetFloat("(Temp)monster" + monsterPos.Key + "PosX" + stage, monsterPos.Value.x);
            PlayerPrefs.SetFloat("(Temp)monster" + monsterPos.Key + "PosY" + stage, monsterPos.Value.y);
            PlayerPrefs.SetFloat("(Temp)monster" + monsterPos.Key + "PosZ" + stage, monsterPos.Value.z);
            index++;
        }
    }

    public void LoadTemporaryMapData(int nextStage) // 스테이지 이동할때 저장했던 맵 데이타 로드
    {
        if(!PlayerPrefs.HasKey("(Temp)isGroundsOn" + nextStage))
        {
            Debug.LogError("임시 데이터가 없어서 로드를 못함");
                return;
        }
        string[] isGroundsOn = PlayerPrefs.GetString("(Temp)isGroundsOn" + nextStage).Split(','); // 땅 정보 불러오기
        database.isGroundsOn[nextStage] = new bool[isGroundsOn.Length];
        for (int i = 0; i < isGroundsOn.Length; i++)
            database.isGroundsOn[nextStage][i] = Convert.ToBoolean(isGroundsOn[i]);

        string[] isMineralsOn = PlayerPrefs.GetString("(Temp)isMineralsOn" + nextStage).Split(','); // 광물 정보 불러오기
        database.isMineralsOn[nextStage] = new bool[isMineralsOn.Length];
        for (int i = 0; i < isMineralsOn.Length; i++)
            database.isMineralsOn[nextStage][i] = Convert.ToBoolean(isMineralsOn[i]);

        string[] isChestsOn = PlayerPrefs.GetString("(Temp)isChestsOn" + nextStage).Split(','); // 상자 정보 불러오기
        database.isChestsOn[nextStage] = new bool[isChestsOn.Length];
        for (int i = 0; i < isChestsOn.Length; i++)
            database.isChestsOn[nextStage][i] = Convert.ToBoolean(isChestsOn[i]);

        string[] isRocksOn = PlayerPrefs.GetString("(Temp)isRocksOn" + nextStage).Split(','); // 바위 정보 불러오기
        database.isRocksOn[nextStage] = new bool[isRocksOn.Length];
        for (int i = 0; i < isRocksOn.Length; i++)
            database.isRocksOn[nextStage][i] = Convert.ToBoolean(isRocksOn[i]);

        string[] isMonstersOn = PlayerPrefs.GetString("(Temp)isMonstersOn" + nextStage).Split(','); // 바위 정보 불러오기
        database.isMonstersOn[nextStage] = new bool[isMonstersOn.Length];
        for (int i = 0; i < isMonstersOn.Length; i++)
            database.isMonstersOn[nextStage][i] = Convert.ToBoolean(isMonstersOn[i]);
        database.monstersPosition[nextStage].Clear();
        for (int i = 0; i < PlayerPrefs.GetInt("(Temp)monsterNum" + nextStage); i++)
        {
            int monsterIndex = PlayerPrefs.GetInt("(Temp)monster" + i + nextStage);
            database.monstersPosition[nextStage][monsterIndex] = new Vector3(PlayerPrefs.GetFloat("(Temp)monster" + monsterIndex + "PosX" + nextStage), PlayerPrefs.GetFloat("(Temp)monster" + monsterIndex + "PosY" + nextStage), PlayerPrefs.GetFloat("(Temp)monster" + monsterIndex + "PosZ" + nextStage));
        }
    }

    public void RemoveTemporaryData()
    {
        for (int i = 1; i <= stageNum; i++)
        {
            if (PlayerPrefs.HasKey("(Temp)isMineralsOn" + i))
            {
                PlayerPrefs.DeleteKey("(Temp)isMineralsOn" + i);
                Debug.Log("지워짐");
            }
        }
    }

    private void OnDisable()
    {
        RemoveTemporaryData();
    }
}
