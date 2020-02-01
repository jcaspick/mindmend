using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public NodeVisual visual;

    public Vector2Int gridCoordinates;
    public bool isAlive;
    public List<Connection> connections;
    public GameColor color;

    private void OnMouseUpAsButton()
    {
        EventManager.Invoke(EventType.NodeClicked, new EventDetails(this));
    }

    public void SetColor(GameColor color)
    {
        this.color = color;

        visual.SetColor(color);
        // placeholder graphics code, replace this with something cooler
        //var renderer = GetComponentInChildren<Renderer>();
        
        //switch (color)
        //{
        //    case GameColor.Neutral:
        //        renderer.material = Resources.Load("grey") as Material;
        //        break;
        //    case GameColor.Red:
        //        renderer.material = Resources.Load("red") as Material;
        //        break;
        //    case GameColor.Blue:
        //        renderer.material = Resources.Load("blue") as Material;
        //        break;
        //}
    }

    public void Select()
    {
        //var currentScale = transform.localScale;
        //transform.localScale = currentScale * 2;

        visual.Select();
    }

    public void Deselect()
    {
        //var currentScale = transform.localScale;
        //transform.localScale = currentScale * 0.5f;

        visual.Deselect();
    }
}

public static class NodeExtensions
{
    public static bool IsNeighbor(this Node self, Node other)
    {
        float distance = (self.gridCoordinates - other.gridCoordinates).magnitude;
        if (distance < 2.0f)
            return true;
        else
            return false;
    }

    public static bool IsDiagonal(this Node self, Node other)
    {
        int distance =
            Mathf.Abs(self.gridCoordinates.x - other.gridCoordinates.x) +
            Mathf.Abs(self.gridCoordinates.y - other.gridCoordinates.y);
        return distance == 2;
    }
}