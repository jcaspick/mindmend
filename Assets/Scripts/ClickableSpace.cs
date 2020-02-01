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
}