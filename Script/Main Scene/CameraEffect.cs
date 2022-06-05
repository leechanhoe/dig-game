using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffect : MonoBehaviour
{
    private Camera theCamera;
    private void Awake()
    {
        theCamera = GetComponent<Camera>();
        CameraResolution();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(0, -1 * Time.deltaTime, 0);
        if (transform.position.y < -50)
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    }

    void CameraResolution()
    {
        Rect rect = theCamera.rect;
        float scaleheight = ((float)Screen.width / Screen.height) / ((float)16 / 9);
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
}
