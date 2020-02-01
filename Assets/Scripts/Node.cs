using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Vector2Int gridCoordinates;
    public bool isAlive;
    public List<Connection> connections;

    private void OnMouseUpAsButton()
    {
        EventManager.Invoke(EventType.NodeClicked, new EventDetails(this));
    }

    public void Select()
    {
        var currentScale = transform.localScale;
        transform.localScale = currentScale * 2;
    }

    public void Deselect()
    {
        var currentScale = transform.localScale;
        transform.localScale = currentScale * 0.5f;
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