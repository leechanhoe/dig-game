using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    static public FadeManager instance;
    public GameObject blackObject;
    public GameObject whiteObject;
    public Image white;
    public Image black;
    public bool completedBlack = false;
    public bool noLoading;

    private Color color;

    private WaitForSeconds waitTime = new WaitForSeconds(0.01f);

    private void Awake()
    {
        if (instance == null) // 파괴방지
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
        else
            Destroy(this.gameObject);
    }

    public void ImageGraduallyAppear(Image image, float speed = 0.02f)
    {
        StartCoroutine(ImageGraduallyAppearC(image, speed));
    }

    IEnumerator ImageGraduallyAppearC(Image image, float speed)
    {
        for (float i = 0f; i < 1f; i += speed)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, i);
            yield return waitTime;
        }
    }

    public void ImageGraduallyDisAppear(Image image, float speed = 0.02f)
    {
        StartCoroutine(ImageGraduallyAppearC(image, speed));
    }

    IEnumerator ImageGraduallyDisAppearC(Image image, float speed)
    {
        for (float i = 1f; i > 0f; i -= speed)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, i);
            yield return waitTime;
        }
    }

    public void SetActiveFalse()
    {
        blackObject.SetActive(false);
    }

    public void FadeOut(float _speed = 0.02f)
    {
        StartCoroutine(FadeOutCoroutine(_speed));
    }
    IEnumerator FadeOutCoroutine(float _speed)
    {
        completedBlack = false;
        blackObject.SetActive(true);
        color = black.color;
        if (noLoading)
            color.a = 0.98f;

        while(color.a < 1f)
        {
            color.a += _speed;
            black.color = color;
            yield return waitTime;
        }
        color.a = 1f;
        black.color = color;
        completedBlack = true;
    }

    public void FadeIn(float _speed = 0.02f)
    {
        StopAllCoroutines();
        StartCoroutine(FadeInCoroutine(_speed));
    }

    IEnumerator FadeInCoroutine(float _speed)
    {
        blackObject.SetActive(true);
        completedBlack = false;
        color = black.color;
        if (noLoading)
            color.a = 0.02f;

        while (color.a > 0f)
        {
            color.a -= _speed;
            black.color = color;
            yield return waitTime;
        }
        color.a = 0f;
        black.color = color;
        blackObject.SetActive(false);
    }

    public void FadeOutIn(float _speed = 0.02f, float _OutTime = 1f, bool canMove = true)
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutInCoroutine(_speed, _OutTime, canMove));
    }

    IEnumerator FadeOutInCoroutine(float _speed, float _OutTime, bool canMove)
    {
        blackObject.SetActive(true);
        color = black.color;
        if (noLoading)
            color.a = 0.98f;

        while (color.a < 1f)
        {
            color.a += _speed;
            black.color = color;
            yield return waitTime;
        }

        completedBlack = true;
        yield return new WaitForSeconds(_OutTime);
        completedBlack = false;
                if (noLoading)
            color.a = 0.02f;

        while (color.a > 0f)
        {
            color.a -= _speed;
            black.color = color;
            yield return waitTime;
        }
        blackObject.SetActive(false);
    }

    public void FlashOut(float _speed = 0.02f)
    {
        StopAllCoroutines();
        StartCoroutine(FlashOutCoroutine(_speed));
    }
    IEnumerator FlashOutCoroutine(float _speed)
    {
        color = white.color;

        while (color.a < 1f)
        {
            color.a += _speed;
            white.color = color;
            yield return waitTime;
        }
    }

    public void FlashIn(float _speed = 0.02f)
    {
        StopAllCoroutines();
        StartCoroutine(FlashInCoroutine(_speed));
    }
    IEnumerator FlashInCoroutine(float _speed)
    {
        color = white.color;

        while (color.a > 0f)
        {
            color.a -= _speed;
            white.color = color;
            yield return waitTime;
        }
    }

    public void Flash(float _speed = 0.1f) // 번개같이 순간 깜빡임
    {
        StopAllCoroutines();
        StartCoroutine(FlashCoroutine(_speed));
    }

    IEnumerator FlashCoroutine(float _speed)
    {
        color = white.color;

        while (color.a < 1f)
        {
            color.a += _speed;
            white.color = color;
            yield return waitTime;
        }

        while (color.a > 0f)
        {
            color.a -= _speed;
            white.color = color;
            yield return waitTime;
        }
    }
}
