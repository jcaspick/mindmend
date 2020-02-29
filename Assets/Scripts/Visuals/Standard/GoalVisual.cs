﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalVisual : GoalVisualBase
{
    public GameObject blueGoal;
    public GameObject redGoal;

    public Sprite blueGoalActive;
    public Sprite redGoalActive;

    public float maxScale;
    public float growFactor;
    public float waitTime;

    public float fadeSpeed = 0.5f;

    public void Awake() {
        foreach (Transform child in gameObject.transform) {
            child.gameObject.SetActive(false);
        }
    }

    public override void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    public override void Achieve(GameColor color)
    {
        switch (color) {
            case GameColor.Red:
                redGoal.GetComponent<SpriteRenderer>().sprite = redGoalActive;
                redGoal.GetComponent<Flicker>();
                StartCoroutine(FadeOut(redGoal));

                break;
            case GameColor.Blue:
                blueGoal.GetComponent<SpriteRenderer>().sprite = blueGoalActive;
                blueGoal.GetComponent<Flicker>();
                StartCoroutine(FadeOut(blueGoal));

                break;
        }
        StartCoroutine(Grow());
    }

    public override void SetColor(GameColor color)
    {
        switch (color) {
            case GameColor.Red:
                redGoal.SetActive(true);
                break;
            case GameColor.Blue:
                blueGoal.SetActive(true);
                break;
        }
    }

    private IEnumerator Grow() {
        while (maxScale > transform.localScale.x) {
            gameObject.transform.localScale += new Vector3(1, 1, 1) * Time.deltaTime * growFactor;

            yield return null;
        }
        yield return new WaitForSeconds(waitTime);
    }

    private IEnumerator FadeOut(GameObject goal) {
        for (float f = 0.0f; f < 1.0f; f += Time.deltaTime / fadeSpeed) {
            goal.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 1.0f - f);
            yield return null;
        }
    }

}