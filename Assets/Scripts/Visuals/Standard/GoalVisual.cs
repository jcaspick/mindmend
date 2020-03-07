using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalVisual : GoalVisualBase
{
    public GameObject blueGoal;
    public GameObject redGoal;
    public GameObject purpleGoal;
    public GameObject greenGoal;
    public GameObject neutralGoal;

    public Sprite blueGoalActive;
    public Sprite redGoalActive;
    public Sprite purpleGoalActive;
    public Sprite greenGoalActive;

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
            case GameColor.Purple:
                purpleGoal.GetComponent<SpriteRenderer>().sprite = purpleGoalActive;
                purpleGoal.GetComponent<Flicker>();
                StartCoroutine(FadeOut(purpleGoal));

                break;
            case GameColor.Green:
                greenGoal.GetComponent<SpriteRenderer>().sprite = greenGoalActive;
                greenGoal.GetComponent<Flicker>();
                StartCoroutine(FadeOut(greenGoal));

                break;
        }
        StartCoroutine(Grow());
    }

    public override void SetColor(GameColor color)
    {
        neutralGoal.SetActive(false);

        switch (color) {
            case GameColor.Red:
                redGoal.SetActive(true);
                break;
            case GameColor.Blue:
                blueGoal.SetActive(true);
                break;
            case GameColor.Purple:
                purpleGoal.SetActive(true);
                break;
            case GameColor.Green:
                greenGoal.SetActive(true);
                break;
            case GameColor.Neutral:
                neutralGoal.SetActive(true);
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
