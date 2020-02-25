using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeVisual : MonoBehaviour
{
    // Order in array represents layer order
    public GameObject blueNode;
    public GameObject redNode;
    public GameObject neutralNode;
    public GameObject inactiveNode;

    public Vector3 selectScale = new Vector3(0.1f, 0.1f, 0.1f);
    private Vector3 originalScale;

    public void Awake() {
        Reset();
        originalScale = transform.localScale;
    }

    public void SetColor(GameColor color) {
        Reset();

        switch (color) {
            case GameColor.Red:
                redNode.SetActive(true);
                break;
            case GameColor.Blue:
                blueNode.SetActive(true);
                break;
            case GameColor.Neutral:
                neutralNode.SetActive(true);
                break;
            default:
                inactiveNode.SetActive(true);
                break;
        }
    }

    public void LowHealth() {
        Flicker flicker = gameObject.AddComponent<Flicker>();
        flicker.minWaitTime = 0;
        flicker.maxWaitTime = 2.0f;
    }

    public void Reset() {
        foreach (Transform child in gameObject.transform) {
            child.gameObject.SetActive(false);
        }
    }

    public void Select()
    {
        if (gameObject.transform.localScale == originalScale) {
            gameObject.transform.localScale = transform.localScale + selectScale;
        }
    }

    public void Deselect()
    {
        gameObject.transform.localScale = originalScale;
    }

}
