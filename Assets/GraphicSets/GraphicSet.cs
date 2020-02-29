using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GraphicSet", menuName = "ScriptableObjects/GraphicSet", order = 1)]
public class GraphicSet : ScriptableObject
{
    public NodeVisualBase node;
    public ConnectionVisualBase connection;
    public GoalVisualBase goal;
    public SignalVisualBase signal;
}