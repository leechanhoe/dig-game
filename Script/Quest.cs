using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Quest
{
    public int questID { get; }
    public string questName { get; }
    public bool isHiddenQuest { get; }
    public int startNpc { get; } // 퀘스트 시작 엔피씨
    public int endNpc { get; } // 퀘스트 종료 엔피씨
    public Dictionary<int, int> completionCondition { get; } // 퀘스트 완료 조건 <아이템id,아이템개수>
    public bool isComplete { get; set; } // 퀘스트를 완료했나
    public int[] rewardItem;
    public int rewardMoney;
    public int rewardExp;

    public Quest(int questID, string questName, int startNpc, int endNpc, int rewardExp, Dictionary<int, int> completionCondition = null, int[] rewardItem = null, int rewardMoney = 0, bool isHiddenQuest = false)
    {
        this.questID = questID;
        this.questName = questName;
        this.isHiddenQuest = isHiddenQuest;
        this.startNpc = startNpc;
        this.endNpc = endNpc;
        this.completionCondition = completionCondition;
        this.rewardItem = rewardItem;
        this.rewardMoney = rewardMoney;
        this.rewardExp = rewardExp;
        isComplete = false;
    }
}
