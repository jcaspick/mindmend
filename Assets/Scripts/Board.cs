using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board
{
    public int width;
    public int height;
    private Node[] nodes;

    public Board(int width, int height)
    {
        this.width = width;
        this.height = height;
        nodes = new Node[width * height];
    }
}
