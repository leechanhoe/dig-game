using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypeEffect : MonoBehaviour
{
    public string targetMsg;
    public float CharPerSecond;
    public GameObject EndCursor;
    public GameObject touchArea;
    public bool isAnim;

    public Text msgText;
    AudioSource audioSource;

    int index;
    float interval;

    void Awake()
    {
        msgText = GetComponent<Text>();
        audioSource = GetComponent<AudioSource>();
    }

    public void SetMsg(string msg)
    {
        if (isAnim)
        {
            EffectEnd();
            msgText.text = targetMsg;
            CancelInvoke();
        }
        else
        {
            targetMsg = msg;
            EffectStart();
        }
    }

    void EffectStart()
    {
        msgText.text = "";
        index = 0;
        EndCursor.SetActive(false);

        //애니 스타트
        interval = 1.0f / CharPerSecond;

        isAnim = true;

        Invoke("Effecting", interval);
    }

    public void Effecting()
    {
        //글자추가 애니
        if (msgText.text == targetMsg)
        {
            EffectEnd();
            return;
        }
        msgText.text += targetMsg[index];
        //사운드
        //if (targetMsg[index] != ' ' || targetMsg[index] != '.' && index % 3 == 0)
            //audioSource.Play();

        index++;
        //재귀
        Invoke("Effecting", 1 / CharPerSecond);
    }

    void EffectEnd()
    {
        isAnim = false;
        touchArea.SetActive(true);
        EndCursor.SetActive(true);
    }
}
