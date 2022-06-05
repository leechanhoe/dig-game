using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IllustratedSlot : MonoBehaviour
{
    public int itemID;
    public Item item { get; set;}
    public Image gradeImage;
    public int grade;
    public Image itemIcon;
    public GameObject selected; // 선택됐을때 하얘지는거
    public Sprite questionMark;
    public Sprite[] gradeSprites;
    public float discoveryRate { get; set; }
    public bool[] upgradeReward { get; set; } = new bool[5];

    public DatabaseManager databaseManager { get; set; }
    IllustratedBook illustratedBook;
    // Start is called before the first frame update

    private void Awake()
    {
        illustratedBook = GetComponentInParent<IllustratedBook>();
        databaseManager = DatabaseManager.instance;
        for (int i = 0; i < databaseManager.itemList.Count; i++)
        {
            if (databaseManager.itemList[i].itemID == itemID)
                item = databaseManager.itemList[i];
        }
    }

    private void UpdateGrade()
    {
        if(databaseManager.totalMineralCount[itemID] != 0)
            discoveryRate = (float)databaseManager.getMineralCount[itemID] / databaseManager.totalMineralCount[itemID];
        if (0 <= discoveryRate && discoveryRate < 0.25f)
            grade = 0;
        else if (0.25 <= discoveryRate && discoveryRate < 0.5f)
            grade = 1;
        else if (0.5 <= discoveryRate && discoveryRate < 0.75f)
            grade = 2;
        else if (0.75 <= discoveryRate && discoveryRate < 1)
            grade = 3;
        else if (discoveryRate == 1)
            grade = 4;
        gradeImage.sprite = gradeSprites[grade];
    }

    public void UpdateSlot()
    {
        if (databaseManager.getMineralCount[itemID] == 0)
            itemIcon.sprite = questionMark;
        else
            itemIcon.sprite = item.itemIcon;
        UpdateGrade();
    }

    public bool getGrade1Reward;

    public bool CanGetReward()
    {
        if (!databaseManager.isGetUpgradeReward[item.itemID][grade] && grade > 0)
        {
            return true;
        }
        return false;
    }
    public void GetUpgradeReward() // 보상받기
    {
        if (!databaseManager.isGetUpgradeReward[item.itemID][grade])
        {
            PlayerStat playerStat = FindObjectOfType<PlayerStat>();
            if(grade >= 1)
            {
                if (!playerStat.grade1Mineral.Contains(item.itemID))
                {
                    Inventory.instance.GetAnItem(30002, floatText: false, soundEffect: false);
                    playerStat.AddGrade1Mineral(item.itemID);
                    databaseManager.isGetUpgradeReward[item.itemID][1] = true;
                }
                if(grade >= 2)
                {
                    playerStat.AddGrade2Mineral(item.itemID);
                    databaseManager.isGetUpgradeReward[item.itemID][2] = true;
                    if (grade >= 3)
                    {
                        playerStat.AddGrade3Mineral(item.itemID);
                        databaseManager.isGetUpgradeReward[item.itemID][3] = true;
                        if (grade >= 4)
                        {
                            playerStat.AddGrade4Mineral(item.itemID);
                            databaseManager.isGetUpgradeReward[item.itemID][4] = true;
                        }
                    }
                }
            }
        }
    }

    public void TouchSlot()
    { 
        illustratedBook.selectedSlot = this;
        illustratedBook.TouchSlot();
    }

    public void InitializeSelected()
    {
        selected.SetActive(false);
    }
}
