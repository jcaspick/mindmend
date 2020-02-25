﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public GoalVisual visual;
    public GoalAudio audio;

    public Vector2Int gridCoordinates;
    public GameColor color;

    public void CreateVisuals(GoalVisual prefab)
    {
        if (!GameSettings.instance.USE_PLACEHOLDER_GOAL)
        {
            visual = Instantiate(prefab);
            visual.transform.position = new Vector3(gridCoordinates.x, gridCoordinates.y, 0);
            foreach (var renderer in GetComponentsInChildren<Renderer>())
            {
                renderer.enabled = false;
            }
        }
    }

    public void CreateAudio(GoalAudio prefab) {
        audio = prefab;
        audio = Instantiate(prefab);
        audio.transform.parent = GameObject.Find("Audio").transform;
        audio.transform.position = new Vector3(gridCoordinates.x, gridCoordinates.y, 0);
    }

    public void Achieve()
    {
        if (GameSettings.instance.USE_PLACEHOLDER_GOAL)
        {
            StartCoroutine(AchieveAnimation());
        } else
        {
            visual.Achieve(color);
            audio.Achieve();
        }
    }

    public void SetColor(GameColor color)
    {
        this.color = color;
        if (!GameSettings.instance.USE_PLACEHOLDER_GOAL)
        {
            visual.SetColor(color);
        } else
        {
            foreach (var renderer in GetComponentsInChildren<Renderer>())
            {
                if (color == GameColor.Red)
                    renderer.material = Resources.Load("red") as Material;
                else if (color == GameColor.Blue)
                    renderer.material = Resources.Load("blue") as Material;
            }
        }
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
