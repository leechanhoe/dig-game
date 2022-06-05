using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    static public QuestManager instance;
    public bool isProgress; // 퀘스트를 진행중인가
    public int questIndex; // 현재 메인 퀘스트 진행도 (100이면 퀘스트 아이디 100짜리 퀘를 받을 수 있는 상태)
    public bool isProgressHidden; // 히든퀘스트를 진행중인가
    public int hiddenQuestIndex;
    Dictionary<int, Quest> questData = new Dictionary<int, Quest>();
    public Dictionary<int, List<GameObject>>[] questObject { get; set; } = new Dictionary<int, List<GameObject>>[6]; // questObject[스테이지][퀘스트 인덱스] = 조정할 오브젝트 리스트

    ObData[] npcs;

    const int MAIN = 0, HIDDEN = 1;
    int selectedTab = MAIN;

    [SerializeField] GameObject questUI;
    [SerializeField] Text questName;
    [SerializeField] Text questContent;
    [SerializeField] GameObject[] selectedEffect;

    DatabaseManager database;
    Inventory inventory;
    PlayerStat playerStat;
    Equipment equipment;

    private void Awake()
    {
        if (instance == null) // 파괴방지
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
        else
            Destroy(this.gameObject);
        for (int i = 1; i <= 5; i++)
            questObject[i] = new Dictionary<int, List<GameObject>>();
    }

    void Start()
    {
        database = FindObjectOfType<DatabaseManager>();
        inventory = FindObjectOfType<Inventory>();
        playerStat = FindObjectOfType<PlayerStat>();
        equipment = FindObjectOfType<Equipment>();
        GenerateData();
    }

    public void LoadNpc()
    {
        npcs = FindObjectsOfType<ObData>();
    }

    void GenerateData()
    {
        questData.Add(100, new Quest(100, "드릴을 구매해보자", 10000, 10000, 100, new Dictionary<int, int>() { { 20101, 1 } }, new int[] { 20101 }));
        questData.Add(200, new Quest(200, "드릴을 장착해보자", 10000, 10000, 100, rewardItem: new int[1] { 30103 }));
        questData.Add(300, new Quest(300, "몬스터를 잡아보자", 10000, 10000, 100, new Dictionary<int, int>() { { 30101, 1 } }, rewardMoney: 3000));
        questData.Add(400, new Quest(400, "바위를 폭파해보자", 10000, 10000, 100, new Dictionary<int, int>() { { 30104, 1 } }, new int[] { 10102, 10102 }));
        questData.Add(500, new Quest(500, "아이템을 제조해보자", 10000, 10000, 100, new Dictionary<int, int>() { { 20401, 1 } }, rewardMoney: 2000));
        questData.Add(600, new Quest(600, "동굴에 들어간 친구를 찾아보자", 10000, 20000, 100, new Dictionary<int, int>() { { 40005, 20 } }, new int[] { 10201 }));
        questData.Add(700, new Quest(700, "시험", 10000, 10000, 100, new Dictionary<int, int>() { { 30105, 1 } }, rewardMoney:10000));
        questData.Add(800, new Quest(800, "오염된 땅", 10000, 10000, 100, new Dictionary<int, int>() { { 30106, 1 } }, rewardMoney: 10000));
        questData.Add(900, new Quest(900, "깨끗한 땅을 찾아서", 10000, 40000, 100, rewardMoney: 10000));
        questData.Add(1000, new Quest(1000, "불의 원석", 10000, 10000, 100, new Dictionary<int, int>() { { 30108, 1 } }, rewardMoney: 10000));
        questData.Add(1100, new Quest(1100, "정화의 돌", 50000, 50000, 100, rewardMoney: 10000));

        //questData.Add(5100, new Quest(5100, "히든 퀘스트", 110000, 110000, new Dictionary<int, int>() { { 40003, 2 } }, new int[] { 10003 },4444,true));
        //questData.Add(6100, new Quest(6100, "히든 퀘스트2", 120000, 120000, new Dictionary<int, int>() { { 40003, 2 } }, new int[] { 10003 }, 5555, true));

    }

    public void AddQuestObjInfo(int index, GameObject obj)
    {
        if (!questObject[playerStat.stage].ContainsKey(index))
            questObject[playerStat.stage][index] = new List<GameObject>();
        questObject[playerStat.stage][index].Add(obj);
    }

    public int CheckHiddenQuest(int npcID)
    {
        foreach (KeyValuePair<int,Quest> quest in questData)
        {
            if(!questData[quest.Key].isComplete && questData[quest.Key].isHiddenQuest && questData[quest.Key].startNpc == npcID)
            {
                if (!isProgressHidden) // 퀘스트를 맨 처음 받을떄
                {
                    hiddenQuestIndex = quest.Key;
                    return hiddenQuestIndex;
                }
                if (questData[hiddenQuestIndex].startNpc != npcID) // 다른 히든퀘스트를 진행중인데 또 다른 히든퀘스트 주는애를 만났을때 거절하기 위함
                    return quest.Key;

                if (npcID == questData[hiddenQuestIndex].endNpc) // 퀘스트를 받는 애랑 완료하는애가 같을때
                {
                    return isConditionFilled(true) ? hiddenQuestIndex + 3 : hiddenQuestIndex + 2;
                } // 퀘스트 받는애랑 완료하는애 다를떄
                else
                {
                    if (isProgressHidden)
                        return hiddenQuestIndex + 1;
                }
            }
            if (isProgressHidden && npcID == questData[hiddenQuestIndex].endNpc) // 퀘스트 완료하는 애일떄
                return isConditionFilled(true) ? hiddenQuestIndex + 3 : hiddenQuestIndex + 2;
        
        }
        return 0;
    }

    public int CheckQuest(int npcID) // 이 엔피씨가 지금 진행할 수 있는 퀘스트를 가지고 있나
    {
        if (npcID == questData[questIndex].startNpc) // 퀘스트 주는애일떄
        {
            if (!isProgress) // 퀘스트를 맨 처음 받을떄
                return questIndex;

            if (npcID == questData[questIndex].endNpc) // 퀘스트를 받는 애랑 완료하는애가 같을때
            {
                    return isConditionFilled() ? questIndex + 3 : questIndex + 2;
            } // 퀘스트 받는애랑 완료하는애 다를떄
            else
            {
                if (isProgress)
                    return questIndex + 1;
            }
        }
        if(npcID == questData[questIndex].endNpc) // 퀘스트 완료하는 애일떄
            return isConditionFilled() ? questIndex + 3 : questIndex + 2;

        return 0;
    }

    public bool isConditionFilled(bool isHiddenQuest = false) // 퀘스트 조건을 완료했나?
    {
        if (!isHiddenQuest)
        {
            if (questData[questIndex].completionCondition != null)
            {
                foreach (KeyValuePair<int, int> condition in questData[questIndex].completionCondition)
                {
                    if (database.ReturnItem(condition.Key).itemCount < condition.Value)
                        return false;
                }
            }
        }
        else
        {
            if (questData[hiddenQuestIndex].completionCondition != null)
            {
                foreach (KeyValuePair<int, int> condition in questData[hiddenQuestIndex].completionCondition)
                {
                    if (database.ReturnItem(condition.Key).itemCount < condition.Value)
                        return false;
                }
            }
        }

        switch (questIndex)
        {
            case 200:
                if (!equipment.IsEquipedDrill())
                    return false;
                break;
        }
        return true;
    }

    public bool QuestComplete(bool isHiddenQuest = false) // 퀘스트 완료
    {
        int questIndex;
        if (isHiddenQuest)
            questIndex = hiddenQuestIndex;
        else
            questIndex = this.questIndex;

        if (questData[questIndex].completionCondition != null)
        {
            foreach (KeyValuePair<int, int> condition in questData[questIndex].completionCondition) // 퀘스트 재료 제거
                inventory.RemoveItem(condition.Key, condition.Value);
        }

        string text = "획득 ▼ \n";
        if (questData[questIndex].rewardItem != null)
        {
            for (int i = 0; i < questData[questIndex].rewardItem.Length; i++) // 퀘스트 보상받기
            {
                if (!inventory.GetAnItem(questData[questIndex].rewardItem[i], floatText: false))
                {
                    inventory.ShowInvenFullPanel();
                    return false;
                }
                text += " ● " + database.ReturnItem(questData[questIndex].rewardItem[i]).itemName + "\n";
            }
        }

        if (questData[questIndex].rewardMoney > 0) // 보상 골드
        {
            text += " ● " + questData[questIndex].rewardMoney + " 골드";
            playerStat.money += questData[questIndex].rewardMoney;
        }
        InformationPanel.instance.EnableOK(new Vector2(650,500),text,"확인");

        text += " ● " + questData[questIndex].rewardExp + " 경험치";
        playerStat.GetExp(questData[questIndex].rewardExp);
        questData[questIndex].isComplete = true;

        QuestObjectControl(questIndex);
        if (!isHiddenQuest)
        {
            this.questIndex += 100;
            isProgress = false;
        }
        else
        {
            isProgressHidden = false;
            hiddenQuestIndex = 0;
        }
        NpcExclamationControl();
        return true;
    }

    public void QuestObjectControl(int index,bool isHiddenQuest = false)
    {
        if (!isHiddenQuest)
        {
            for (int j = index; j > 0; j -= 100)
            {
                if (questData[j].isComplete && questObject[playerStat.stage].ContainsKey(j))
                {
                    for (int i = 0; i < questObject[playerStat.stage][index].Count; i++)
                    {
                        if (questObject[playerStat.stage][index][i].CompareTag("Portal"))
                            questObject[playerStat.stage][index][i].SetActive(true);
                        else if (questObject[playerStat.stage][index][i].CompareTag("Npc"))
                            questObject[playerStat.stage][index][i].SetActive(false);
                    }
                }
            }
        }
    }

    public void QuestStart(bool isHiddenQuest = false)
    {
        if (isHiddenQuest)
        {
            if (isProgressHidden)
            {
                InformationPanel.instance.EnableOK(Vector2.zero, "히든퀘스트는 동시에 한개만 진행가능합니다.", "확인");
                return;
            }
            isProgressHidden = true;
            InformationPanel.instance.EnableDownInfoPanel("'" + questData[hiddenQuestIndex] + "' 퀘스트를 수락했습니다.");
        }
        else
        {
            isProgress = true;
            InformationPanel.instance.EnableDownInfoPanel("'" + questData[questIndex] + "' 퀘스트를 수락했습니다.");
        }
        NpcExclamationControl();
    }

    public void GiveUpQuest()
    {
        isProgressHidden = false;
        hiddenQuestIndex = 0;
        InformationPanel.instance.EnableDownInfoPanel("히든퀘스트를 포기했습니다.");
    }

    public void InitializeQuestData()
    {
        questIndex = 0;
        isProgress = false;
        hiddenQuestIndex = 0;
        isProgressHidden = false;
    }

    GameManager.GameMode prevGameMode;

    public void OpenQuestUI()
    {
        questUI.SetActive(true);
        prevGameMode = GameManager.gameMode;
        GameManager.gameMode = GameManager.GameMode.OpenQuestUI;
        TouchTab(selectedTab);
    }

    public void TouchTab(int type)
    {
        if(type == MAIN)
        {
            if (!isProgress)
            {
                questName.text = "진행중인 퀘스트가 없음";
                questContent.text = "";
            }
            else
            {
                questName.text = questData[questIndex].questName;
                string content = "";
                for (int i = 0; i < TalkManager.instance.talkData[questData[questIndex].startNpc + questIndex].Length; i++)
                    content += TalkManager.instance.talkData[questData[questIndex].startNpc + questIndex][i] + "\n";

                questContent.text = content;
            }
            selectedEffect[MAIN].SetActive(true);
            selectedEffect[HIDDEN].SetActive(false);
        }
        else if(type == HIDDEN)
        {
            if(!isProgressHidden)
            {
                questName.text = "진행중인 퀘스트가 없음";
                questContent.text = "";
            }
            else
            {
                questName.text = questData[hiddenQuestIndex].questName;
                string content = "";
                for (int i = 0; i < TalkManager.instance.talkData[questData[hiddenQuestIndex].startNpc + hiddenQuestIndex].Length; i++)
                    content += TalkManager.instance.talkData[questData[hiddenQuestIndex].startNpc + hiddenQuestIndex][i] + "\n";

                questContent.text = content;
            }
            selectedEffect[HIDDEN].SetActive(true);
            selectedEffect[MAIN].SetActive(false);
        }
    }

    public void CloseQuestUI()
    {
        questUI.SetActive(false);
        GameManager.gameMode = prevGameMode;
    }

    public void NpcExclamationControl() // 느낌표 마크가 필요한 엔피씨 적용
    {
        for (int i = 0; i < npcs.Length; i++)
        {
            npcs[i].ExclamationMarkOff();

            if (questData.ContainsKey(questIndex))
            {
                if (questData[questIndex].startNpc.Equals(npcs[i].id) && !isProgress)
                {
                    npcs[i].ExclamationMarkOn();
                    continue;
                }
            }

            if (questData.ContainsKey(hiddenQuestIndex))
            {
                if (isProgressHidden)
                {
                    if (questData[hiddenQuestIndex].startNpc.Equals(npcs[i].id))
                        continue;
                    else
                        npcs[i].ExclamationMarkOn();
                }
            }
            else
            {
                foreach (KeyValuePair<int, Quest> quest in questData)
                {
                    if (quest.Value.isHiddenQuest && !quest.Value.isComplete && quest.Value.startNpc.Equals(npcs[i].id))
                    {
                        npcs[i].ExclamationMarkOn();
                        break;
                    }
                }
            }
        }
    }
}
