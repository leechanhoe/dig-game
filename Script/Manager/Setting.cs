using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    public GameObject settingPanel;
    public Toggle soundEffect;
    public Toggle bgm;

    AudioManager audioManager;
    BGMManager bGMManager;

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        bGMManager = FindObjectOfType<BGMManager>();

        if (PlayerPrefs.HasKey("isBGMOn"))
        {
            soundEffect.isOn = Convert.ToBoolean(PlayerPrefs.GetString("isSoundEffectOn"));
            bgm.isOn = Convert.ToBoolean(PlayerPrefs.GetString("isBGMOn"));
            BGMControl();
            SoundEffectControl();
        }
    }

    public void EnableSettingPanel()
    {
        settingPanel.SetActive(true);
    }

    public void DisEnableSettingPanel()
    {
        settingPanel.SetActive(false);
    }

    public void SoundEffectControl()
    {
        if (soundEffect.isOn)
            audioManager.SetAllClipVolumnOn();
        else if (!soundEffect.isOn)
            audioManager.SetAllClipVolumnOff();
    }

    float bgmVolumn;
    public void BGMControl()
    {
        if (bgm.isOn)
            bGMManager.SetVolumn(bgmVolumn);
        else if (!bgm.isOn)
        {
            bgmVolumn = bGMManager.GetVolumnScale();
            bGMManager.SetVolumn(0);
        }
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetString("isSoundEffectOn", soundEffect.isOn.ToString());
        PlayerPrefs.SetString("isBGMOn", bgm.isOn.ToString());
    }
}
