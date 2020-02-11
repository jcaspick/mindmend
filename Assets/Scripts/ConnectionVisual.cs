using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionVisual : MonoBehaviour
{
    private GameObject activeConnection;
    public GameObject blueConnection;
    public GameObject redConnection;
    public GameObject neutralConnection;

    private Vector3 originNode;

    public void Awake() {
        Reset();
    }

    public void Create(Vector3 startNode, Vector3 endNode, float angle)
    {
        originNode = startNode;

        gameObject.transform.position = startNode; // start position
        gameObject.transform.rotation = Quaternion.Euler( new Vector3(gameObject.transform.rotation.x, gameObject.transform.rotation.y, angle - 180));

        float distance = Vector3.Distance(startNode, endNode);

        SizeMask(gameObject.transform, distance);
    }

    public void Break()
    {
        GameObject.Destroy(gameObject);    
    }

    public void SetHealthPercentage(int health, float healthMetric)
    {
        if (activeConnection && health <= 3) {
            GameObject mainConnection = activeConnection.transform.GetChild(0).gameObject;

            Flicker flicker = mainConnection.GetComponent<Flicker>();
            if(flicker == null) {
                flicker = mainConnection.AddComponent<Flicker>();
            }
            flicker.minWaitTime = healthMetric;
            flicker.maxWaitTime = health * healthMetric;

            if (health == 1) {
                flicker.minWaitTime = 0;
                flicker.maxWaitTime = 1.0f;
            }
        }
    }

    public void SetColor(GameColor color)
    {
        Reset();

        switch (color) {
            case GameColor.Red:
                redConnection.SetActive(true);
                activeConnection = redConnection;
                break;
            case GameColor.Blue:
                blueConnection.SetActive(true);
                activeConnection = blueConnection;
                break;
            case GameColor.Neutral:
                neutralConnection.SetActive(true);
                activeConnection = neutralConnection;
                break;
            default:
                activeConnection = null;
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
