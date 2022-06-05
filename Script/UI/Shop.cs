using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public List<int> saleUseItems;
    public List<int> saleEquipItems;
    public List<int> saleEtcItems;
    public bool cantSell; // 판매는 불가능한 상점 
    public GameObject ShopUI;
    List<GameObject> shopSlotsList = new List<GameObject>();
    public GameObject shopSlotPrefab;
    public Transform tf;
    public GameObject[] tab; // 소비 장비 기타 탭

    Inventory inventory;
    Player player;

    // Start is called before the first frame update
    void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        player = FindObjectOfType<Player>();
        //shopSlots = new GameObject[saleEquipItems.count + saleUseItems.Length + saleEtcItems.Length];
    }

    public void EnterShop()
    {
        player.CantMove();
        if (cantSell)
            inventory.cantSell = true;
        else
            inventory.cantSell = false;
        inventory.ShowInventory2(Inventory.InventoryMode.shop);
        ShopUI.SetActive(true);
        TouchTab(0);
        Time.timeScale = 0;
    }

    public void ExitShop()
    {
        player.CanMove();
        inventory.CloseInventory2();
        ShopUI.SetActive(false);
        Time.timeScale = 1;
    }

    public void TouchTab(int itemType)
    {
        for (int i = 0; i < tab.Length;i++)
            tab[i].SetActive(false);
        for (int i = 0; i < shopSlotsList.Count; i++)
            Destroy(shopSlotsList[i]);
        shopSlotsList.Clear();
        switch (itemType)
        {
            case 0: // 소비
                tab[itemType].SetActive(true);
                for (int i = 0; i < saleUseItems.Count; i++)
                {
                    shopSlotsList.Add(Instantiate(shopSlotPrefab, tf));
                    shopSlotsList[i].GetComponent<ShopSlot>().AddItem(saleUseItems[i]);
                }
                break;
            case 1: // 장비
                tab[itemType].SetActive(true);
                for (int i = 0; i < saleEquipItems.Count; i++)
                {
                    shopSlotsList.Add(Instantiate(shopSlotPrefab, tf));
                    shopSlotsList[i].GetComponent<ShopSlot>().AddItem(saleEquipItems[i]);
                }
                break;
            case 2: // 기타
                tab[itemType].SetActive(true);
                for (int i = 0; i < saleEtcItems.Count; i++)
                {
                    shopSlotsList.Add(Instantiate(shopSlotPrefab, tf));
                    shopSlotsList[i].GetComponent<ShopSlot>().AddItem(saleEtcItems[i]);
                }
                break;
        }
        ShopSlot[] shopSlots = new ShopSlot[shopSlotsList.Count];
        for (int i = 0; i < shopSlots.Length; i++)
            shopSlots[i] = shopSlotsList[i].GetComponent<ShopSlot>();
        for (int i = 0; i < shopSlots.Length; i++)
            shopSlots[i].slots = shopSlots;
        AudioManager.instance.Play("TouchButton");
    }
}
