using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalVisual : MonoBehaviour
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

    public void Achieve(GameColor color)
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
        StartCoroutine("Grow");
    }

    public void SetColor(GameColor color)
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

    public IEnumerator Grow() {
        float timer = 0;

        while (true)
        {
            while (maxScale > transform.localScale.x) {
                timer += Time.deltaTime;
                gameObject.transform.localScale += new Vector3(1, 1, 1) * Time.deltaTime * growFactor;

                yield return null;
            }
            yield return new WaitForSeconds(waitTime);
        }
    }

    public IEnumerator FadeOut(GameObject goal) {
        // fade out
        for (float f = 0.0f; f < 1.0f; f += Time.deltaTime / fadeSpeed) {
            goal.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 1.0f - f);
            yield return null;
        }
    }

}
