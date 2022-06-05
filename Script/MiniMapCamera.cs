using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapCamera : MonoBehaviour
{
    static public MiniMapCamera instance;

    public GameObject target;
    public float moveSpeed;
    Vector3 targetPosition;

    public BoxCollider2D bound;

    Vector3 minBound; //카메라 맵 밖 탈출제한
    Vector3 maxBound;

    private Camera theCamera;

    //박스 콜라이더 영역의 최소최대 xyz값을 지님.

    private float halfWidth;
    private float halfHeight;

    private void Awake()
    {
        if (instance == null) // 파괴방지
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
        else
            Destroy(this.gameObject);
        theCamera = GetComponent<Camera>();
    }
    void CameraResolution()
    {
        Rect rect = theCamera.rect;
        float scaleheight = ((float)Screen.width / Screen.height) / ((float)74 / 51);
        float scalewidth = 1f / scaleheight;
        if (scaleheight < 1)
        {
            rect.height = scaleheight;
            rect.y = (1f - scaleheight) / 2f;
        }
        else
        {
            rect.width = scalewidth;
            rect.x = (1f - scalewidth) / 2f;
        }
        theCamera.rect = rect;
    }

    void Start()
    {
        minBound = bound.bounds.min;
        maxBound = bound.bounds.max;
        halfHeight = theCamera.orthographicSize;
        halfWidth = halfHeight;
    }

    // Update is called once per frame
    void Update()
    {
        if (target.gameObject != null)
        {
            targetPosition.Set(target.transform.position.x, target.transform.position.y, this.transform.position.z);
            this.transform.position = Vector3.Lerp(this.transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }

        float clampedX = Mathf.Clamp(this.transform.position.x, minBound.x + halfWidth, maxBound.x - halfWidth);
        //(10,0,100) ㅡ> 왼쪽값이 0과 100사이안에 들으면 왼쪽값 리턴,0보다 작음 0리턴 , 100보다 크면 100리턴
        float clampedY = Mathf.Clamp(this.transform.position.y, minBound.y + halfHeight, maxBound.y - halfHeight);
        this.transform.position = new Vector3(clampedX, clampedY, this.transform.position.z);
    }

    public void SetBound(BoxCollider2D newBound)
    {
        bound = newBound;
        minBound = bound.bounds.min;
        maxBound = bound.bounds.max;
    }
}
