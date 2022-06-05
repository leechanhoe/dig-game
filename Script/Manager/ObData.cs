using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObData : MonoBehaviour
{
    public int id;
    public string npcName;
    public bool isHiddenNpc;
    [SerializeField] GameObject exclamation;
    [Tooltip("퀘스트와 관련있는 오브젝트면 해당 퀘스트인덱스를 입력")] public int questObject;

    private void Start()
    {
        if (questObject > 0)
            QuestManager.instance.AddQuestObjInfo(questObject, gameObject);
    }

    public void ExclamationMarkOn()
    {
        exclamation.SetActive(true);
    }

    public void ExclamationMarkOff()
    {
        exclamation.SetActive(false);
    }
    
}
