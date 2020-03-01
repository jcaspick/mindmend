using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimalGoalVisual : GoalVisualBase
{
    public override void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    public override void Achieve(GameColor color)
    {
        StartCoroutine(AchieveAnimation());
    }

    public override void SetColor(GameColor color)
    {
        foreach (var renderer in GetComponentsInChildren<Renderer>())
        {
            switch (color)
            {
                case GameColor.Red:
                    renderer.material = Resources.Load("red") as Material;
                    break;
                case GameColor.Blue:
                    renderer.material = Resources.Load("blue") as Material;
                    break;
                case GameColor.Green:
                    renderer.material = Resources.Load("green") as Material;
                    break;
                case GameColor.Purple:
                    renderer.material = Resources.Load("purple") as Material;
                    break;
                default:
                    renderer.material = Resources.Load("grey") as Material;
                    break;
            }
        }
    }

    private IEnumerator AchieveAnimation()
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
