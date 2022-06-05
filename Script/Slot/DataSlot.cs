using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataSlot : MonoBehaviour
{
    public int slotIndex;
    [SerializeField] Text maxDepth;
    [SerializeField] Text level;
    [SerializeField] Text modeText;
    [SerializeField] Text playTime;
    [SerializeField] GameObject removeButton;
    [SerializeField] GameObject selected;
    [SerializeField] GameObject noData;
    // Start is called before the first frame update

    DataSlotUI dataSlotUI;
    private void Awake()
    {
        dataSlotUI = FindObjectOfType<DataSlotUI>();
    }
    public void UpdateSlot()
    {
        selected.SetActive(false);
        if (!PlayerPrefs.HasKey(slotIndex + "playTime"))
        {
            noData.SetActive(true);
            maxDepth.text = "";
            level.text = "";
            modeText.text = "";
            playTime.text = "";
            removeButton.SetActive(false);
            return;
        }

        noData.SetActive(false);
        maxDepth.text = "최대 깊이 : " + PlayerPrefs.GetInt(slotIndex + "maxDepth");
        level.text = "Level : " + PlayerPrefs.GetInt(slotIndex + "level");
        if (!Convert.ToBoolean(PlayerPrefs.GetString(slotIndex + "hardMode")))
        {
            modeText.text = "일반 모드";
            modeText.color = Color.black;
        }
        else
        {
            modeText.text = "하드 모드";
            modeText.color = new Color(188f / 255, 40f / 255, 40f / 255);
        }
        float _playTime = PlayerPrefs.GetFloat(slotIndex + "playTime");
        playTime.text = (int)(_playTime / 3600) + ":" + (((int)_playTime % 3600) / 60) + ":" + ((int)_playTime % 60);
        removeButton.SetActive(true);
    }

    public void TouchSlot()
    {
        SaveNLoad saveNLoad = SaveNLoad.instance;
        if (!selected.activeSelf)
        {
            for (int i = 0; i < dataSlotUI.dataSlots.Length; i++)
            {
                dataSlotUI.dataSlots[i].selected.SetActive(false);
            }
            selected.SetActive(true);
            saveNLoad.selectedSlot = slotIndex;
        }
        else
        {
            if (dataSlotUI.mode == 0)
                InformationPanel.instance.EnableOOC(Vector2.zero, slotIndex + "번 슬롯에 저장하시겠습니까?", "저장", "취소", saveNLoad.CallSave);
            else if (dataSlotUI.mode == 1 && !noData.activeSelf)
                InformationPanel.instance.EnableOOC(Vector2.zero, slotIndex + "번 슬롯을 불러오시겠습니까?", "로드", "취소", saveNLoad.CallLoad);
        }

    }

    public void TouchRemoveButton(int slotIndex)
    {
        SaveNLoad.instance.selectedSlot = slotIndex;
        InformationPanel.instance.EnableOOC(Vector2.zero, slotIndex + "번 슬롯을 삭제하시겠습니까?", "삭제", "취소", SaveNLoad.instance.RemoveData);
    }
}
