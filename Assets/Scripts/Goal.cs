using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public GoalVisual visual;

    public Vector2Int gridCoordinates;
    public GameColor color;

    public void Achieve()
    {
        //StartCoroutine(AchieveAnimation());

        visual.Achieve();
    }

    public void SetColor(GameColor color)
    {
        this.color = color;
        visual.SetColor(color);
    }

    IEnumerator AchieveAnimation()
    {
        float duration = 0.25f;
        float elapsed = 0.0f;

        Vector3 startSize = Vector3.one;
        Vector3 endSize = Vector3.one * 0.25f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startSize, endSize,
                Mathf.SmoothStep(0, 1, elapsed / duration));
            yield return null;
        }

        gameObject.SetActive(false);
    }
}
