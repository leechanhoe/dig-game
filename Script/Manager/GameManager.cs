using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static public GameManager instance;
    public bool isPlaying { get; set; } // 게임중인가(메인메뉴제외)
    Player player;
    PlayerStat playerStat;
    Bound[] bounds;
    CameraManager cameraManager;
    MiniMapCamera miniMapCamera;
    DatabaseManager database;
    FadeManager fadeManager;
    PlayCanvas playCanvas;
    Equipment equipment;

    public enum GameMode{Normal, OpenInventory, OpenIllustratedBook, OpenInformationPanel, OpenProductionMethodPanel, OpenQuestUI
                        ,OpenDataSlot,OpenDifficultyUI,OpenTfMapUI}
    static public GameMode gameMode;

    [SerializeField] GameObject hpBar;
    [SerializeField] GameObject oxyBar;
    [SerializeField] GameObject expBar;
    [SerializeField] GameObject minimap;
    [SerializeField] GameObject joyStick;
    [SerializeField] GameObject quickSlots;
    [SerializeField] GameObject actButton;
    [SerializeField] GameObject subMenu;
    [SerializeField] GameObject levelText;

    private void Start()
    {
        if (instance == null) // 파괴방지
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
        else
            Destroy(this.gameObject);

        database = FindObjectOfType<DatabaseManager>();
        player = FindObjectOfType<Player>();
        playerStat = FindObjectOfType<PlayerStat>();
        fadeManager = FindObjectOfType<FadeManager>();
        playCanvas = FindObjectOfType<PlayCanvas>();
        equipment = FindObjectOfType<Equipment>();
    }

    // Update is called once per frame
    public void LoadStart(bool isBeginning = false)
    {
        StartCoroutine(LoadWaitCoroutine(isBeginning));
    }

    IEnumerator LoadWaitCoroutine(bool isBeginning) // 여기서 씬 넘어갈때 할거 하면됨
    {
        yield return new WaitForSeconds(0.2f);
        playerStat.AliveState();
        yield return new WaitUntil(() => FindObjectOfType<CameraManager>() != null);
        isPlaying = true;
        cameraManager = FindObjectOfType<CameraManager>();
        miniMapCamera = FindObjectOfType<MiniMapCamera>();

        Color color = player.GetComponent<SpriteRenderer>().color;
        color.a = 1f;
        player.GetComponent<SpriteRenderer>().color = color;
        player.ShowPlayerOnMinimap();
        player.transform.position = new Vector3(0, 1, player.transform.position.z);

        cameraManager.target = player.gameObject;
        miniMapCamera.target = cameraManager.target;

        yield return new WaitUntil(() => FindObjectOfType<Bound>() != null);
        bounds = FindObjectsOfType<Bound>();
        for (int i = 0; i < bounds.Length; i++)
        {
            if(bounds[i].isMainBound)
            {
                bounds[i].SetBound();
                break;
            }
        }

        if (isBeginning)
            database.BeginningSetting();
        else
        {
            database.LoadGround();
            database.LoadMineral();
            database.LoadChest();
            database.LoadRock();
            database.LoadMonster();

            database.CheckMineralNum();

            database.arrangeGround();
            database.arrangeMineral();
            database.arrangeChest();
            database.arrangeRock();
            database.arrangeMonster();
        }

        if (!hpBar.activeSelf)
        {
            hpBar.SetActive(true);
            oxyBar.SetActive(true);
            expBar.SetActive(true);
            minimap.SetActive(true);
            joyStick.GetComponent<JoyPad>().Transparency100();
            quickSlots.SetActive(true);
            actButton.SetActive(true);
            subMenu.SetActive(true);
            levelText.SetActive(true);
        }
        playerStat.UpdateScore();
        player.CanMove();

        playerStat.MineralGradeEffectOn();

        QuestManager.instance.LoadNpc();
        QuestManager.instance.NpcExclamationControl();

        player.GetComponentInChildren<Canvas>().worldCamera = cameraManager.GetComponent<Camera>();
        fadeManager.FadeIn();

        if(FindObjectOfType<Setting>().bgm.isOn)
            BGMManager.instance.Play(0);
        else if (!FindObjectOfType<Setting>().bgm.isOn)
            BGMManager.instance.Play(0,0);

        if(isBeginning && playerStat.stage.Equals(1))
            playCanvas.UITutorial();
        if (playerStat.stage > 0)
            player.CheckNearbyMonster();
        playCanvas.ExitAction();
        player.CanMove();
    }

    public void GoToMain()
    {
        StartCoroutine(GotoMainC());
    }

    IEnumerator GotoMainC()
    {
        fadeManager.FadeOut();
        yield return new WaitUntil(() => fadeManager.completedBlack);

        playerStat.AliveState();
        LoadingSceneManager.LoadScene("Main");
        isPlaying = false;
        playCanvas.ExitAction();
        hpBar.SetActive(false);
        oxyBar.SetActive(false);
        expBar.SetActive(false);
        minimap.SetActive(false);
        joyStick.GetComponent<JoyPad>().Transparency0();
        quickSlots.SetActive(false);
        actButton.SetActive(false);
        subMenu.GetComponent<SubMenu>().MenuClose(false);
        subMenu.SetActive(false);
        levelText.SetActive(false);

        player.transform.position = new Vector3(20f, 0.5f, player.transform.position.z);
        Color color = player.GetComponent<SpriteRenderer>().color;
        color.a = 0;
        player.GetComponent<SpriteRenderer>().color = color;

        yield return new WaitForSeconds(1f);
        yield return new WaitUntil(() => FindObjectOfType<CameraEffect>() != null);
        FindObjectOfType<CameraEffect>().GetComponent<AudioListener>().enabled = false;
        fadeManager.FadeIn();
        player.CantMove();

        equipment.RemoveAllEquipItem();
        Inventory.instance.RemoveAllItem();
        database.InitializeMineralNum();
        database.InitializeUpgradeReward();
        FindObjectOfType<Smithy>().InitializeProduction();
        QuestManager.instance.InitializeQuestData();
        playerStat.InitializeStat();
        SaveNLoad.instance.RemoveTemporaryData();

        BGMManager.instance.Stop();
    }

    public void MinimapUiOff()
    {
        minimap.SetActive(false);
    }

    public void MinimapUiOn()
    {
        minimap.SetActive(true);
    }
}