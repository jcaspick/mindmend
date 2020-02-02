using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeVisual : MonoBehaviour
{
    // Order in array represents layer order
    public GameObject blueNode;
    public GameObject redNode;
    public GameObject inactiveNode;

    private GameObject activeNode;

    public void Awake() {
        Reset();
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
            default:
                inactiveNode.SetActive(true);
                activeNode = inactiveNode;
                break;
        }
    }

    public void Expire() {
        SetColor(GameColor.Neutral);    
    }

    public void LowHealth() {
        GameObject mainNode = activeNode.transform.GetChild(0).gameObject;

        Debug.Log(mainNode.name);

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
        
    }

    public void Deselect()
    {

    }
}
