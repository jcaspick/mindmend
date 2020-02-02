using System.Collections;
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

    public IEnumerator ShowMemory(GameColor color, int i)
    {
        StopAllCoroutines();

        var sprite = color == GameColor.Red ?
            redMemories[i] :
            blueMemories[i];
        memoryImage.sprite = sprite;

        yield return StartCoroutine(MemoryFadeIn(0.5f));
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(MemoryFadeOut(3.0f));
    }

    public IEnumerator ShowFinalMemory()
    {
        StopAllCoroutines();
        memoryImage.sprite = finalMemory;

        yield return StartCoroutine(MemoryFadeIn(2.5f));
        yield return new WaitForSeconds(5.0f);
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
}
