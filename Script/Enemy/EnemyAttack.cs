using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public int atk;
    public float duration;

    private void OnEnable()
    {
        StartCoroutine(DisappearC());
    }

    

    IEnumerator DisappearC()
    {
        yield return new WaitForSeconds(duration);
        gameObject.SetActive(false);
        Debug.Log("꺼짐");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player.instance.Damaged((int)(atk * Random.Range(0.8f, 1.2f)));
            gameObject.SetActive(false);
        }
        //else if (collision.gameObject.CompareTag("Ground"))
        {
          //  gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        StopCoroutine(DisappearC());
    }
}
