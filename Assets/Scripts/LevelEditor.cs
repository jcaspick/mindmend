using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Tool
{
    Place,
    Erase
}

public class LevelEditor : MonoBehaviour
{
    public int width = 8;
    public int height = 8;
    public EditorMarker markerPrefab;

    public Node nodeVisualsPrefab;

    private Board board;
    private Transform markerHolder;
    private Tool tool = Tool.Place;

    void Start()
    {
        markerHolder = new GameObject("markers").transform;
        GenerateMarkers();

        EventManager.AddListener(EventType.EditorMarkerEnter, HandleMarkerEnter);
        EventManager.AddListener(EventType.EditorMarkerExit, HandleMarkerExit);
        EventManager.AddListener(EventType.EditorMarkerClick, HandleMarkerClick);
    }

    void GenerateMarkers()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var marker = Instantiate(markerPrefab);
                marker.transform.position = new Vector3(x, y, 0);
                marker.gridCoordinates = new Vector2Int(x, y);
                marker.transform.SetParent(markerHolder);
            }
        }

        markerHolder.transform.position = new Vector3(
            ((float)width - 1) * -0.5f, 
            ((float)height - 1) * -0.5f, 0);
    }

    void PlaceNode(Vector2Int gridCoordinates)
    {
        var node = Instantiate(nodeVisualsPrefab);
        node.transform.SetParent(markerHolder);
        node.transform.localPosition = new Vector3(gridCoordinates.x, gridCoordinates.y, 10);
    }

    void EraseNode(Vector2Int gridCoordinates)
    {

    }

    void HandleMarkerEnter(EventDetails details)
    {
        switch (tool)
        {
            case Tool.Place:
                if (Input.GetMouseButton(0))
                    PlaceNode(details.coordinates);
                break;
            case Tool.Erase:
                break;
        }
    }

    void HandleMarkerExit(EventDetails details)
    {

    }

    void HandleMarkerClick(EventDetails details)
    {
        switch (tool)
        {
            case Tool.Place:
                PlaceNode(details.coordinates);
                break;
            case Tool.Erase:
                break;
        }
    }
}
