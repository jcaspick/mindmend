using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimalConnectionVisual : ConnectionVisualBase
{
    public override void Break()
    {
        Destroy(gameObject);
    }

    public override void Create(Vector3 startNode, Vector3 endNode, float angle)
    {
        transform.position = startNode;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // stretch graphic if it is a diagonal connection
        if (startNode.x != endNode.x && startNode.y != endNode.y)
        {
            transform.localScale = new Vector3(1.414f, 1.0f, 1.0f);
        }
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
           
    public override void SetHealth(int health, int maxHealth)
    {
        float normalizedHealth = (float)health / (float)maxHealth;
        float length = transform.localScale.x;
        transform.localScale = new Vector3(length, normalizedHealth, normalizedHealth);
    }
}
