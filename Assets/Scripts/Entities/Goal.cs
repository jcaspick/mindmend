using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public GoalVisualBase visual;
    public GoalAudio audio;

    public Vector2Int gridCoordinates;
    public GameColor color;

    void OnDestroy()
    {
        if (visual != null)
            Destroy(visual.gameObject);
    }

    void Update()
    {
        visual.SetPosition(transform.position);
    }

    public void CreateVisuals()
    {
        visual = Instantiate(GameSettings.instance.graphics.goal);
        visual.SetPosition(new Vector3(gridCoordinates.x, gridCoordinates.y, 0));
    }

    public void CreateAudio(GoalAudio prefab) {
        audio = prefab;
        audio = Instantiate(prefab);
        audio.transform.parent = GameObject.Find("Audio").transform;
        audio.transform.position = new Vector3(gridCoordinates.x, gridCoordinates.y, 0);
    }

    public void Achieve()
    {
        visual.Achieve(color);
        audio.Achieve();
    }

    public void SetColor(GameColor color)
    {
        this.color = color;
        visual.SetColor(color);
    }
}
