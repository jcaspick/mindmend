using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board
{
    public int width;
    public int height;
    public Vector2Int redSignalStart;
    public Vector2Int blueSignalStart;
    public List<Vector2Int> redGoals;
    public List<Vector2Int> blueGoals;
    public bool[] arrangement;

    public Board(int width, int height)
    {
        this.width = width;
        this.height = height;
        arrangement = new bool[width * height];
    }

    public void AddNode(Vector2Int coordinates)
    {
        arrangement[coordinates.y * width + coordinates.x] = true;
    }

    public void RemoveNode(Vector2Int coordinates)
    {
        arrangement[coordinates.y * width + coordinates.x] = false;
    }

    public bool HasNode(Vector2Int coordinates)
    {
        return arrangement[coordinates.y * width + coordinates.x];
    }
}