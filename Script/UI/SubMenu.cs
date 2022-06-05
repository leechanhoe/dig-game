using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SubMenu : MonoBehaviour
{
    Animator anim;
    static public SubMenu instance;

    public GameObject menuButton;
    public Text menuButtonText;
    public GameObject[] subMenu;
    AudioManager audioManager;
    public SaveNLoad saveNLoad;
    public string soundEffect;

    private void Start()
    {
        instance = this;
        anim = GetComponent<Animator>();
        audioManager = FindObjectOfType<AudioManager>();
        saveNLoad = FindObjectOfType<SaveNLoad>();  
    }

    public void Save()
    {
        saveNLoad.CallSave();
    }

    public void Load()
    {
        saveNLoad.CallLoad();
    }

    public void MenuAction()
    {
        if (anim.GetBool("Opening")) // 메뉴가 열려있으면
            MenuClose(); // 메뉴닫음
        else // 아니면 반대
            MenuOpen();
    }

    public void MenuOpen()
    {
        if (FindObjectOfType<PlayCanvas>().isTutorial)
            return;
        audioManager.Play(soundEffect);
        anim.SetBool("Opening", true);
        for (int i = 0; i < subMenu.Length; i++)
            subMenu[i].SetActive(true);
        menuButtonText.text = "▶";
    }

    public void MenuClose(bool soundEffectOn = true)
    {
        if(soundEffectOn)
            audioManager.Play(soundEffect);
        StartCoroutine(MenuCloseC());
    }

    IEnumerator MenuCloseC()
    {
        anim.SetBool("Opening", false);
        yield return new WaitForSeconds(0.2f); // 서브메뉴들의 애니메이션이 다 끝난 다음에 서브메뉴 비활성화
        for (int i = 0; i < subMenu.Length; i++)
            subMenu[i].SetActive(false);
        menuButtonText.text = "◀";
    }
}
