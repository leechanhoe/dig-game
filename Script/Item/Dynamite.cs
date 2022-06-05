using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dynamite : Bomb
{
    Animator animatord;
    public GameObject target { get; set; }
    public bool mega;

    private void OnEnable()
    {
        StartCoroutine(OnEnableC());
    }

    IEnumerator OnEnableC()
    {
        yield return new WaitForEndOfFrame();
        animatord = GetComponent<Animator>();
        isExplode = false;
        usingBomb = true;
        AudioManager.instance.Play(dropSound);
        StartCoroutine(ReadyBoom());
    }

    IEnumerator ReadyBoom()
    {
        yield return new WaitForSeconds(readyTime);
            Explosion();
            isExplode = true;
    }
    void Explosion()
    {
        StartCoroutine(ExplosionC());
    }

    IEnumerator ExplosionC()
    {
        if (target != null)
        {
            if (target.GetComponent<Ground>().rock != null && target.GetComponent<Ground>().haveRock && !mega)
            {
                    target.GetComponent<Ground>().rock.SetActive(false);
                    target.SetActive(false);
            }
            else if(target.GetComponent<Ground>().rock != null && (target.GetComponent<Ground>().haveSpecialRock || target.GetComponent<Ground>().haveRock) && mega)
            {
                target.GetComponent<Ground>().rock.SetActive(false);
                target.SetActive(false);
            }
        }
        else
        {
            Collider2D[] colli = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
            for (int i = 0; i < colli.Length; i++)
            {
                if (colli[i].tag == "Monster")
                {
                    colli[i].GetComponent<Monster>().Damaged(power);
                    break;
                }
            }
        }

        AudioManager.instance.Play(ExplosionSound);
        animatord.SetBool("Boom", true);
        usingBomb = false;
        yield return new WaitForSeconds(1.25f);
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
