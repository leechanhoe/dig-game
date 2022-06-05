using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SellPanels : MonoBehaviour
{
    Inventory inventory;
    // Start is called before the first frame update
    void Start()
    {
        inventory = FindObjectOfType<Inventory>();
    }

    private void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (GameManager.gameMode == GameManager.GameMode.OpenInformationPanel && Input.GetKey(KeyCode.Escape) && !DatabaseManager.preventDoubleClick)
            {
                StartCoroutine(PreventDoubleClickC());
            }
        }
    }

    IEnumerator PreventDoubleClickC()
    {
        DatabaseManager.preventDoubleClick = true;
        if (sellAllPanel.activeSelf)
            CloseSellAllPanel();
        else if (sellPanel.activeSelf)
            CloseSellPanel();
        yield return new WaitForSeconds(0.1f);
        DatabaseManager.preventDoubleClick = false;
    }

    public GameObject sellAllPanel;
    public Text sumOfPrice;
    public int price; // 결국 결정된 판매 가격
    [SerializeField] GameObject preventOtherTouch;

    //모두 팔기
    #region 
    public void SellAllPanel()
    {
        GameManager.gameMode = GameManager.GameMode.OpenInformationPanel;
        sellAllPanel.SetActive(true);
        preventOtherTouch.SetActive(true);
        sumOfPrice.text = inventory.CalculateSumOfPrice().ToString();
    }

    public void ExecuteSellAll()
    {
        inventory.SellAllMineral();
        CloseSellAllPanel();
    }

    public void CloseSellAllPanel()
    {
        GameManager.gameMode = GameManager.GameMode.OpenInventory;
        sumOfPrice.text = "";
        sellAllPanel.SetActive(false);
        preventOtherTouch.SetActive(false);
    }
    #endregion

    public GameObject sellPanel;
    public Text itemCountText;
    public Text informationText;
    public Text priceText;
    public GameObject itemCountPanel; // 광물팔때 개수 조정하는 ui
    int maxItemCount; // 선택된 템의 최대 팔수있는 개수
    int selectedCount; // 팔기로 결정한 개수

    public void SellPanel()
    {
        GameManager.gameMode = GameManager.GameMode.OpenInformationPanel;
        sellPanel.SetActive(true);
        preventOtherTouch.SetActive(true);
        maxItemCount = inventory.selectedSlot.itemCount;
        selectedCount = maxItemCount;

        itemCountText.text = selectedCount.ToString();
        priceText.text = (inventory.selectedItem.itemSalePrice * selectedCount).ToString();

        if (inventory.selectedItem.itemType == Item.ItemType.Mineral)
        {
            sellPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(550, 700);
            informationText.text = "몇 개를 파시겠습니까?";
            itemCountPanel.SetActive(true);
        }
        else if(inventory.selectedItem.itemType == Item.ItemType.Use || inventory.selectedItem.itemType == Item.ItemType.Equip)
        {
            sellPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(550, 500);
            informationText.text = "정말 파시겠습니까?";
            itemCountPanel.SetActive(false);
        }
    }

    public void ModulateItemCount(bool up) // 개수 조절 화살표를 눌렀을 때
    {
        if (up)
        {
            selectedCount += 1;
            if (selectedCount > maxItemCount)
                selectedCount = maxItemCount;
        }
        else
        {
            selectedCount -= 1;
            if (selectedCount < 1)
                selectedCount = 1;
        }
        itemCountText.text = selectedCount.ToString();
        priceText.text = (inventory.selectedItem.itemSalePrice * selectedCount).ToString();
    }

    public void ExecuteSell()
    {
        inventory.SellItem(selectedCount);
        CloseSellPanel();
    }

    public void CloseSellPanel()
    {
        GameManager.gameMode = GameManager.GameMode.OpenInventory;
        itemCountText.text = "";
        informationText.text = "";
        priceText.text = "";
        selectedCount = 0;
        maxItemCount = 0;
        sellPanel.SetActive(false);
        preventOtherTouch.SetActive(false);
    }
}
