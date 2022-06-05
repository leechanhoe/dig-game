using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    private DatabaseManager theDatabase;
    private AudioManager theAudio;
    private Equipment equipment;
    Player player;

    public string cancel_sound;
    public string open_sound;
    public string beep_sound; // 잘못된 행동 했을 시 사운드
    public string touch_sound;
    public string use_sound;
    public string dump_sound;
    public string getItemSound;

    public InventorySlot[] slots;
    public int slotCount;

    public List<Item> inventoryItemList = new List<Item>(); // 플레이어가 소지한 아이템 리스트

    public Text Description_Text; // 부연설명

    public GameObject goInventory; // 인벤토리 버튼
    public GameObject inventoryUI; // 인벤토리 활성화 비활성화
    public GameObject useItemButton; // 아이템 사용버튼
    public GameObject dumpItemButton; // 아이템 버리기 버튼
    public GameObject QuickSlotButton; // 인벤토리에서의 퀵슬롯 버튼
    public Text itemNameText;
    public Text useItemText;
    public Text dumpItemText;

    public Item selectedItem; // 선택된 아이템
    public InventorySlot selectedSlot; // 선택된 슬롯

    public GameObject prefab_floating_Text;

    public GameObject SlotPanel;
    public GameObject descriptionPanel;
    public GameObject equipPanel;

    public GameObject sellAllButton;
    public GameObject sellButton;
    public GameObject produceButton;
    [SerializeField] GameObject storeButton;

    public QuickSlot[] QuickSlots; // 퀵슬롯에있는 아이템 정보
    bool setupQuickSlotMode;
    public GameObject quickFadeBlack;

    public Text moneyText; // 소지금
    PlayerStat playerStat;
    Storage storage;
    public Transform floatTf;
    public bool cantSell { get; set; }

    public enum InventoryMode
    {
        normal,
        shop,
        smithy,
        storage
    }
    InventoryMode inventoryMode;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        theAudio = FindObjectOfType<AudioManager>();
        theDatabase = FindObjectOfType<DatabaseManager>();
        equipment = FindObjectOfType<Equipment>();
        player = FindObjectOfType<Player>();
        playerStat = FindObjectOfType<PlayerStat>();
        storage = FindObjectOfType<Storage>();

    }

    private void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (GameManager.gameMode == GameManager.GameMode.OpenInventory && Input.GetKey(KeyCode.Escape) && !DatabaseManager.preventDoubleClick)
            {
                StartCoroutine(PreventDoubleClickC());
            }
        }
    }

    IEnumerator PreventDoubleClickC()
    {
        DatabaseManager.preventDoubleClick = true;
        if (inventoryMode == InventoryMode.shop)
            player.shop.ExitShop();
        else if (inventoryMode == InventoryMode.smithy)
            FindObjectOfType<Smithy>().ExitSmithy();
        else
            CloseInventory();
        yield return new WaitForSeconds(0.1f);
        DatabaseManager.preventDoubleClick = false;
    }

    public void ShowInventory()
    {
        inventoryMode = InventoryMode.normal;
        theAudio.Play(open_sound);
        //player.NotMove();
        inventoryUI.SetActive(true);
        equipPanel.SetActive(true);
        descriptionPanel.SetActive(true);
        goInventory.SetActive(false);

        Description_Text.text = "";
        itemNameText.text = "";
        useItemButton.SetActive(false);
        dumpItemButton.SetActive(false);
        QuickSlotButton.SetActive(false);

        GameManager.gameMode = GameManager.GameMode.OpenInventory;
        SubMenu.instance.MenuClose();
        Minimap.MinimapOff();
        ShowItem();
    }

    public void ShowInventory2(InventoryMode mode) // 이건 상점,대장간용
    {
        inventoryUI.SetActive(true);
        descriptionPanel.SetActive(false);
        equipPanel.SetActive(false);
        inventoryMode = mode;
        GameManager.gameMode = GameManager.GameMode.OpenInventory;
        Minimap.MinimapOff();
        ShowItem();
    }

    public void CloseInventory()
    {
        if (!inventoryUI.activeSelf)
            return;
        FindObjectOfType<StatPanel>().CloseStatPanel();
        goInventory.SetActive(true);
        equipPanel.SetActive(false);
        descriptionPanel.SetActive(false);
        theAudio.Play(cancel_sound);
        inventoryUI.SetActive(false);

        GameManager.gameMode = GameManager.GameMode.Normal;
        Minimap.MinimapOn();
        //player.CanMove();
    }
    public void CloseInventory2() // 상점용
    {
        descriptionPanel.SetActive(true);
        equipPanel.SetActive(true);
        inventoryUI.SetActive(false);
        inventoryMode = InventoryMode.normal;

        GameManager.gameMode = GameManager.GameMode.Normal;
        Minimap.MinimapOn();
    }

    public List<Item> SaveItem()
    {
        return inventoryItemList;
    }

    public void LoadItem(List<Item> _itemList)
    {
        inventoryItemList = _itemList;
    }

    public void EquipToInventory(Item _item)
    {
        inventoryItemList.Add(_item);
        _item.itemCount++;
        CheckNullItem();
    }
    
    public bool IsInventoryFull(int _itemID = 0)
    {
        slotCount = 0;
        for(int i = 0;i < inventoryItemList.Count;i++)
        {
            if (inventoryItemList[i].itemCount > inventoryItemList[i].maxItemCount)
            {
                slotCount += Mathf.CeilToInt((float)inventoryItemList[i].itemCount / inventoryItemList[i].maxItemCount);
            }
            else
                slotCount++;
        }

        if (slotCount < playerStat.maxItemSlot)
            return false;
        else
        {
            if (!SearchItem(_itemID))
                return true;
            for (int i = 0; i < inventoryItemList.Count; i++)
            {
                if (inventoryItemList[i].itemID == _itemID)
                {
                    if (inventoryItemList[i].itemCount % inventoryItemList[i].maxItemCount == 0)
                        return true;
                }
            }
            return false;
        }
    }

    public bool SearchItem(int itemID)
    {
        for (int i = 0; i < inventoryItemList.Count; i++)
        {
            if (inventoryItemList[i].itemID == itemID && inventoryItemList[i].itemCount > 0)
                return true;
        }
        return false;
    }

    public void ShowInvenFullPanel()
    {
        InformationPanel.instance.EnableOK(Vector2.zero, "인벤토리가 꽉 찼습니다.", "확인");
    }

    public bool GetAnItem(int _itemID, int _count = 1, bool floatText = true, bool soundEffect = true)
    {
        if(IsInventoryFull(_itemID))
        {
                theAudio.Play(cancel_sound);
                Debug.Log("꽉참");
                return false;
        }

        for(int i = 0;i < theDatabase.itemList.Count;i++) // 데이터베이스 아이템 검색
        {
            if (_itemID == theDatabase.itemList[i].itemID) // 데이터베이스에 아이템 발견
            {
                if (floatText)
                {
                    var clone = Instantiate(prefab_floating_Text, new Vector3(player.transform.position.x,
                    player.transform.position.y + 0.5f, player.transform.position.z), Quaternion.Euler(Vector3.zero)); // 정확한 형식을 모를 떄 var , instantiate 는 프리펩 생성해서 클론에 넘
                                                                                                                       // instantiate(대상,위치,각도)
                    clone.GetComponent<FloatingText>().text.text = "+" + theDatabase.itemList[i].itemSalePrice;
                    clone.transform.SetParent(floatTf);
                    clone.GetComponent<FloatingText>().text.color = Color.green;
                    clone.transform.localScale = new Vector3(1f, 1f, 1f);
                }
                if(soundEffect)
                    theAudio.Play(getItemSound);

                for(int j = 0;j < inventoryItemList.Count;j++) //소지품에 같은 아이템이 있는지 검색
                {
                    if(inventoryItemList[j].itemID == _itemID) // 소지품에 같은 템이 있으니 개수만 증감
                    {
                        inventoryItemList[j].itemCount += _count;
                        if(inventoryItemList[j].isOnQuickslot)
                        {
                            for (int k = 0; k < QuickSlots.Length; k++)
                            {
                                if (QuickSlots[k].item == inventoryItemList[j])
                                    QuickSlots[k].AddItem(inventoryItemList[j]);
                            }
                        }
                        return true;
                    }
                }
                inventoryItemList.Add(theDatabase.itemList[i]); // 소지품에 해당 아이템 추가.
                inventoryItemList[inventoryItemList.Count - 1].itemCount = _count;
                for (int k = 0; k < QuickSlots.Length; k++)
                {
                    if (QuickSlots[k].item == inventoryItemList[inventoryItemList.Count - 1])
                        QuickSlots[k].AddItem(inventoryItemList[inventoryItemList.Count - 1]);
                }
                return true;
            }
        }
        Debug.LogError("데이터베이스에 해당 ID값을 가진 아이템이 존재하지 않습니다."); // 데이터베이스에 itemID 없음
        return false;
    }

    public void RemoveItem(int _itemID, int _count = 1) // 아이템 삭제
    {
        for (int i = 0;i < inventoryItemList.Count;i++)
        {
            if (_itemID == inventoryItemList[i].itemID)
            {
                if (_itemID == 10203)
                {
                    player.RemoveStoredPos();
                    inventoryItemList[i].itemDescription = "현재 위치를 저장한다. 다시 저장한 위치로 이동할 수 있다. 저장된 위치 : 지하 ";
                }
                if (inventoryItemList[i].itemCount - _count <= 0)
                {
                    inventoryItemList[i].itemCount = 0;
                    inventoryItemList.RemoveAt(i);
                }
                else
                    inventoryItemList[i].itemCount -= _count;
            }
        }
    }

    public void RemoveAllItem() // 소지템 모두 제거
    {
        for (int i = 0; i < inventoryItemList.Count; i++)
            inventoryItemList[i].itemCount = 0;
        inventoryItemList.Clear();
    }

    public void RemoveSlot() // 인벤토리 슬롯 초기화
    {
        for(int i = 0;i < slots.Length;i++)
        {   
            slots[i].RemoveItem();
            slots[i].gameObject.SetActive(false);
        }
    }

    void InitializeSelected() // 선택된 템 정보랑 출력했던거 초기화
    {
        selectedItem = null;
        Description_Text.text = "";
        itemNameText.text = "";
        useItemButton.SetActive(false);
        dumpItemButton.SetActive(false);
        QuickSlotButton.SetActive(false);
    }

    void CheckNullItem()
    {
        for (int i = 0; i < inventoryItemList.Count; i++)
        {
            if (inventoryItemList[i].itemID == 0)
            {
                inventoryItemList.Remove(inventoryItemList[i]);
            }
        }
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].itemCount == 0)
                slots[i].RemoveItem();
        }
    }
    public void ShowItem() // 아이템 활성화 (inventoryTabList에 조건에 맞는 아이템들만 넣어주고, 인벤토리 슬롯에 출력
    {
        CheckNullItem();
        sellButton.SetActive(false);
        produceButton.SetActive(false);
        storeButton.SetActive(false);
        RemoveSlot();
        slotCount = 0;

        for (int i = 0; i < slots.Length; i++) // 선택된 템 흐리게하는 효과 일단 다 헤제
            slots[i].selected_Item.SetActive(false);
        for (int i = 0;i < playerStat.maxItemSlot;i++)
            slots[i].gameObject.SetActive(true);

        for (int i = 0;i < inventoryItemList.Count; i++) // 인벤토리 탭 리스트의 내용을, 인벤토리 슬롯에 추가
        {
            if (inventoryItemList[i].itemCount > inventoryItemList[i].maxItemCount) // 하나의 슬롯에 들어갈수있는 최대개수 초과로 있는템들
            {
                int neededSlot = Mathf.CeilToInt((float)inventoryItemList[i].itemCount / inventoryItemList[i].maxItemCount);

                if (inventoryItemList[i].itemType == Item.ItemType.Mineral) // 광물이면 하나 뺴줘야 오류안남
                    neededSlot -= 1;

                for (int j = 0; j < neededSlot; j++)
                    slots[slotCount++].Additem(inventoryItemList[i], inventoryItemList[i].maxItemCount);
                if (inventoryItemList[i].itemCount % inventoryItemList[i].maxItemCount != 0)
                    slots[slotCount++].Additem(inventoryItemList[i], inventoryItemList[i].itemCount % inventoryItemList[i].maxItemCount);
            }
            else
            {
                slots[slotCount++].Additem(inventoryItemList[i],inventoryItemList[i].itemCount);
            }
        }
        moneyText.text = playerStat.money.ToString();
        SellAllButtonControl();
    }

    void SellAllButtonControl() // 모두 팔기 버튼 컨트롤
    {
        if (inventoryMode != InventoryMode.shop || cantSell)
        {
            sellAllButton.SetActive(false);
            return;
        }
        bool haveMineral = false;
        for (int i = inventoryItemList.Count - 1; i >= 0; i--)
        {
            if (inventoryItemList[i].itemType == Item.ItemType.Mineral)
            {
                haveMineral = true;
            }
        }
        if (haveMineral && !cantSell)
            sellAllButton.SetActive(true);
        else
            sellAllButton.SetActive(false);
    }

    public void TouchItem2() // 아이템 아이콘 터치시 
    {
        theAudio.Play(touch_sound);
        Description_Text.text = selectedItem.itemDescription;
        itemNameText.text = selectedItem.itemName;
        useItemButton.SetActive(false);
        dumpItemButton.SetActive(false);
        QuickSlotButton.SetActive(false);
        sellButton.SetActive(false);
        storeButton.SetActive(false);
        produceButton.SetActive(false);

        if (selectedItem.itemType == Item.ItemType.Use) // 소비창
        {
            useItemText.text = "사용";
            useItemButton.SetActive(true);
            QuickSlotButton.SetActive(true);
            dumpItemButton.SetActive(true);
        }
        else if (selectedItem.itemType == Item.ItemType.Equip) // 장비창
        {
            useItemText.text = "장착";
            useItemButton.SetActive(true);
            dumpItemButton.SetActive(true);
        }
        else if (selectedItem.itemType == Item.ItemType.Etc) // 기타
        {
            dumpItemButton.SetActive(true);
        }
        else if (selectedItem.itemType == Item.ItemType.Mineral) // 광물
        {
            dumpItemButton.SetActive(true);
            Description_Text.text = "";
        }

        if (inventoryMode == InventoryMode.shop && selectedItem.itemType != Item.ItemType.Quest && selectedItem.itemType != Item.ItemType.Null && !cantSell)
            sellButton.SetActive(true);
        else if (inventoryMode == InventoryMode.smithy && selectedItem.production > 0 && selectedItem.itemID / 10000 == 3) // 제조법일때
            produceButton.SetActive(true);
        else if (inventoryMode == InventoryMode.storage && selectedItem.itemType != Item.ItemType.Null)
            storeButton.SetActive(true);
    }

    public void RemoveItemButton() // 템 버리기 버튼 클릭시
    {
        RemoveItem(selectedSlot.item.itemID, selectedSlot.itemCount);
        theAudio.Play(dump_sound);
        InitializeSelected();
        ShowItem();
    }

    public void ItemUseButton()
    {
        if (selectedItem.itemType == Item.ItemType.Use)
            ItemUse();
        else if (selectedItem.itemType == Item.ItemType.Equip)
            EquipButton();
    }
    public void ItemUse() // 아이템 사용버튼 누를시
    {
        if(!theDatabase.UseItem(selectedItem.itemID,true))
            return;

        theAudio.Play(use_sound);
        RemoveItem(selectedItem.itemID);
        InitializeSelected();
        ShowItem();
    }

    public void EquipButton() // 장착버튼 눌렀을 떄
    {
        equipment.EquipItem(selectedItem);
        RemoveItem(selectedItem.itemID);
        InitializeSelected();
        ShowItem();
    }

    public void SetupQuickSlot() // 인벤토리에 있는 퀵슬롯 설정버튼 누를시
    {
        theAudio.Play(touch_sound);
        setupQuickSlotMode = true;
        inventoryUI.SetActive(false);
        quickFadeBlack.SetActive(true);
    }

    public void ExitSetupQuickSlot() // 퀵슬롯 설정 모드에서 퀵슬롯버튼을 제외한 아무 곳이나 터치했을 경우
    {
        if (!setupQuickSlotMode)
            return;
        inventoryUI.SetActive(true);
        setupQuickSlotMode = false;
        quickFadeBlack.SetActive(false);
    }

    public void TouchQuickSlot(int index) // 퀵슬롯 터치시
    {
        if (setupQuickSlotMode) // 퀵슬롯 설정 모드시
        {
            if (QuickSlots[index].item == selectedItem) // 똑같은 아이템 터치시 퀵슬롯 삭제
                QuickSlots[index].RemoveItem();
            else
            {
                QuickSlots[index].AddItem(selectedItem);
            }
            return;
        }

        if (QuickSlots[index].isOn)
        {
            if (QuickSlots[index].item.itemCount <= 0 || !theDatabase.UseItem(QuickSlots[index].item.itemID,true))
                return;

            RemoveItem(QuickSlots[index].item.itemID);
            QuickSlots[index].AddItem(QuickSlots[index].item);

            if (QuickSlots[index].item.itemCount == 0)
                QuickSlots[index].ExhaustItem(QuickSlots[index].item);
            QuickSlots[index].UseItem();
        }
    }

    public int CalculateSumOfPrice(bool sell = false) // 템창의 광물 가격의 합
    {
        int sum = 0;
        for (int i = inventoryItemList.Count - 1; i >= 0; i--)
        {
            if (inventoryItemList[i].itemType == Item.ItemType.Mineral)
            {
                sum += inventoryItemList[i].itemCount * inventoryItemList[i].itemSalePrice;
                if (sell)
                {
                    inventoryItemList[i].itemCount = 0;
                    inventoryItemList.Remove(inventoryItemList[i]);
                }
            }
        }
        return sum;
    }

    public void SellAllMineral()
    {
        playerStat.money += CalculateSumOfPrice(true);
        theAudio.Play("Sell Item");
        ShowItem();
    }

    public void SellItem(int _itemCount)
    {
        for (int i = inventoryItemList.Count - 1; i >= 0; i--)
        {
            if (inventoryItemList[i] == selectedItem)
            {
                inventoryItemList[i].itemCount -= _itemCount;
                if (inventoryItemList[i].itemCount == 0)
                    inventoryItemList.Remove(inventoryItemList[i]);
            }
        }
        playerStat.money += selectedItem.itemSalePrice * _itemCount;
        theAudio.Play("Sell Item");
        ShowItem();
    }

    public void GetProduction() // 제작법 터득버튼 클릭
    {
        FindObjectOfType<Smithy>().AddProductionableItem(selectedItem.production);
        RemoveItem(selectedItem.itemID);
        ShowItem();
    }

    public void TouchStoreButton()
    {

        if (selectedSlot.itemCount > 1)
            InformationPanel.instance.EnableUpDown("몇 개를 보관하시겠습니까?", Store, selectedSlot.itemCount);
        else if (selectedSlot.itemCount == 1)
            Store();
    }

    public void Store()
    {
        int count;
        if (selectedSlot.itemCount > 1)
            count = InformationPanel.instance.ReturnValue();
        else if (selectedSlot.itemCount == 1)
            count = 1;
        else
        {
            Debug.LogError("창고에 0개 이하인 템이 선택됨 ㄷㄷ");
            count = 0;
        }

        if (storage.GetAnItem(selectedItem.itemID, count))
            RemoveItem(selectedItem.itemID, count);
        ShowItem();
    }
}