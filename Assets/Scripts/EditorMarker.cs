using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorMarker : MonoBehaviour
{
    public Vector2Int gridCoordinates;

    private void OnMouseDown()
    {
        EventManager.Invoke(EventType.EditorMarkerClick, new EventDetails(gridCoordinates));
    }

    private void OnMouseEnter()
    {
        EventManager.Invoke(EventType.EditorMarkerEnter, new EventDetails(gridCoordinates));
    }

    private void OnMouseExit()
    {
        EventManager.Invoke(EventType.EditorMarkerExit, new EventDetails(gridCoordinates));
    }
}
