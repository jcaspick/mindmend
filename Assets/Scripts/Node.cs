using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public NodeVisual visual;
    public NodeAudio audio;

    public Vector2Int gridCoordinates;
    public bool isAlive;
    public List<Connection> connections;
    public GameColor color;

    Vector3 startScale;

    void Start()
    {
        startScale = transform.localScale;
    }

    public void CreateVisuals(NodeVisual prefab)
    {
        if (!GameSettings.instance.USE_PLACEHOLDER_NODE)
        {
            visual = Instantiate(prefab);
            visual.transform.position = new Vector3(gridCoordinates.x, gridCoordinates.y, 0);
            GetComponentInChildren<Renderer>().enabled = false;
        }
    }

    public void CreateAudio(NodeAudio prefab) {
        audio = Instantiate(prefab);
        audio.transform.position = new Vector3(gridCoordinates.x, gridCoordinates.y, 0);
    }

    public void SetColor(GameColor color)
    {
        this.color = color;

        if (GameSettings.instance.USE_PLACEHOLDER_NODE)
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

        if (color == GameColor.Red || color == GameColor.Blue) {
            audio.Active();
        } else {
            audio.Inactive();
        }
    }

    public void Select()
    {
        if (GameSettings.instance.USE_PLACEHOLDER_NODE)
        {
            var currentScale = transform.localScale;
            transform.localScale = startScale * 2;
        } else
        {
            visual.Select();
        }
    }

    public void Deselect()
    {
        if (GameSettings.instance.USE_PLACEHOLDER_NODE)
        {
            var currentScale = transform.localScale;
            transform.localScale = startScale;
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

    public static bool IsConnected(this Node self, Node other)
    {
        foreach (var connection in self.connections)
        {
            if (connection.a == other || connection.b == other)
                return true;
        }
        return false;
    }

    public static List<Vector2Int> GetNeighborCoords(this Node self)
    {
        var a = self.gridCoordinates;
        var result = new List<Vector2Int>();

        result.Add(new Vector2Int(a.x + 1, a.y    ));
        result.Add(new Vector2Int(a.x + 1, a.y + 1));
        result.Add(new Vector2Int(a.x,     a.y + 1));
        result.Add(new Vector2Int(a.x - 1, a.y + 1));
        result.Add(new Vector2Int(a.x - 1, a.y    ));
        result.Add(new Vector2Int(a.x - 1, a.y - 1));
        result.Add(new Vector2Int(a.x,     a.y - 1));
        result.Add(new Vector2Int(a.x + 1, a.y - 1));

        return result;
    }

    public static List<Node> GetNeighbors(this Node self)
    {
        var result = new List<Node>();

        foreach (var connection in self.connections)
        {
            if (connection.a == self)
                result.Add(connection.b);
            else
                result.Add(connection.a);
        }

        return result;
    }

    public static Connection GetConnection(this Node self, Node other)
    {
        if (!self.IsNeighbor(other))
            return null;

        foreach (var connection in self.connections)
        {
            if (connection.a == self && connection.b == other)
                return connection;
            else if (connection.b == self && connection.a == other)
                return connection;
        }

        return null;
    }
}