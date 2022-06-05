using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    Animator animator;
    public float readyTime = 3; // 폭탄이 터지기까지 시간
    public float explosionRadius;
    public int power; // 폭탄세기
    public string dropSound;
    public string ExplosionSound;
    protected bool isExplode;

    static public bool usingBomb;
    private void OnEnable()
    {
        StartCoroutine(OnEnableC());
    }

    IEnumerator OnEnableC()
    {
        yield return new WaitForEndOfFrame();
        animator = GetComponent<Animator>();
        isExplode = false;
        usingBomb = true;
        AudioManager.instance.Play(dropSound);
        StartCoroutine(readyBoomC());
    }

    IEnumerator readyBoomC()
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
        Collider2D[] colli = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        for (int i = 0; i < colli.Length; i++)
        {
            if (colli[i].tag == "Monster")
            {
                if (colli[i].GetComponent<Monster>().isBoss && colli.Length > 1) // 두마리 동시에맞을때 잡몹부터 맞게하기위함
                    continue;
                PlayerStat playerStat = FindObjectOfType<PlayerStat>();
                float dmg = (power + playerStat.explosivePower) * Random.Range(playerStat.skilling, 1.1f);
                colli[i].GetComponent<Monster>().Damaged((int)dmg);
                break;
            }
        }

        AudioManager.instance.Play(ExplosionSound);
        animator.SetBool("Boom", true);
        usingBomb = false;
        yield return new WaitForSeconds(1.25f);
        gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
