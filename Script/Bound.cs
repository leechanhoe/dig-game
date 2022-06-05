using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bound : MonoBehaviour
{
    public bool isMainBound;
    private BoxCollider2D bound;
    public string boundName;// 나중에 불러올 때 바운드도 따라와야 카메라가 따라오기 때문
    private CameraManager theCamera;
    MiniMapCamera miniMapCamera;

    void Start()
    {
        bound = GetComponent<BoxCollider2D>();
        theCamera = CameraManager.instance;
        miniMapCamera = MiniMapCamera.instance;
    }

    public void SetBound()
    {
        if(theCamera != null)
        {
            theCamera.SetBound(bound);
        }
        if(miniMapCamera != null)
        {
            miniMapCamera.SetBound(bound);
        }
    }
}
