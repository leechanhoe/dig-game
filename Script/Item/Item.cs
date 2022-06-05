using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class Item 
{
    public int itemID; // 템 고유 id값,중복불가
    public string itemName; // 템이름,중복가능
    public string itemDescription; // 템 설명
    public string mineralDistribution; // 광물 분포
    public int production; // 제작법으로 제작되는 아이템(제작법 한정) 
    public int[] productionMaterial; // 제작 재료
    public int[] productionMaterialCount; // 제작 재료 개수
    public int maxItemCount;// 최대 소지 개수
    public int itemCount; // 소지 개수
    public int storageItemCount; // 창고에 이 템 소지개수
    public int exp; // 아이템 획득시 경험치(광물한정)
    public Sprite itemIcon;
    public ItemType itemType;

    public int itemSalePrice; // 아이템 판매 가격
    public int itemPurchasePrice; // 아이템 구매 가격

    public bool isOnQuickslot;
    public float coolTime; // 템쿨타임
    public float curCoolTime; // 현재 쿨타임

    public enum ItemType // enum = 열거
    {
        Null,
        Use,
        Equip,
        Etc,
        Mineral,
        Quest
    }

    public int atk;
    public int def;
    public int oxy;
    public int cri;
    public float atkSpeed;
    public float mom;
    public float speedX;
    public float skilling;
    public int power; // 폭탄의 세기

    public Item()
    {
        itemID = 0;
        itemName = "";
        itemType = ItemType.Null;
        itemCount = 0;
        maxItemCount = 1;
    }

    public Item(int _itemID, string _itemName, string _itemDes, ItemType _itemType,
        int _maxItemCount = 1, int _itemSalePrice = 0, int _itemPurchasePrice = 0,
        int _atk = 0, int _def = 0, int _oxy = 0, int _cri = 0, float _mom = 0, float _atkSpeed = 0, float _speedX = 0, 
        float _skilling = 0, int _power = 0, int _itemCount = 0, string _mineralDisribution = "",
        int[] _productMaterial = null, int[] _productMaterialCount = null, int _production = 0, float coolTime = 10, int exp = 0)
    {
        itemID = _itemID;
        itemName = _itemName;
        itemDescription = _itemDes;
        mineralDistribution = _mineralDisribution;

        production = _production;
        productionMaterial = _productMaterial;
        productionMaterialCount = _productMaterialCount;
        
        itemType = _itemType;
        itemCount = _itemCount;
        storageItemCount = 0;
        if(itemID % 10000 >= 1000 && itemID / 10000 == 1) // 아이템이 상자면
            itemIcon = Resources.Load("ItemIcon/Chest", typeof(Sprite)) as Sprite;
        else if (itemID % 10000 >= 1000 && itemID / 10000 == 3) // 아이템이 제조법면
            itemIcon = Resources.Load("ItemIcon/Paper", typeof(Sprite)) as Sprite;
        else
            itemIcon = Resources.Load("ItemIcon/" + _itemID.ToString(), typeof(Sprite)) as Sprite; // Resources에 있는거 Sprite형태로 가져옴 
        
        itemPurchasePrice = _itemPurchasePrice;
        itemSalePrice = _itemSalePrice;
        maxItemCount = _maxItemCount;
        this.exp = exp;

        atk = _atk;
        def = _def;
        oxy = _oxy;
        cri = _cri;
        atkSpeed = _atkSpeed;
        mom = _mom;
        speedX = _speedX;
        skilling = _skilling;
        power = _power;

        this.coolTime = coolTime;
    }
}
