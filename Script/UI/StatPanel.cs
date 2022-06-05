using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatPanel : MonoBehaviour
{
    public GameObject statPanel;
    public GameObject closeButton;
    public Text[] statText;
    const int ATK = 0, MOM = 1, CRI = 2, ATK_SPEED = 3, MAX_OXY = 4, SKILLING = 5, EXPLOSIVE_POWER = 6;

    PlayerStat playerStat;
    // Start is called before the first frame update
    void Start()
    {
        playerStat = FindObjectOfType<PlayerStat>();
    }

    public void ShowStatPanel()
    {
        statPanel.SetActive(true);
        closeButton.SetActive(true);
        AudioManager.instance.Play("TouchButton");
        ShowStatText();
    }

    public void CloseStatPanel()
    {
        statPanel.SetActive(false);
        closeButton.SetActive(false);
    }

    public void ShowStatText()
    {
        statText[ATK].text = playerStat.atk.ToString();
        statText[MOM].text = playerStat.momentum.ToString();
        statText[CRI].text = playerStat.cri + "%";
        statText[ATK_SPEED].text = playerStat.atkSpeed.ToString();
        statText[MAX_OXY].text = playerStat.speedX.ToString();
        statText[SKILLING].text = (int)(playerStat.skilling * 100) + "%";
        statText[EXPLOSIVE_POWER].text = playerStat.explosivePower.ToString();
    }

}
