using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseUI;
    bool isPause;
    // Start is called before the first frame update

    private void Update()
    {
        if(Application.platform == RuntimePlatform.Android)
        {
            if (GameManager.gameMode == GameManager.GameMode.Normal && Input.GetKey(KeyCode.Escape) && !DatabaseManager.preventDoubleClick)
            {
                StartCoroutine(PreventDoubleClickC());
            }
        }
    }

    IEnumerator PreventDoubleClickC()
    {
        DatabaseManager.preventDoubleClick = true;
        if (!isPause)
            SetPause();
        else
            Continue();
        yield return new WaitForSeconds(0.1f);
        DatabaseManager.preventDoubleClick = false;
    }

    public void SetPause()
    {
        if (FindObjectOfType<PlayCanvas>().isTutorial)
            return;
        if(!isPause)
        {
            isPause = true;
            Time.timeScale = 0;
            pauseUI.SetActive(true);
        }
    }

    public void Continue()
    {
        if (isPause)
        {
            isPause = false;
            Time.timeScale = 1;
            pauseUI.SetActive(false);
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void MainMenu()
    {
        Continue();
        FindObjectOfType<GameManager>().GoToMain();
    }
}
