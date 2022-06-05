using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainScene : MonoBehaviour
{
    public GameObject pressText;
    public GameObject pressToStart;
    public GameObject mainMenu;
    public GameObject notLoad; // 저장을 한번도 안했으면 로드 막기
    public string click_sound;

    public GameObject hpBar;
    public GameObject oxyBar;
    public GameObject minimap;
    public GameObject joyStick;
    public GameObject quickSlots;
    public GameObject actButton;
    public GameObject subMenu;
    public GameObject scoreText;

    [SerializeField] GameObject difficultytUI;
    [SerializeField] GameObject[] selectedDifficulty;
    [SerializeField] Text difficultyExplain;

    SaveNLoad saveNLoad;
    const int NORMAL = 0, HARD = 1;
    int selectedMode;
    // Start is called before the first frame update
    void Start()
    {
        saveNLoad = FindObjectOfType<SaveNLoad>();
    }
    private void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (GameManager.gameMode == GameManager.GameMode.OpenDifficultyUI && Input.GetKey(KeyCode.Escape) && !DatabaseManager.preventDoubleClick)
            {
                StartCoroutine(PreventDoubleClickC());
            }
        }
    }

    IEnumerator PreventDoubleClickC()
    {
        CloseDifficultyUI();
        yield return new WaitForSeconds(0.1f);
        DatabaseManager.preventDoubleClick = false;
    }
    public void PressToStart()
    {
        pressText.SetActive(false);
        pressToStart.SetActive(false);
        mainMenu.SetActive(true);
        if (PlayerPrefs.HasKey("playerX"))
            notLoad.SetActive(false);
        else
            notLoad.SetActive(true);
    }

    GameManager.GameMode prevGameMode;
    public void StartGameButton()
    {
        OpenDifficultyUI();
    }

    public void GameStart()
    {
        StartCoroutine(StartGameC());
    }

    IEnumerator StartGameC()
    {
        CloseDifficultyUI();
        AudioManager.instance.Play(click_sound);
        FadeManager fadeManager = FindObjectOfType<FadeManager>();
        fadeManager.FadeOut();
        yield return new WaitUntil(() => fadeManager.completedBlack);

        if (selectedMode == HARD)
            PlayerStat.instance.hardMode = true;
        else
            PlayerStat.instance.hardMode = false;

        LoadingSceneManager.LoadScene("Stage1", true);
    }

    public void Load()
    {
        AudioManager.instance.Play(click_sound);
        saveNLoad.GetComponentInChildren<DataSlotUI>().OpenDataSlotUI(1);
    }

    public void Quit()
    {
        AudioManager.instance.Play(click_sound);
        Application.Quit();
    }

    public void ChooseDifficultyMode(int mode)
    {
        if(mode == NORMAL)
        {
            selectedDifficulty[HARD].SetActive(false);
            if (!selectedDifficulty[NORMAL].activeSelf)
            {
                difficultyExplain.text = "보통의 난이도입니다.";
                selectedDifficulty[NORMAL].SetActive(true);
                selectedMode = NORMAL;
            }
            else if (selectedDifficulty[NORMAL].activeSelf)
                InformationPanel.instance.EnableOOC(Vector2.zero, "일반 모드로 플레이하시겠습니까?", "플레이", "취소", GameStart);
            
        }
        else if(mode == HARD)
        {
            selectedDifficulty[NORMAL].SetActive(false);
            if (!selectedDifficulty[HARD].activeSelf)
            {
                difficultyExplain.text = "하드 모드는 일반 모드에 비해\n산소 감소량이 더 높고 \n아이템의 재사용 대기시간이 더 깁니다.\n\n또한 캐릭터 사망시 부활의 돌을 소지하고 있지 않으면 \n해당 플레이의 저장 데이터가 영구히 사라집니다.";
                selectedDifficulty[HARD].SetActive(true);
                selectedMode = HARD;
            }
            else if (selectedDifficulty[HARD].activeSelf)
                InformationPanel.instance.EnableOOC(Vector2.zero, "하드 모드로 플레이하시겠습니까?", "플레이", "취소", GameStart);
        }
    }

    public void OpenDifficultyUI()
    {
        if (!difficultytUI.activeSelf)
        {
            difficultyExplain.text = "모드를 선택하세요.";
            difficultytUI.SetActive(true);
            selectedDifficulty[NORMAL].SetActive(false);
            selectedDifficulty[HARD].SetActive(false);
            prevGameMode = GameManager.gameMode;
            GameManager.gameMode = GameManager.GameMode.OpenDifficultyUI;
        }
    }

    public void CloseDifficultyUI()
    {
        if (difficultytUI.activeSelf)
        {
            difficultytUI.SetActive(false);
            GameManager.gameMode = prevGameMode;
        }
    }
}
