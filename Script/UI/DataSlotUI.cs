using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataSlotUI : MonoBehaviour
{
    [SerializeField] GameObject dataSlotUI;
    [SerializeField] GameObject preventOtherTouch;
    public DataSlot[] dataSlots;

    public int mode { get; set; }
    const int SAVE_MODE = 0, LOAD_Mode = 1;
    // Start is called before the first frame update
    private void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (GameManager.gameMode == GameManager.GameMode.OpenDataSlot && Input.GetKey(KeyCode.Escape) && !DatabaseManager.preventDoubleClick)
            {
                StartCoroutine(PreventDoubleClickC());
            }
        }
    }

    IEnumerator PreventDoubleClickC()
    {
        DatabaseManager.preventDoubleClick = true;
        CloseDataSlotUI();
        yield return new WaitForSeconds(0.1f);
        DatabaseManager.preventDoubleClick = false;
    }


    GameManager.GameMode prevGameMode;
    
    /// <summary>
    /// 0은 저장모드, 1은 로드모드
    /// </summary>
    public void OpenDataSlotUI(int mode)
    {
        if (dataSlotUI.activeSelf)
            return;
        this.mode = mode;
        prevGameMode = GameManager.gameMode;
        GameManager.gameMode = GameManager.GameMode.OpenDataSlot;
        dataSlotUI.SetActive(true);
        preventOtherTouch.SetActive(true);
        ShowSlot();
    }

    public void CloseDataSlotUI()
    {
        if (!dataSlotUI.activeSelf)
            return;
        GameManager.gameMode = prevGameMode;
        dataSlotUI.SetActive(false);
        preventOtherTouch.SetActive(false);
    }

    public void ShowSlot()
    {
        if (!dataSlotUI.activeSelf)
            return;

        for (int i = 0; i < dataSlots.Length; i++)
            dataSlots[i].UpdateSlot();
    }
}
