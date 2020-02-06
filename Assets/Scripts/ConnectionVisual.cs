using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionVisual : MonoBehaviour
{
    public GameObject blueConnection;
    public GameObject redConnection;
    public GameObject neutralConnection;

    private Node originNode;

    public void Awake() {
        Reset();
    }

    public void Create(Node startNode, Node endNode, float angle)
    {
        originNode = startNode;

        gameObject.transform.position = startNode.transform.position; // start position
        gameObject.transform.rotation = Quaternion.Euler( new Vector3(gameObject.transform.rotation.x, gameObject.transform.rotation.y, angle - 180));

        float distance = Vector3.Distance(startNode.transform.position, endNode.transform.position);

        SizeMask(gameObject.transform, distance);
    }

    public void Break()
    {
        GameObject.Destroy(gameObject);    
    }

    public void SetHealthPercentage(int health, float healthMetric)
    {
        GameObject connection = gameObject.transform.GetChild(0).gameObject;

        if (health <= 3) {
            GameObject mainConnection = connection.transform.GetChild(0).gameObject;

            Flicker flicker = mainConnection.GetComponent<Flicker>();
            if(flicker == null) {
                flicker = mainConnection.AddComponent<Flicker>();
            }
            flicker.minWaitTime = healthMetric;
            flicker.maxWaitTime = health * healthMetric;

            if (health == 1) {
                flicker.minWaitTime = 0;
                flicker.maxWaitTime = 1.0f;

                originNode.visual.LowHealth();
            }
        }
    }

    public void SetColor(GameColor color)
    {
        Reset();

        switch (color) {
            case GameColor.Red:
                redConnection.SetActive(true);
                break;
            case GameColor.Blue:
                blueConnection.SetActive(true);
                break;
            case GameColor.Neutral:
                neutralConnection.SetActive(true);
                break;
        }
    }

    private void Reset() {
        foreach (Transform child in gameObject.transform) {
            child.gameObject.SetActive(false);
        }
    }

    private void SizeMask(Transform parent, float size) {
        for (int x = 0; x < parent.childCount; x++) {

            Transform child = parent.GetChild(x);

            if (child.tag == "Mask") {
                RectTransform maskRect = child.gameObject.GetComponent<RectTransform>();
                maskRect.localScale = new Vector2(size * 2, child.localScale.y);
            }

            SizeMask(child, size);
        }
    }
}
