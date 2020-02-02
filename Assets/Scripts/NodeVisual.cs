using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeVisual : MonoBehaviour
{
    // Order in array represents layer order
    public GameObject blueNode;
    public GameObject redNode;
    public GameObject inactiveNode;

    public void Awake() {
        foreach(Transform child in gameObject.transform) {
            child.gameObject.SetActive(false);
        }
    }

    public void SetColor(GameColor color) {
        switch (color) {
            case GameColor.Red:
                redNode.SetActive(true);
                break;
            case GameColor.Blue:
                blueNode.SetActive(true);
                break;
            default:
                inactiveNode.SetActive(true);
                break;
        }
    }

    public void Select()
    {

    }

    public void Deselect()
    {

    }
}
