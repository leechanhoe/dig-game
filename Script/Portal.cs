using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Transform destination;
    public Bound targetBound;
    public bool minimap;
    public bool stageMove;
    public int stage;
    [Tooltip("퀘스트와 관련있는 오브젝트면 해당 퀘스트인덱스를 입력")] public int questObject;
    public bool activeFalse;

    private void Start()
    {
        if (questObject > 0)
            QuestManager.instance.AddQuestObjInfo(questObject, gameObject);
        if (activeFalse)
            gameObject.SetActive(false);
    }

    public void ObjectControl() // 맵이동할때 추가로 실행해야할 함수들
    {

    }
}
