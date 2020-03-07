using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeVisual : NodeVisualBase
{
    public GameObject blueNode;
    public GameObject redNode;
    public GameObject purpleNode;
    public GameObject greenNode;
    public GameObject neutralNode;
    public GameObject inactiveNode;

    public Vector3 selectScale = new Vector3(0.1f, 0.1f, 0.1f);
    private Vector3 originalScale;

    public void Awake() {
        Reset();
        originalScale = transform.localScale;
    }

    public override void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    public override void SetColor(GameColor color) {
        Reset();

        switch (color) {
            case GameColor.Red:
                redNode.SetActive(true);
                break;
            case GameColor.Blue:
                blueNode.SetActive(true);
                break;
            case GameColor.Purple:
                purpleNode.SetActive(true);
                break;
            case GameColor.Green:
                greenNode.SetActive(true);
                break;
            case GameColor.Neutral:
                neutralNode.SetActive(true);
                break;
            default:
                inactiveNode.SetActive(true);
                break;
        }
    }

    public override void Select()
    {
        if (gameObject.transform.localScale == originalScale) {
            gameObject.transform.localScale = transform.localScale + selectScale;
        }
    }

    public override void Deselect()
    {
        gameObject.transform.localScale = originalScale;
    }

    private void LowHealth() {
        Flicker flicker = gameObject.AddComponent<Flicker>();
        flicker.minWaitTime = 0;
        flicker.maxWaitTime = 2.0f;
    }

    private void Reset() {
        foreach (Transform child in gameObject.transform) {
            child.gameObject.SetActive(false);
        }
    }
}
