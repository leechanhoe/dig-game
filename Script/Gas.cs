using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gas : MonoBehaviour
{
    private void Destroy()
    {
        gameObject.SetActive(false);
    }
}
