using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingSceneManager : MonoBehaviour
{
    public Image loadingBar;
    static string currentSceneName;
    static string nextSceneName;
    static bool isBeginning;
    static int stageIndex;

    static string[] stages = new string[6] {"Main","Stage1","Stage2","Stage3","Stage4","Stage5" };
    void Start()
    {
        loadingBar.fillAmount = 0;
        StartCoroutine(LoadAsyncScene());
    }

    IEnumerator LoadAsyncScene()
    {
        if (nextSceneName.Contains("Stage"))
            PlayerStat.instance.ChangeStage(stageIndex);
        else if (nextSceneName.Equals("Main"))
            PlayerStat.instance.ChangeStage(0);
        yield return null;
        AsyncOperation asyncScene = SceneManager.LoadSceneAsync(nextSceneName);
        asyncScene.allowSceneActivation = false;
        float timeC = 0;
        while(!asyncScene.isDone)
        {
            yield return null;
            timeC += Time.deltaTime;
            if(asyncScene.progress >= 0.9f)
            {
                loadingBar.fillAmount = Mathf.Lerp(loadingBar.fillAmount, 1, timeC);
                if (loadingBar.fillAmount == 1.0f) // 로딩 후 다룰것을 여기에 놓아야함
                {
                    asyncScene.allowSceneActivation = true;
                    if (isBeginning)                  
                        GameManager.instance.LoadStart(isBeginning);
                    else if (!isBeginning && nextSceneName.Contains("Stage"))
                        GameManager.instance.LoadStart();
                    break;
                }
            }
            else
            {
                loadingBar.fillAmount = Mathf.Lerp(loadingBar.fillAmount, asyncScene.progress, timeC);
                if (loadingBar.fillAmount >= asyncScene.progress)
                    timeC = 0f;
            }
        }
    }

    static public void LoadScene(string nextSceneName, bool isBeginning = false)
    {
        Player.instance.CantMove();
        currentSceneName = stages[PlayerStat.instance.stage];
        if (currentSceneName.Equals(nextSceneName))
        {
            Debug.LogError("이동하려는 맵이 같음");
            return;
        }
        LoadingSceneManager.nextSceneName = nextSceneName;
        LoadingSceneManager.isBeginning = isBeginning;

       /* if(PlayerStat.instance.stage > 0)
        {
            for (int i = 0; i < DatabaseManager.instance.monsters[PlayerStat.instance.stage].Length; i++)
                DatabaseManager.instance.monsters[PlayerStat.instance.stage][i].gameObject.SetActive(true);
        }*/

        if (nextSceneName.Contains("Stage"))
        {
            stageIndex = int.Parse(nextSceneName.Split('e')[1]);

            if (PlayerStat.instance.maxStage < stageIndex) // 만약 안가본 스테이지면
                LoadingSceneManager.isBeginning = true;

            if (PlayerStat.instance.visitedStage.Contains(stageIndex) && !currentSceneName.Equals(stages[0])) // 맵이동하면서 임시 데이터 저장
                SaveNLoad.instance.LoadTemporaryMapData(stageIndex);
            if (!currentSceneName.Equals(stages[0]))
                SaveNLoad.instance.SaveTemporaryMapData(PlayerStat.instance.stage);
        }
        SceneManager.LoadScene("Loading");
    }
}
