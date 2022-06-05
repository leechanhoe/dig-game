using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickSlot : MonoBehaviour
{
    public Item item;
    [SerializeField] Image icon;
    [SerializeField] Text count;
    public Image coolTimeImage;
    [SerializeField] GameObject useButton;
    float coolTime;
    public bool isOn { get; set; } // 아이템이 올려져있는가
     
    private void Update()
    {
        if (item == null)
            return;
        if (item.itemID.Equals(0))
            return;
        if (item.curCoolTime > 0)
            coolTimeImage.fillAmount = item.curCoolTime / item.coolTime;
        if (item.curCoolTime <= 0 && !useButton.activeSelf)
            useButton.SetActive(true);
    }

    public void AddItem(Item _item)
    {
        if (_item.itemCount <= 0)
            return;
        Color color = new Color(1, 1, 1);
        icon.color = color;
        item = _item;
        item.isOnQuickslot = true;
        icon.gameObject.SetActive(true);
        icon.sprite = item.itemIcon;
        isOn = true;
        count.text = item.itemCount.ToString();
    }

    public void RemoveItem()
    {
        if(item != null)
            item.isOnQuickslot = false;
        item = null;
        icon.sprite = null;
        icon.gameObject.SetActive(false);
        isOn = false;
        count.text = "";
    }

    public void UseItem()
    {
        useButton.SetActive(false);
    }

    public void ExhaustItem(Item _item) // 템 개수가 0이 되었을 떄
    {
        if (_item.itemCount > 0)
            return;

        Color color = new Color(160f / 255f, 160f / 255f, 160f / 255f);
        icon.color = color;
        item = _item;
        isOn = true;
        count.text = "0";
    }
}
