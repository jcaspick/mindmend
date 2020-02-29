using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board
{
    public int width;
    public int height;
    public int numColors;
    public int numGoals;
    public int startingHealth;
    public int healthPerGoal;
    public Vector2Int[] signalStart;
    public Vector2Int[] goals;
    public bool[] arrangement;

    // --- deprecated ---
    public Vector2Int redSignalStart;
    public Vector2Int blueSignalStart;
    public Vector2Int[] redGoals;
    public Vector2Int[] blueGoals;
    // ------------------

    public Board(int width, int height)
    {
        this.width = width;
        this.height = height;
        numColors = 2;
        numGoals = 10;
        startingHealth = 5;
        healthPerGoal = 2;
        signalStart = new Vector2Int[numColors];
        goals = new Vector2Int[numGoals];
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
        if (coordinates.x < 0 ||
            coordinates.x >= width ||
            coordinates.y < 0 ||
            coordinates.y >= height)
            return false;
        return arrangement[coordinates.y * width + coordinates.x];
    }
}