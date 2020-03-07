using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionVisual : ConnectionVisualBase
{
    private GameObject activeConnection;
    public GameObject blueConnection;
    public GameObject redConnection;
    public GameObject purpleConnection;
    public GameObject greenConnection;
    public GameObject neutralConnection;

    private Vector3 originNode;

    public void Awake() {
        Reset();
    }

    public override void Create(Vector3 startNode, Vector3 endNode, float angle)
    {
        originNode = startNode;

        gameObject.transform.position = startNode; // start position
        gameObject.transform.rotation = Quaternion.Euler( new Vector3(gameObject.transform.rotation.x, gameObject.transform.rotation.y, angle - 180));

        float distance = Vector3.Distance(startNode, endNode);

        SizeMask(gameObject.transform, distance);
    }

    public override void Break()
    {
        GameObject.Destroy(gameObject);    
    }

    public override void SetHealth(int health, int maxHealth)
    {
        float normalizedHealth = (float)health / (float)maxHealth;

        if (activeConnection && health <= 3)
        {
            GameObject mainConnection = activeConnection.transform.GetChild(0).gameObject;

            Flicker flicker = mainConnection.GetComponent<Flicker>();
            if (flicker == null)
            {
                flicker = mainConnection.AddComponent<Flicker>();
            }
            flicker.minWaitTime = normalizedHealth;
            flicker.maxWaitTime = health * normalizedHealth;

            if (health == 1)
            {
                flicker.minWaitTime = 0;
                flicker.maxWaitTime = 1.0f;
            }
        }
    }

    public override void SetColor(GameColor color)
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
            case GameColor.Purple:
                purpleConnection.SetActive(true);
                activeConnection = purpleConnection;
                break;
            case GameColor.Green:
                greenConnection.SetActive(true);
                activeConnection = greenConnection;
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
