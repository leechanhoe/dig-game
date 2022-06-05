using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


public class InventorySlot : MonoBehaviour
{
    public Item item;
    public Image icon;
    public int itemCount;
    public Text itemCount_Text;
    public GameObject selected_Item;
    public GameObject TouchButton;
    static public int activeSlots;
    static public int maxSlots;
    public bool isFull;
    InventorySlot[] slots;

    Inventory inventory;
    PlayerStat playerStat;

    private void Awake()
    {
        inventory = Inventory.instance;
        slots = FindObjectsOfType<InventorySlot>();
        playerStat = PlayerStat.instance;
    }

    public void Additem(Item _item, int _itemcount)
    {
        maxSlots = playerStat.maxItemSlot;
        itemCount = _itemcount;
        if (_itemcount == 0)
            _itemcount = _item.itemCount;
        if (activeSlots == maxSlots)
        {
            Debug.Log("템창꽉참");
            return;
        }

        activeSlots++;
        item = _item;
        icon.sprite = _item.itemIcon;
        icon.color = new Color(255f, 255f, 255f, 255f);
        if(item.maxItemCount > 1)
        {
            if (_item.itemCount > 0)
                itemCount_Text.text = _itemcount.ToString();
            else
                itemCount_Text.text = "";
        }
    }

    public void RemoveItem()
    {
        item = new Item();
        activeSlots--;
        itemCount_Text.text = "";
        icon.color = new Color(255f, 255f, 255f, 0f);
        icon.sprite = null;
    }

    public void TouchItem() // 아이템 아이콘 터치시 
    {
        inventory.selectedSlot = this;
        for (int i = 0; i < slots.Length; i++)
            slots[i].selected_Item.SetActive(false); // 일단 선택된 탭 흐려지는거 초기화
        selected_Item.SetActive(true); // 터치된 애만 흐려지는 효과
        inventory.selectedItem = item;
        inventory.TouchItem2();
    }
}
