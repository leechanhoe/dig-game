using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Smithy : MonoBehaviour
{
    public List<int> production = new List<int>();
    public GameObject SmithyUI;
    public GameObject SmithySlotPrefab;
    [SerializeField]
    Transform tf;
    [SerializeField]
    GameObject methodUI;
    [SerializeField]
    Text methodText;
    [SerializeField] GameObject removeButton;
    List<GameObject> smithySlots = new List<GameObject>();
    public Item selectedItem { get; set; }
    public SmithySlot selectedSlot { get; set; }

    DatabaseManager database;
    Inventory inventory;
    Player player;
    // Start is called before the first frame update
    void Start()
    {
        database = FindObjectOfType<DatabaseManager>();
        inventory = FindObjectOfType<Inventory>();
        player = FindObjectOfType<Player>();
    }

    private void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (GameManager.gameMode == GameManager.GameMode.OpenProductionMethodPanel && Input.GetKey(KeyCode.Escape) && !DatabaseManager.preventDoubleClick)
            {
                StartCoroutine(PreventDoubleClickC());
            }
        }
    }
    IEnumerator PreventDoubleClickC()
    {
        DatabaseManager.preventDoubleClick = true;
        CloseMethodUI();
        yield return new WaitForSeconds(0.1f);
        DatabaseManager.preventDoubleClick = false;
    }

    public void EnterSmithy()
    {
        SmithyUI.SetActive(true);
        inventory.ShowInventory2(Inventory.InventoryMode.smithy);
        player.CantMove();
        ShowProduction();
        Time.timeScale = 0;
    }

    public void ExitSmithy()
    {
        SmithyUI.SetActive(false);
        inventory.CloseInventory2();
        player.CanMove();
        Time.timeScale = 1;
    }

    public void InitializeProduction()
    {
        production.Clear();
    }

    public void RemoveButtonActive()
    {
        removeButton.SetActive(true);
    }

    public void ShowProduction()
    {
        removeButton.SetActive(false);
        for (int i = 0; i < smithySlots.Count; i++)
            Destroy(smithySlots[i]);
        smithySlots.Clear();
        for (int i = 0; i < production.Count; i++)
        {
            smithySlots.Add(Instantiate(SmithySlotPrefab, tf));
            smithySlots[i].GetComponent<SmithySlot>().database = database;
            smithySlots[i].GetComponent<SmithySlot>().AddItem(production[i]);
        }
    }

    public void InitializeSelected()
    {
        for (int i = 0; i < production.Count; i++)
        {
            smithySlots[i].GetComponent<SmithySlot>().selected.SetActive(false);
        }
    }

    public void AddProductionableItem(int itemID)
    {
        production.Add(itemID);
        ShowProduction();
    }

    public void TouchRemoveButton()
    {
        InformationPanel.instance.EnableOOC(Vector2.zero, "해당 제조법을 삭제하시겠습니까?", "확인", "취소", RemoveItem);
    }

    public void RemoveItem()
    {
        production.Remove(selectedSlot.item.itemID);
        InformationPanel.instance.EnableOK(Vector2.zero, "삭제완료", "확인");
        ShowProduction();
    }

    public void OpenMethodUI() // 제작법창 열기
    {
        GameManager.gameMode = GameManager.GameMode.OpenProductionMethodPanel;
        methodUI.SetActive(true);
        methodText.text = "";
        for (int i = 0; i < selectedItem.productionMaterial.Length; i++)
        {
            Item material = database.ReturnItem(selectedItem.productionMaterial[i]);
            methodText.text += "● " + material.itemName + " " + material.itemCount + " / " + selectedItem.productionMaterialCount[i] + "\n";
        }
    }

    public void CloseMethodUI() //제작법창 닫기
    {
        methodUI.SetActive(false);
        methodText.text = "";
        selectedItem = new Item();
        GameManager.gameMode = GameManager.GameMode.OpenInventory;
    }

    public void ClickProduceButton() // 제작법에서 제작 버튼 눌렀을 때
    {
        for (int i = 0; i < selectedItem.productionMaterial.Length; i++)
        {
            Item material = database.ReturnItem(selectedItem.productionMaterial[i]);
            if (material.itemCount - selectedItem.productionMaterialCount[i] < 0) // 재료가 없을 때
            {
                FindObjectOfType<InformationPanel>().EnableOK(new Vector2(650, 400), "재료가 부족합니다.", "확인");
                AudioManager.instance.Play("Beep");
                CloseMethodUI();
                return;
            }
            material.itemCount -= selectedItem.productionMaterialCount[i];
        }
        FindObjectOfType<InformationPanel>().EnableOK(new Vector2(650, 400), "제작이 완료되었습니다.", "확인");
        inventory.GetAnItem(selectedItem.itemID);
        inventory.ShowItem();
        CloseMethodUI();
    }
}
