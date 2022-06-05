using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransferMap : MonoBehaviour
{
    [SerializeField] GameObject TfMapUI;
    public MapSlot[] mapSlots;
    private void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (GameManager.gameMode == GameManager.GameMode.OpenTfMapUI && Input.GetKey(KeyCode.Escape) && !DatabaseManager.preventDoubleClick)
            {
                StartCoroutine(PreventDoubleClickC());
            }
        }
    }

    IEnumerator PreventDoubleClickC()
    {
        DatabaseManager.preventDoubleClick = true;
        CloseTfMapUI();
        yield return new WaitForSeconds(0.1f);
        DatabaseManager.preventDoubleClick = false;
    }


    GameManager.GameMode prevGameMode;

    public void OpenTfMapUI()
    {
        if (TfMapUI.activeSelf)
            return;
        prevGameMode = GameManager.gameMode;
        GameManager.gameMode = GameManager.GameMode.OpenTfMapUI;
        TfMapUI.SetActive(true);
        ShowSlot();
    }

    public void CloseTfMapUI()
    {
        if (!TfMapUI.activeSelf)
            return;
        GameManager.gameMode = prevGameMode;
        TfMapUI.SetActive(false);
    }

    public void ShowSlot()
    {
        if (!TfMapUI.activeSelf)
            return;

        for (int i = 0; i < mapSlots.Length; i++)
            mapSlots[i].UpdateSlot();
    }
}
