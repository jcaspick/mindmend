using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connection : MonoBehaviour
{
    public Node a;
    public Node b;
    public int health;
    public GameColor color;

    public ConnectionVisual visual;

    public void CreateVisuals(ConnectionVisual prefab, float angle)
    {
        if (!GameController.USE_PLACEHOLDER_CONNECTION)
        {
            visual = Instantiate(prefab);
            visual.Create(a.transform.position, b.transform.position, angle);
            GetComponentInChildren<Renderer>().enabled = false;
        }
    }

    public void SetHealthPercentage(float health)
    {
        if (GameController.USE_PLACEHOLDER_CONNECTION)
        {
            float length = transform.localScale.x;
            transform.localScale = new Vector3(length, health, health);
        } else
        {
            visual.SetHealthPercentage(health);
        }
    }

    public void SetColor(GameColor color)
    {
        this.color = color;

        if (GameController.USE_PLACEHOLDER_CONNECTION)
        {
            var renderer = GetComponentInChildren<Renderer>();

            switch (color)
            {
                case GameColor.Neutral:
                    renderer.material = Resources.Load("grey") as Material;
                    break;
                case GameColor.Red:
                    renderer.material = Resources.Load("red") as Material;
                    break;
                case GameColor.Blue:
                    renderer.material = Resources.Load("blue") as Material;
                    break;
            }
        } else
        {
            visual.SetColor(color);
        }
    }
}