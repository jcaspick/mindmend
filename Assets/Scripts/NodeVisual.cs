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

    private GameObject activeNode;

    public void Awake() {
        Reset();
        originalScale = transform.localScale;
    }

    public void SetColor(GameColor color) {
        Reset();

        switch (color) {
            case GameColor.Red:
                redNode.SetActive(true);
                activeNode = redNode;
                break;
            case GameColor.Blue:
                blueNode.SetActive(true);
                activeNode = blueNode;
                break;
            case GameColor.Neutral:
                neutralNode.SetActive(true);
                activeNode = neutralNode;
                break;
            default:
                inactiveNode.SetActive(true);
                activeNode = inactiveNode;
                break;
        }
    }

    public void LowHealth() {
        GameObject mainNode = activeNode.transform.GetChild(0).gameObject;

        Flicker flicker = mainNode.AddComponent<Flicker>();
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
        gameObject.transform.localScale = transform.localScale + selectScale;
    }

    public void Deselect()
    {
        gameObject.transform.localScale = originalScale;
    }

}
