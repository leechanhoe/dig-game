using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SmithySlot : MonoBehaviour
{
    public Item item;
    public Image icon;
    public Text itemDescription;
    public Text itemName;
    public GameObject selected;

    public DatabaseManager database { get; set; }
    Smithy smithy;

    // Start is called before the first frame update
    void Start()
    {
        smithy = GetComponentInParent<Smithy>();
        database = DatabaseManager.instance;
    }

    public void AddItem(int _itemID)
    {
        selected.SetActive(false);
        for (int i = 0; i < database.itemList.Count; i++)
        {
            if (database.itemList[i].itemID == _itemID)
            {
                item = database.itemList[i];
                break;
            }
        };
        if (item == null)
            Debug.LogError("데이터베이스에 해당 아이템이 없습니다.");
        icon.sprite = item.itemIcon;
        itemDescription.text = item.itemDescription;
        itemName.text = item.itemName;
    }

    public void TouchSlot()
    {
        smithy.InitializeSelected();
        selected.SetActive(true);
        smithy.selectedSlot = this;
        smithy.RemoveButtonActive();
    }

    public void TouchProduceButton() // 템슬롯에서 제작버튼 눌렀을 때
    {
        smithy.selectedItem = item;
        smithy.OpenMethodUI();
    }
}
