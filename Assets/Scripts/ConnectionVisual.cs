using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionVisual : MonoBehaviour
{
    // Order in array represents layer order
    public GameObject blueConnection;
    public GameObject redConnection;

    public void Awake() {
        foreach (Transform child in gameObject.transform) {
            child.gameObject.SetActive(false);
        }
    }

    public void Create(Vector3 startNode, Vector3 endNode, float angle)
    {
        gameObject.transform.position = startNode; // start position
        gameObject.transform.rotation = Quaternion.Euler( new Vector3(gameObject.transform.rotation.x, gameObject.transform.rotation.y, angle - 180));

        float distance = Vector3.Distance(startNode, endNode);

        SizeMask(gameObject.transform, distance);
    }

    public void Break()
    {

    }

    public void SetHealthPercentage(float health)
    {

    }

    public void SetColor(GameColor color)
    {
        switch (color) {
            case GameColor.Red:
                redConnection.SetActive(true);
                break;
            case GameColor.Blue:
                blueConnection.SetActive(true);
                break;
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
