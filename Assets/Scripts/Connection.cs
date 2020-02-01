using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connection : MonoBehaviour
{
    public Node a;
    public Node b;
    public int health;
    public GameColor color;

    // health will be a value from 0 - 1 representing percentage of highest possible health
    public void SetAppearance(float health)
    {
        float length = transform.localScale.x;
        transform.localScale = new Vector3(length, health, health);
    }

    public void SetColor(GameColor color)
    {
        this.color = color;

        // placeholder graphics code, replace this with something cooler
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
    }
}