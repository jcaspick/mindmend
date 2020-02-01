using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Signal : MonoBehaviour
{
    public Vector2Int gridCoordinates;
    public GameColor color;

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
