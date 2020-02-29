using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Signal : MonoBehaviour
{
    public Vector2Int gridCoordinates;
    public GameColor color;

    public SignalVisualBase visual;

    public void CreateVisuals(SignalVisualBase prefab)
    {
        visual = Instantiate(prefab);
        visual.SetPosition(new Vector3(gridCoordinates.x, gridCoordinates.y, 0));
    }

    public void Update() {
        visual.SetPosition(gameObject.transform.position);
    }

    public void SetColor(GameColor color)
    {
        this.color = color;
        visual.SetColor(color);
    }
}
