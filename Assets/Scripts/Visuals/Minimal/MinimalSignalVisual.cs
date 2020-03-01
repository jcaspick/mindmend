using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimalSignalVisual : SignalVisualBase
{
    public override void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    public override void SetColor(GameColor color)
    {
        var renderer = GetComponentInChildren<Renderer>();

        switch (color)
        {
            case GameColor.Red:
                renderer.material = Resources.Load("red") as Material;
                break;
            case GameColor.Blue:
                renderer.material = Resources.Load("blue") as Material;
                break;
            case GameColor.Green:
                renderer.material = Resources.Load("green") as Material;
                break;
            case GameColor.Purple:
                renderer.material = Resources.Load("purple") as Material;
                break;
            default:
                renderer.material = Resources.Load("grey") as Material;
                break;
        }
    }
}