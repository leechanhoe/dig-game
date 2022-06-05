using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IllustratedBook : MonoBehaviour
{
    public GameObject illustratedBookUI;
    public Slider discoveryRate;
    public Text discoveryRateText;
    public Text itemName;
    public Text itemDescription;
    public Text mineralDistribution;

    [SerializeField] GameObject rewardActive;
    [SerializeField] string rewardSound;
    IllustratedSlot[] illustratedSlots;
    DatabaseManager database;
    public IllustratedSlot selectedSlot { get; set; }
    // Start is called before the first frame update
    private void Start()
    {
        database = FindObjectOfType<DatabaseManager>();
    }

    private void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (GameManager.gameMode == GameManager.GameMode.OpenIllustratedBook && Input.GetKey(KeyCode.Escape) && !DatabaseManager.preventDoubleClick)
            {
                StartCoroutine(PreventDoubleClickC());
            }
        }
    }
    IEnumerator PreventDoubleClickC()
    {
        DatabaseManager.preventDoubleClick = true;
        CloseIllustratedBook();
        yield return new WaitForSeconds(0.1f);
        DatabaseManager.preventDoubleClick = false;
    }


    public void OpenIllustratedBook()
    {
        illustratedBookUI.SetActive(true);
        if(illustratedSlots == null)
            illustratedSlots = FindObjectsOfType<IllustratedSlot>();

        GameManager.gameMode = GameManager.GameMode.OpenIllustratedBook;
        SubMenu.instance.MenuClose();
        Minimap.MinimapOff();
        ShowIllustratedSlot();
        InitializeSelected();
    }

    public void CloseIllustratedBook()
    {
        ShowIllustratedSlot();
        InitializeSelected();
        illustratedBookUI.SetActive(false);

        GameManager.gameMode = GameManager.GameMode.Normal;
        Minimap.MinimapOn();
    }

    public void ShowIllustratedSlot()
    {
        for (int i = 0; i < illustratedSlots.Length; i++)
        {
            illustratedSlots[i].databaseManager = database;
            illustratedSlots[i].UpdateSlot();
        }
    }

    void InitializeSelected()
    {
        for (int i = 0; i < illustratedSlots.Length; i++)
            illustratedSlots[i].InitializeSelected();
        discoveryRate.value = 0;
        discoveryRateText.text = "";
        itemName.text = "";
        itemDescription.text = "";
        mineralDistribution.text = "";
    }
        public void TouchSlot()
    {
        InitializeSelected();
        selectedSlot.selected.SetActive(true);
        if (selectedSlot.discoveryRate == 0)
        {
            discoveryRate.value = 0;
            discoveryRateText.text = "??%";
            itemName.text = "??";
            itemDescription.text = "";
            mineralDistribution.text = "분포 : ??";
            rewardActive.SetActive(true);
        }
        else
        {
            discoveryRate.value = selectedSlot.discoveryRate;
            discoveryRateText.text = (int)(selectedSlot.discoveryRate * 100) + "%";
            itemName.text = selectedSlot.item.itemName;
            itemDescription.text = selectedSlot.item.itemDescription;
            mineralDistribution.text = "분포 : " + selectedSlot.item.mineralDistribution;
            if(selectedSlot.CanGetReward())
                rewardActive.SetActive(false);
            else
                rewardActive.SetActive(true);
        }
    }

    public void TouchRewardButton()
    {
        selectedSlot.GetUpgradeReward();
        FindObjectOfType<PlayerStat>().MineralGradeEffectOn();
        AudioManager.instance.Play(rewardSound);
        InformationPanel.instance.EnableOK(Vector2.zero, "보상이 적용되었습니다.", "확인");
        rewardActive.SetActive(true);
    }

    public void TouchQuestionButton()
    {
        InformationPanel.instance.EnableOK(new Vector2(850,550), " 1성보상 : 깨진 슬롯 증가 교환권\n 2성보상 : 해당 광물의 인벤토리 한 칸당 최대소지개수가 하나 증가\n 3성보상 : 해당 광물의 판매가, 경험치 1.5배 증가\n 마지막보상 : 상점에서 해당 광물 구매가능, ??", "확인");
    }
}
