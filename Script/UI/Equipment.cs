
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Equipment : MonoBehaviour
{
    private AudioManager theAudio;
    private PlayerStat playerStat;
    private Inventory theInven;
    
    public string key_sound;
    public string enter_sound;
    public string opne_sound;
    public string close_sound;
    public string takeoff_sound;
    public string equip_sound;

    const int DRILL = 0, CAP = 1, SHOES = 2, ACCESSORY = 3;
    public int added_atk, added_def, added_cri, added_oxy, added_explosivePower;
    public float added_atkSpeed, added_mom , added_speedX, added_skilling;

    public Image[] img_slots; // 장비 아이콘들
    public Item[] equipItemList; // 장착된 장비 리스트.
    bool[] isEquip; // 장착중인가
    private int selectedSlot; // 선택된 장비 슬롯

    public Image[] selectedEquip; // 선택된 장비 뿌옇게하는거

    // Start is called before the first frame update
    void Start()
    {
        theAudio = FindObjectOfType<AudioManager>();
        theInven = FindObjectOfType<Inventory>();
        playerStat = FindObjectOfType<PlayerStat>();
        isEquip = new bool[4];
    }

    public void ClearEquip() // 장비템 이미지 초기화
    {
        Color color = img_slots[0].color;
        color.a = 0f;

        for (int i = 0; i < img_slots.Length; i++)
        {
            img_slots[i].sprite = null;
            img_slots[i].color = color;
        }
    }

    public void RemoveAllEquipItem()
    {
        for (int i = 0; i < equipItemList.Length; i++)
        {
            TakeOffEffect(equipItemList[i]);
            equipItemList[i] = new Item();
        }
        ClearEquip();
    }

    public void EquipItem(Item _item) // 장비템 장착
    {
        string temp = _item.itemID.ToString();
        temp = temp.Substring(0, 3); // 문자열의 0~2번쨰 인덱스 가져옴
        
        switch (temp)
        {
            case "201": // 드릴
                EquipItemCheck(DRILL, _item);
                break;
            case "202": // 모자
                EquipItemCheck(CAP, _item);
                break;
            case "203": // 신발
                EquipItemCheck(SHOES, _item);
                break;
            case "204": // 장신구
                EquipItemCheck(ACCESSORY, _item);
                break;
        }
    }

    public void EquipItemCheck(int _count, Item _item) 
    {
        if(!isEquip[_count])
        {
            equipItemList[_count] = _item;
        }
        else
        {
            theInven.EquipToInventory(equipItemList[_count]); // 이미 템이 있으면 입던템 인벤토리로
            equipItemList[_count] = _item;
        }
        isEquip[_count] = true;
        EquipEffect(_item);
        theAudio.Play(equip_sound);
        ClearEquip();
        ShowEquip();
    }

    public void ShowEquip() // 장비템 보여주기
    {
        Color color = img_slots[0].color;
        color.a = 1f;

        for (int i = 0; i < img_slots.Length; i++)
        {
            if(equipItemList[i].itemID != 0)
            {
                img_slots[i].sprite = equipItemList[i].itemIcon;
                img_slots[i].color = color;
            }
        }
    }

    private void EquipEffect(Item _item) // 아이템 장착시 능력치 오르기
    {
        playerStat.atk += _item.atk;
        playerStat.def += _item.def;
        playerStat.maxOxygen += _item.oxy;
        playerStat.cri += _item.cri;
        playerStat.atkSpeed += _item.atkSpeed;
        playerStat.momentum += _item.mom;
        playerStat.speedX += _item.speedX;
        playerStat.skilling += _item.skilling;
        playerStat.explosivePower += _item.power;

        added_atk += _item.atk;
        added_def += _item.def;
        added_cri += _item.cri;
        added_atkSpeed += _item.atkSpeed;
        added_oxy += _item.oxy;
        added_mom += _item.mom;
        added_speedX += _item.speedX;
        added_skilling += _item.skilling;
        added_explosivePower += _item.power;
    }

    private void TakeOffEffect(Item _item) // 아이템 헤제시 능력치 감소
    {
        playerStat.atk -= _item.atk;
        playerStat.def -= _item.def;
        playerStat.maxOxygen -= _item.oxy;
        playerStat.cri -= _item.cri;
        playerStat.atkSpeed -= _item.atkSpeed;
        playerStat.momentum -= _item.mom;
        playerStat.speedX -= _item.speedX;
        playerStat.skilling -= _item.skilling;
        playerStat.explosivePower -= _item.power;

        added_atk -= _item.atk;
        added_def -= _item.def;
        added_cri -= _item.cri;
        added_atkSpeed -= _item.atkSpeed;
        added_oxy -= _item.oxy;
        added_mom -= _item.mom;
        added_speedX -= _item.speedX;
        added_skilling -= _item.skilling;
        added_explosivePower -= _item.power;
    }

    void SelectedEffectOff() // 선택된 템 뿌옇게하는거 초기화
    {
        for (int i = 0; i < selectedEquip.Length; i++)
        {
            Color color = selectedEquip[i].color;
            color.a = 0f;
            selectedEquip[i].color = color;
        }
        GameManager.gameMode = GameManager.GameMode.OpenInventory;
    }

    public void EquippedItemButton(int _selectedSlot) // 장착된 템을 눌렀을떄
    {
        if (!isEquip[_selectedSlot])
            return;
        SelectedEffectOff();
        selectedSlot = _selectedSlot;
        Color color = selectedEquip[selectedSlot].color;
        color.a = 60.0f / 255f;
        selectedEquip[selectedSlot].color = color;
        InformationPanel.instance.EnableOOC(new Vector2(650, 400), "장착을 헤제하시겠습니까?", "헤제", "취소", TakeOffEquip, SelectedEffectOff);
    }

    void TakeOffEquip() // 장착해제
    {
        if (theInven.IsInventoryFull())
            Debug.LogError("템창꽉참");

        theAudio.Play(takeoff_sound);
        theInven.GetAnItem(equipItemList[selectedSlot].itemID, floatText: false);
        TakeOffEffect(equipItemList[selectedSlot]);
        equipItemList[selectedSlot] = new Item();
        isEquip[selectedSlot] = false;

        SelectedEffectOff();
        ClearEquip();
        ShowEquip();
        theInven.ShowItem();
        GameManager.gameMode = GameManager.GameMode.OpenInventory;
    }

    public bool IsEquipedDrill() // 드릴을 장착중인가
    {
        return equipItemList[DRILL].itemID > 0;
    }
}
