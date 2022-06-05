using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage : MonoBehaviour
{
    [SerializeField] GameObject storageUI;
    [SerializeField] GameObject takeOutButton;

    public List<Item> storageItemList;
    [SerializeField] StorageSlot[] slots;
    int slotCount;
    public int maxSlotCount;
    public Item selectedItem { get; set; }
    public StorageSlot selectedSlot { get; set; }

    DatabaseManager theDatabase;
    Inventory inventory;
    Player player;
    // Start is called before the first frame update
    void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        player = FindObjectOfType<Player>();
        theDatabase = FindObjectOfType<DatabaseManager>();
    }

    public void EnterStorage()
    {
        storageUI.SetActive(true);
        inventory.ShowInventory2(Inventory.InventoryMode.storage);
        player.CantMove();
        ShowItem();
    }

    public void ExitStorage()
    {
        storageUI.SetActive(false);
        inventory.CloseInventory2();
        player.CanMove();
    }

    public void RemoveStorage() // 인벤토리 슬롯 초기화
    {
        for (int i = 0; i < slots.Length; i++)
            slots[i].RemoveItem();
    }

    void InitializeSelected() // 선택된 템 정보랑 출력했던거 초기화
    {
        selectedItem = null;
    }

    void CheckNullItem()
    {
        for (int i = 0; i < storageItemList.Count; i++)
        {
            if (storageItemList[i].itemID == 0)
            {
                storageItemList.Remove(storageItemList[i]);
            }
        }
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].itemCount == 0)
                slots[i].RemoveItem();
        }
    }

    bool SearchItem(int itemID)
    {
        for (int i = 0; i < storageItemList.Count; i++)
        {
            if (storageItemList[i].itemID == itemID)
                return true;
        }
        return false;
    }

    public bool IsStorageFull(int _itemID = 0)
    {
        slotCount = 0;
        for (int i = 0; i < storageItemList.Count; i++)
        {
            if (storageItemList[i].storageItemCount > storageItemList[i].maxItemCount)
            {
                slotCount += Mathf.CeilToInt((float)storageItemList[i].storageItemCount / storageItemList[i].maxItemCount);
            }
            else
                slotCount++;
        }

        if (slotCount < maxSlotCount)
            return false;
        else
        {
            if (!SearchItem(_itemID))
                return true;
            for (int i = 0; i < storageItemList.Count; i++)
            {
                if (storageItemList[i].itemID == _itemID)
                {
                    if (storageItemList[i].storageItemCount % storageItemList[i].maxItemCount == 0)
                        return true;
                }
            }
            return false;
        }
    }

    public bool GetAnItem(int _itemID, int _count = 1)
    {
        if (IsStorageFull(_itemID))
        {
            AudioManager.instance.Play("Beep");
            InformationPanel.instance.EnableOK(Vector2.zero, "창고가 꽉 찼습니다.");
            Debug.Log("꽉참");
            return false;
        }

        for (int i = 0; i < theDatabase.itemList.Count; i++) // 데이터베이스 아이템 검색
        {
            if (_itemID == theDatabase.itemList[i].itemID) // 데이터베이스에 아이템 발견
            { 
                for (int j = 0; j < storageItemList.Count; j++) //소지품에 같은 아이템이 있는지 검색
                {
                    if (storageItemList[j].itemID == _itemID) // 소지품에 같은 템이 있으니 개수만 증감
                    {
                        storageItemList[j].storageItemCount += _count;
                        ShowItem();
                        return true;
                    }
                }
                storageItemList.Add(theDatabase.itemList[i]); // 소지품에 해당 아이템 추가.
                storageItemList[storageItemList.Count - 1].storageItemCount = _count;
                ShowItem();
                return true;
            }
        }
        Debug.LogError("데이터베이스에 해당 ID값을 가진 아이템이 존재하지 않습니다."); // 데이터베이스에 itemID 없음
        return false;
    }

    public void ShowItem() // 아이템 활성화 (inventoryTabList에 조건에 맞는 아이템들만 넣어주고, 인벤토리 슬롯에 출력
    {
        takeOutButton.SetActive(false);
        CheckNullItem();
        RemoveStorage();
        slotCount = 0;

        inventory.ShowItem();

        for (int i = 0; i < slots.Length; i++) // 선택된 템 흐리게하는 효과 일단 다 헤제
            slots[i].selected_Item.SetActive(false);

        for (int i = 0; i < storageItemList.Count; i++) // 인벤토리 탭 리스트의 내용을, 인벤토리 슬롯에 추가
        {
            if (storageItemList[i].storageItemCount > storageItemList[i].maxItemCount) // 하나의 슬롯에 들어갈수있는 최대개수 초과로 있는템들
            {
                int neededSlot = Mathf.CeilToInt((float)storageItemList[i].storageItemCount / storageItemList[i].maxItemCount);

                if (storageItemList[i].itemType == Item.ItemType.Mineral) // 광물이면 하나 뺴줘야 오류안남
                    neededSlot -= 1;

                for (int j = 0; j < neededSlot; j++)
                    slots[slotCount++].Additem(storageItemList[i], storageItemList[i].maxItemCount);
                if (storageItemList[i].storageItemCount % storageItemList[i].maxItemCount != 0)
                    slots[slotCount++].Additem(storageItemList[i], storageItemList[i].storageItemCount % storageItemList[i].maxItemCount);
            }
            else
            {
                slots[slotCount++].Additem(storageItemList[i], storageItemList[i].storageItemCount);
            }
        }
    }

    public void TouchItem()
    {
        if(selectedItem.itemType != Item.ItemType.Null)
            takeOutButton.SetActive(true);
    }

    public void TouchTakeOutButton()
    {
        if (selectedSlot.itemCount > 1)
            InformationPanel.instance.EnableUpDown("꺼낼 개수를 정해주세요.", TakeOutItem, selectedSlot.itemCount);
        else
            TakeOutItem();
    }

    public void TakeOutItem()
    {
        int count;
        if (selectedSlot.itemCount > 1)
            count = InformationPanel.instance.ReturnValue();
        else
            count = 1;

        if (!inventory.GetAnItem(selectedItem.itemID, count))
            return;
        RemoveItem(selectedItem.itemID, count);
        ShowItem();
    }

    public void RemoveItem(int _itemID, int _count = 1) // 아이템 삭제
    {
        for (int i = 0; i < storageItemList.Count; i++)
        {
            if (_itemID == storageItemList[i].itemID)
            {
                if (storageItemList[i].storageItemCount - _count <= 0)
                {
                    storageItemList[i].storageItemCount = 0;
                    storageItemList.RemoveAt(i);
                }
                else
                    storageItemList[i].storageItemCount -= _count;
            }
        }
    }

    public void RemoveAllItem() // 소지템 모두 제거
    {
        for (int i = 0; i < storageItemList.Count; i++)
            storageItemList[i].itemCount = 0;
        storageItemList.Clear();
    }
}
