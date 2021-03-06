﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Controller : MonoBehaviour
{
    public static UI_Controller instance;

    public CanvasGroup memoryFader;
    public CanvasGroup blackScreenFader;
    public GameObject memoryDisplay;
    public Image memoryImage;
    public CanvasGroup redTurnFader;
    public CanvasGroup blueTurnFader;
    public CanvasGroup purpleTurnFader;
    public CanvasGroup greenTurnFader;

    public List<Sprite> redMemories;
    public List<Sprite> blueMemories;
    public Sprite finalMemory;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        EventManager.AddListener(EventType.PlayerChange, SetTurnDisplay);
    }

    public IEnumerator ShowMemory(GameColor color, int i)
    {
        StopAllCoroutines();

        var sprite = color == GameColor.Red ?
            redMemories[i] :
            blueMemories[i];
        memoryImage.sprite = sprite;

        yield return StartCoroutine(MemoryFadeIn(0.5f));
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(MemoryFadeOut(2.5f));
    }

    public IEnumerator ShowFinalMemory()
    {
        StopAllCoroutines();
        memoryImage.sprite = finalMemory;

        yield return StartCoroutine(FadeToBlack(4.5f));
    }

    public IEnumerator FadeToBlack(float duration)
    {
        float elapsed = 0;
        float redStart = redTurnFader.alpha;
        float blueStart = blueTurnFader.alpha;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / duration);
            blackScreenFader.alpha = Mathf.Lerp(0.0f, 1.0f, t);
            redTurnFader.alpha = Mathf.Lerp(redStart, 0.0f, t);
            blueTurnFader.alpha = Mathf.Lerp(blueStart, 0.0f, t);
            memoryFader.alpha = Mathf.Lerp(0.0f, 1.0f, t);
            yield return null;
        }
    }

    public IEnumerator FinalMemoryFadeIn(float duration)
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / duration);
            memoryFader.alpha = Mathf.Lerp(0.0f, 1.0f, t);
            yield return null;
        }
    }

    public IEnumerator MemoryFadeIn(float duration)
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / duration);
            memoryFader.alpha = Mathf.Lerp(0.0f, 1.0f, t);
            blackScreenFader.alpha = Mathf.Lerp(0.0f, 0.65f, t);
            yield return null;
        }
    }

    public IEnumerator MemoryFadeOut(float duration)
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / duration);
            memoryFader.alpha = Mathf.Lerp(1.0f, 0.0f, t);
            blackScreenFader.alpha = Mathf.Lerp(0.65f, 0.0f, t);
            yield return null;
        }
    }

    public void SetTurnDisplay(EventDetails details)
    {
        var color = details.color;
        if (color == GameColor.Red)
            StartCoroutine(TweenTurnDisplay(1.0f, 0.0f, 0.0f, 0.0f));
        else if (color == GameColor.Blue)
            StartCoroutine(TweenTurnDisplay(0.0f, 1.0f, 0.0f, 0.0f));
        else if (color == GameColor.Purple)
            StartCoroutine(TweenTurnDisplay(0.0f, 0.0f, 1.0f, 0.0f));
        else if (color == GameColor.Green)
            StartCoroutine(TweenTurnDisplay(0.0f, 0.0f, 0.0f, 1.0f));
    }

    IEnumerator TweenTurnDisplay(float redEnd, float blueEnd, float purpleEnd, float greenEnd)
    {
        float elapsed = 0.0f;
        float duration = 0.75f;
        float redStart = redTurnFader.alpha;
        float blueStart = blueTurnFader.alpha;
        float purpleStart = purpleTurnFader.alpha;
        float greenStart = greenTurnFader.alpha;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / duration);
            redTurnFader.alpha = Mathf.Lerp(redStart, redEnd, t);
            blueTurnFader.alpha = Mathf.Lerp(blueStart, blueEnd, t);
            purpleTurnFader.alpha = Mathf.Lerp(purpleStart, purpleEnd, t);
            greenTurnFader.alpha = Mathf.Lerp(greenStart, greenEnd, t);
            yield return null;
        }
    }
}
