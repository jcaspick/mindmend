﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board
{
    public int width;
    public int height;
    public Vector2Int redSignalStart;
    public Vector2Int blueSignalStart;
    public Vector2Int[] redGoals;
    public Vector2Int[] blueGoals;
    public bool[] arrangement;

    public Board(int width, int height)
    {
        this.width = width;
        this.height = height;
        redGoals = new Vector2Int[6];
        blueGoals = new Vector2Int[6];
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