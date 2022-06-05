using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapSlot : MonoBehaviour
{
    public int stageIndex;
    public bool isOn { get; set; }

    [SerializeField] Sprite icon;
    [SerializeField] Sprite questionMark;
    [SerializeField] string depth;

    [SerializeField] GameObject selected;
    [SerializeField] GameObject currentStage; // 현재 스테이지는 약간 검어지게
    [SerializeField] Image image;
    [SerializeField] Text depthText;

    TransferMap transferMap;

    private void Awake()
    {
        transferMap = GetComponentInParent<TransferMap>();
    }
    // Start is called before the first frame update

    public void UpdateSlot()
    {
        if (stageIndex <= PlayerStat.instance.maxStage)
            isOn = true;
        if (isOn)
        {
            image.sprite = icon;
            depthText.text = depth + "m";
        }
        else
        {
            image.sprite = questionMark;
            depthText.text = "???m";
        }
        if (stageIndex == PlayerStat.instance.stage)
            currentStage.SetActive(true);
        else
            currentStage.SetActive(false);
        selected.SetActive(false);
    }

    public void TouchSlot()
    {
        if (currentStage.activeSelf)
            return;
        for (int i = 0; i < transferMap.mapSlots.Length; i++)
        {
            transferMap.mapSlots[i].selected.SetActive(false);
        }
        selected.SetActive(true);
        if (!isOn)
            return;

        InformationPanel.instance.EnableOOC(Vector2.zero, depth + "m로 이동하시겠습니까?", "이동", "취소",TransferMap);
    }

    void TransferMap()
    {
        StartCoroutine(TransferMapC());
    }

    IEnumerator TransferMapC()
    {
        FadeManager.instance.FadeOut();
        yield return new WaitUntil(() => FadeManager.instance.completedBlack);
        transferMap.CloseTfMapUI();
        LoadingSceneManager.LoadScene("Stage" + stageIndex);
    }
}
