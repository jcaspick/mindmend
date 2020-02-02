﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditorButtons : MonoBehaviour
{
    public void PlaceNode()
    {
        EventManager.Invoke(EventType.EditorToolChange, new EventDetails(Tool.Place));
    }

    public void EraseNode()
    {
        EventManager.Invoke(EventType.EditorToolChange, new EventDetails(Tool.Erase));
    }

    public void RedSignal()
    {
        EventManager.Invoke(EventType.EditorToolChange, new EventDetails(Tool.RedSignal));
    }

    public void BlueSignal()
    {
        EventManager.Invoke(EventType.EditorToolChange, new EventDetails(Tool.BlueSignal));
    }

    public void RedGoal(int number)
    {
        EventManager.Invoke(EventType.EditorToolChange, new EventDetails(Tool.RedGoal, number));
    }

    public void BlueGoal(int number)
    {
        EventManager.Invoke(EventType.EditorToolChange, new EventDetails(Tool.BlueGoal, number));
    }

    public void Save()
    {
        LevelEditor.instance.SaveBoard();
    }
}
