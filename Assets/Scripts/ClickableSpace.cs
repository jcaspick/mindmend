using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableSpace : MonoBehaviour
{
    public Vector2Int gridCoordinates;

    void OnMouseUpAsButton()
    {
        EventManager.Invoke(EventType.SpaceClicked, new EventDetails(gridCoordinates));
    }

    void OnMouseDown()
    {
        EventManager.Invoke(EventType.SpaceMouseDown, new EventDetails(gridCoordinates));
    }

    void OnMouseUp()
    {
        EventManager.Invoke(EventType.SpaceMouseUp, new EventDetails(gridCoordinates));
    }

    void OnMouseEnter()
    {
        EventManager.Invoke(EventType.SpaceMouseEnter, new EventDetails(gridCoordinates));
    }

    void OnMouseExit()
    {
        EventManager.Invoke(EventType.SpaceMouseExit, new EventDetails(gridCoordinates));
    }
}