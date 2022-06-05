using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayCanvas : MonoBehaviour
{
    static public PlayCanvas instance;
    public Image actButton;
    public Text actButtonText;
    public Sprite[] sprites;
    public GameObject shopUI;
    public Transform shopSlotTf;
    public GameObject[] shopTab;

    public bool isTutorial { get; set; }
    [SerializeField] GameObject[] uiManual;
    bool[] haveSeenManual = new bool[4];
    const int JOYPAD = 0, ACT_BUTTON = 1, QUICKSLOT = 2, BAR = 3;

    // Start is called before the first frame update

    Player player;
    Smithy smithy;
    Storage storage;
    TransferMap transferMap;

    private void Awake()
    {
        if (instance == null) // 파괴방지
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
        else
            Destroy(this.gameObject);
        player = FindObjectOfType<Player>();
        smithy = FindObjectOfType<Smithy>();
        storage = FindObjectOfType<Storage>();
        transferMap = FindObjectOfType<TransferMap>();

    }

    public void OnShop()
    {
        EntryAction();
        player.shop.ShopUI = shopUI;
        player.shop.tab = shopTab;
        player.shop.tf = shopSlotTf;
    }

    public void ExitAction()
    {
        actButton.sprite = sprites[0];
        actButtonText.text = "▲";
        player.actionState = Player.ActionState.Normal;
    }

    public void EntryAction()
    {
        actButton.sprite = sprites[1];
        actButtonText.text = "";
    }

    public void TouchTab(int itemType)
    {
        player.shop.TouchTab(itemType);
    }

    public void CloseShopUI()
    {
        player.shop.ExitShop();
    }

    public void CloseSmithyUI()
    {
        smithy.ExitSmithy();
    }

    public void CloseStorageUI()
    {
        storage.ExitStorage();
    }

    public void CloseCartUI()
    {
        transferMap.CloseTfMapUI();
    }

    public void TouchManualPanel(int type)
    {
        haveSeenManual[type] = true;
    }

    public void UITutorial()
    {
        StartCoroutine(UITutorialC());
    }

    IEnumerator UITutorialC()
    {
        isTutorial = true;
        yield return new WaitUntil(() => !FadeManager.instance.blackObject.activeSelf);
        yield return new WaitForSeconds(1f);

        player.CantMove();
        uiManual[JOYPAD].SetActive(true);
        yield return new WaitUntil(() => haveSeenManual[JOYPAD]);
        uiManual[JOYPAD].SetActive(false);

        uiManual[ACT_BUTTON].SetActive(true);
        yield return new WaitUntil(() => haveSeenManual[ACT_BUTTON]);
        uiManual[ACT_BUTTON].SetActive(false);

        uiManual[QUICKSLOT].SetActive(true);
        yield return new WaitUntil(() => haveSeenManual[QUICKSLOT]);
        uiManual[QUICKSLOT].SetActive(false);

        uiManual[BAR].SetActive(true);
        yield return new WaitUntil(() => haveSeenManual[BAR]);
        uiManual[BAR].SetActive(false);
        isTutorial = false;
        player.CanMove();
    }
}
