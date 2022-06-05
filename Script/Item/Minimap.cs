using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    public GameObject minimapUI;
    static public GameObject minimap;

    private void Start()
    {
        minimap = minimapUI;
    }

    static public void MinimapOn()
    {
        minimap.SetActive(true);
    }

    static public void MinimapOff()
    {
        minimap.SetActive(false);
    }
}
