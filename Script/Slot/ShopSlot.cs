using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour
{
    public Item item;
    public Image icon;
    public Text itemDescription;
    public Text itemName;
    public Text itemPrice;
    public GameObject purchaseButton;
    public GameObject selectedItem;
    public string cancelSound;

    DatabaseManager database;
    public ShopSlot[] slots { get; set; }
    Inventory inventory;
    PlayerStat playerStat;
    // Start is called before the first frame update
    void Awake()
    {
        database = DatabaseManager.instance;
        inventory = Inventory.instance;
        playerStat = PlayerStat.instance;
    }

    public void AddItem(Item _item)
    {
        item = _item;
        icon.sprite = item.itemIcon;
        itemDescription.text = item.itemDescription;
        itemName.text = item.itemName;
        itemPrice.text = item.itemPurchasePrice.ToString();
    }
    public void AddItem(int _itemID)
    {
        for (int i = 0;i < database.itemList.Count;i++)
        {
            if (database.itemList[i].itemID == _itemID)
                item = database.itemList[i];
        };
        if (item == null)
            Debug.LogError("데이터베이스에 해당 아이템이 없습니다.");
        icon.sprite = item.itemIcon;
        if (item.itemID < 40000)
            itemDescription.text = item.itemDescription;
        else
            itemDescription.text = "";
        itemName.text = item.itemName;
        itemPrice.text = item.itemPurchasePrice.ToString();
    }

    public void TouchItem()
    {
        for (int i = 0; i < slots.Length; i++)
        {
                slots[i].selectedItem.SetActive(false);
        }
        selectedItem.SetActive(true);
        AudioManager.instance.Play("TouchButton");
    }

    public void PurchaseItem()
    {
        if (inventory.IsInventoryFull(item.itemID))
        {
            AudioManager.instance.Play(cancelSound);
            FindObjectOfType<InformationPanel>().EnableOK(new Vector2(650, 400), "인벤토리가 꽉 찼습니다.", "확인");
            return;
        }
        if (playerStat.money - item.itemPurchasePrice < 0)
        {
            Debug.Log("돈이없음");
            FindObjectOfType<InformationPanel>().EnableOK(new Vector2(650, 400), "소지금이 부족합니다.", "확인");
            return;
        }
        playerStat.money -= item.itemPurchasePrice;
        inventory.GetAnItem(item.itemID, floatText: false);
        inventory.ShowItem();
    }
}
