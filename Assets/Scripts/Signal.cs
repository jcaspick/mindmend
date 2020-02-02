using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Signal : MonoBehaviour
{
    public Vector2Int gridCoordinates;
    public GameColor color;

    public SignalVisual visual;

    public void CreateVisuals(SignalVisual prefab)
    {
        if (!GameController.USE_PLACEHOLDER_SIGNAL)
        {
            visual = Instantiate(prefab);
            visual.transform.position = new Vector3(gridCoordinates.x, gridCoordinates.y, 0);
        }
    }

    public void Update() {
        if (!GameController.USE_PLACEHOLDER_SIGNAL)
            visual.transform.position = gameObject.transform.position;
    }

    public void SetColor(GameColor color)
    {
        this.color = color;

        if (GameController.USE_PLACEHOLDER_SIGNAL)
        {
            var renderer = GetComponentInChildren<Renderer>();

            switch (color)
            {
                case GameColor.Neutral:
                    renderer.material = Resources.Load("grey") as Material;
                    break;
                case GameColor.Red:
                    renderer.material = Resources.Load("red") as Material;
                    break;
                case GameColor.Blue:
                    renderer.material = Resources.Load("blue") as Material;
                    break;
            }
        } else
        {
            visual.SetColor(color);
        }
    }
}
