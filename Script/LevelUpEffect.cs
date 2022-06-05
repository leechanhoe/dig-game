using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpEffect : MonoBehaviour
{
    Animator animator;
    // Start is called before the first frame update
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        AudioManager.instance.Play("LevelUp");
        animator.SetTrigger("On");
    }

    void Destroy()
    {
        gameObject.SetActive(false);
    }
}
