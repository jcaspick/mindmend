using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalVisual : MonoBehaviour
{
    public GameObject blueGoal;
    public GameObject redGoal;

    public Sprite blueGoalActive;
    public Sprite redGoalActive;

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
                break;
            case GameColor.Blue:
                blueGoal.GetComponent<SpriteRenderer>().sprite = blueGoalActive;
                break;
        }
    }

    public void SetColor(GameColor color)
    {
        switch (color) {
            case GameColor.Red:
                redGoal.SetActive(true);
                GameObject.Destroy(blueGoal);
                break;
            case GameColor.Blue:
                blueGoal.SetActive(true);
                GameObject.Destroy(redGoal);
                break;
        }
    }
}
