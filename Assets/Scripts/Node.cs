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

    public void CreateVisuals(NodeVisual prefab)
    {
        if (!GameController.USE_PLACEHOLDER_NODE)
        {
            visual = Instantiate(prefab);
            visual.transform.position = new Vector3(gridCoordinates.x, gridCoordinates.y, 0);
        }
    }

    public void SetColor(GameColor color)
    {
        this.color = color;

        if (GameController.USE_PLACEHOLDER_NODE)
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
        }
        else
        {
            visual.SetColor(color);
        }
    }

    public void Select()
    {
        if (GameController.USE_PLACEHOLDER_NODE)
        {
            var currentScale = transform.localScale;
            transform.localScale = currentScale * 2;
        } else
        {
            visual.Select();
        }
    }

    public void Deselect()
    {
        if (GameController.USE_PLACEHOLDER_NODE)
        {
            var currentScale = transform.localScale;
            transform.localScale = currentScale * 0.5f;
        } else
        {
            visual.Deselect();
        }
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