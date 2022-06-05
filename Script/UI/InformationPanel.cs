using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InformationPanel : MonoBehaviour
{
    public static InformationPanel instance;

    public delegate void ClickButton();
    ClickButton clickOkButton; // 여따 함수넣으면 ok눌렀을때 그함수 실행함
    ClickButton clickCancelButton;
    public GameObject oocPanel; // ok or cancel panel
    public RectTransform oocRectTransform;

    public Text oocContent;
    public Text oocOkText;
    public Text oocCancelText;

    public GameObject okPanel; // 확인만 나오는 창
    public RectTransform okRectTransform;

    public Text okContent;
    public Text okButtonText;

    [SerializeField] GameObject upDownPanel; // ok or cancel panel

    int minValue;
    int maxValue;
    int selectedCount; // 화살표로 정한 개수
    [SerializeField] Text upDownContent;
    [SerializeField] Text countText;

    [SerializeField] GameObject downInfoPanel;
    [SerializeField] Text downInfoText;
    [SerializeField] Animator downPanelanimator;

    [SerializeField] GameObject preventOtherTouch;

    GameManager.GameMode prevGameMode;

    // Start is called before the first frame update

    private void Awake()
    {
        if (instance == null) // 파괴방지
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
        else
            Destroy(this.gameObject);
    }
    void Start()
    {
        oocRectTransform = oocPanel.GetComponent<RectTransform>();
        instance = this;
    }

    private void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (GameManager.gameMode == GameManager.GameMode.OpenInformationPanel && Input.GetKey(KeyCode.Escape) && !DatabaseManager.preventDoubleClick)
            {
                StartCoroutine(PreventDoubleClickC());
            }
        }
    }

    IEnumerator PreventDoubleClickC()
    {
        DatabaseManager.preventDoubleClick = true;
        DisableOkPanel();
        DisableOOCPanel();
        yield return new WaitForSeconds(0.1f);
        DatabaseManager.preventDoubleClick = false;
    }

    public void EnableOOC(Vector2 size, string _content, string _okText, string _cancelText, ClickButton _clickOkButton, ClickButton _clickCancelButton = null) // 팝업창 띄우기
    {
        prevGameMode = GameManager.gameMode;
        GameManager.gameMode = GameManager.GameMode.OpenInformationPanel;
        oocPanel.SetActive(true);
        preventOtherTouch.SetActive(true);
        if (size == Vector2.zero)
            size = new Vector2(650, 400);
        oocRectTransform.sizeDelta = size;
        oocContent.text = _content;
        oocOkText.text = _okText;
        oocCancelText.text = _cancelText;

        clickOkButton = _clickOkButton;
        if (_clickCancelButton == null)
            clickCancelButton = DisableOOCPanel;
        else
            clickCancelButton = _clickCancelButton;
    }

    public void OkButton()
    {
        if (clickOkButton != null) 
            clickOkButton();
        clickOkButton = null;
        oocPanel.SetActive(false);
        okPanel.SetActive(false);
        upDownPanel.SetActive(false);
        preventOtherTouch.SetActive(false);
        GameManager.gameMode = prevGameMode;
    }

    public void CancelButton()
    {
        if(clickCancelButton != null)
            clickCancelButton();
        clickCancelButton = null;
        preventOtherTouch.SetActive(false);
        oocPanel.SetActive(false);
        upDownPanel.SetActive(false);
        GameManager.gameMode = prevGameMode;
    }

    public void DisableOOCPanel()
    {
        clickOkButton = null;
        clickCancelButton = null;
        oocPanel.SetActive(false);
        preventOtherTouch.SetActive(false);
        GameManager.gameMode = prevGameMode;
    }

    public void EnableOK(Vector2 size, string _content, string _okText = "확인",ClickButton clickOkButton = null) // 팝업창 띄우기
    {
        prevGameMode = GameManager.gameMode;
        GameManager.gameMode = GameManager.GameMode.OpenInformationPanel;
        okPanel.SetActive(true);
        preventOtherTouch.SetActive(true);

        if (size == Vector2.zero)
            size = new Vector2(650, 400);
        okRectTransform.sizeDelta = size;
        okContent.text = _content;
        okButtonText.text = _okText;

        if (clickOkButton == null)
            this.clickOkButton = DisableOkPanel;
        else
            this.clickOkButton = clickOkButton;
    }

    public void DisableOkPanel()
    {
        okPanel.SetActive(false);
        preventOtherTouch.SetActive(false);
        GameManager.gameMode = prevGameMode;
    }

    public void EnableUpDown(string _content,ClickButton _clickOkButton,int maxValue,int minValue = 1, ClickButton _clickCancelButton = null) // 팝업창 띄우기
    {
        prevGameMode = GameManager.gameMode;
        GameManager.gameMode = GameManager.GameMode.OpenInformationPanel;
        upDownPanel.SetActive(true);
        preventOtherTouch.SetActive(true);

        selectedCount = maxValue;
        countText.text = selectedCount.ToString();

        this.maxValue = maxValue;
        this.minValue = minValue;
        upDownContent.text = _content;

        clickOkButton = _clickOkButton;
        if (_clickCancelButton == null)
            clickCancelButton = DisableUpDownPanel;
        else
            clickCancelButton = _clickCancelButton;
    }

    public void DisableUpDownPanel()
    {
        upDownPanel.SetActive(false);
        preventOtherTouch.SetActive(false);
        GameManager.gameMode = prevGameMode;
    }

    public void ModulateItemCount(bool up) // 개수 조절 화살표를 눌렀을 때
    {
        if (up)
        {
            selectedCount += 1;
            if (selectedCount > maxValue)
                selectedCount = maxValue;
        }
        else
        {
            selectedCount -= 1;
            if (selectedCount < minValue)
                selectedCount = minValue;
        }
        countText.text = selectedCount.ToString();
    }

    public int ReturnValue() // 정한 개수 반환
    {
        return selectedCount;
    }

    bool isDownPanelActive;
    public void EnableDownInfoPanel(string content, float duration = 3)
    {
        if (isDownPanelActive)
            return;
        StartCoroutine(EnableDownC(content, duration));
    }

    IEnumerator EnableDownC(string content, float duration)
    {
        isDownPanelActive = true;
        downPanelanimator.SetTrigger("Open");
        downInfoText.text = content;
        yield return new WaitForSeconds(duration);

        downPanelanimator.SetTrigger("Close");
        isDownPanelActive = false;
    }
}
