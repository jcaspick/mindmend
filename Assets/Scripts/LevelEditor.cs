using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public enum Tool
{
    Place,
    Erase,
    RedSignal,
    BlueSignal
}

public class LevelEditor : MonoBehaviour
{
    public static LevelEditor instance;

    public int width = 8;
    public int height = 8;
    public EditorMarker markerPrefab;
    public GameObject mainCamera;

    public Node nodeVisualPrefab;
    public Signal signalVisualPrefab;

    private Board board;
    private Transform markerHolder;
    private Tool tool = Tool.Place;
    private List<Node> nodes;
    private Signal redSignal;
    private Signal blueSignal;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        board = new Board(width, height);
        nodes = new List<Node>();

        board.redSignalStart = new Vector2Int(0, 0);
        board.blueSignalStart = new Vector2Int(width - 1, height - 1);
        redSignal = Instantiate(signalVisualPrefab);
        redSignal.transform.position = Vector3.zero;
        redSignal.SetColor(GameColor.Red);

        blueSignal = Instantiate(signalVisualPrefab);
        blueSignal.transform.position = new Vector3(width - 1, height - 1, 0);
        blueSignal.SetColor(GameColor.Blue);

        markerHolder = new GameObject("markers").transform;
        GenerateMarkers();
        GenerateNodes();

        EventManager.AddListener(EventType.EditorMarkerEnter, HandleMarkerEnter);
        EventManager.AddListener(EventType.EditorMarkerExit, HandleMarkerExit);
        EventManager.AddListener(EventType.EditorMarkerClick, HandleMarkerClick);
        EventManager.AddListener(EventType.EditorToolChange, HandleToolChange);

        mainCamera.transform.position = new Vector3(((float)width - 1) * 0.5f, ((float)height - 1) * 0.5f, -10);
    }

    void GenerateMarkers()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var marker = Instantiate(markerPrefab);
                marker.transform.position = new Vector3(x, y, -4);
                marker.gridCoordinates = new Vector2Int(x, y);
                marker.transform.SetParent(markerHolder);
            }
        }
    }

    void GenerateNodes()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                PlaceNode(new Vector2Int(x, y));
            }
        }
    }

    void PlaceNode(Vector2Int gridCoordinates)
    {
        var node = Instantiate(nodeVisualPrefab);
        node.transform.SetParent(markerHolder);
        node.transform.localPosition = new Vector3(gridCoordinates.x, gridCoordinates.y, 10);
        node.gridCoordinates = gridCoordinates;
        nodes.Add(node);
        board.AddNode(gridCoordinates);
    }

    void EraseNode(Vector2Int gridCoordinates)
    {
        var node = GetNodeAtCoordinates(gridCoordinates);
        if (node == null)
            return;
        nodes.Remove(node);
        board.RemoveNode(gridCoordinates);
        Destroy(node.gameObject);
    }

    void SetRedSignal(Vector2Int gridCoordinates)
    {
        board.redSignalStart = gridCoordinates;
        redSignal.transform.position = new Vector3(gridCoordinates.x, gridCoordinates.y, 0);
    }

    void SetBlueSignal(Vector2Int gridCoordinates)
    {
        board.blueSignalStart = gridCoordinates;
        blueSignal.transform.position = new Vector3(gridCoordinates.x, gridCoordinates.y, 0);
    }

    Node GetNodeAtCoordinates(Vector2Int coordinates)
    {
        foreach (var node in nodes)
        {
            if (node.gridCoordinates == coordinates)
                return node;
        }

        return null;
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
                if (Input.GetMouseButton(0))
                    EraseNode(details.coordinates);
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
                EraseNode(details.coordinates);
                break;
            case Tool.RedSignal:
                SetRedSignal(details.coordinates);
                break;
            case Tool.BlueSignal:
                SetBlueSignal(details.coordinates);
                break;
        }
    }

    void HandleToolChange(EventDetails details)
    {
        tool = details.tool;
    }

    public void SaveBoard()
    {
        Debug.Log("Saving board!");
        var boardData = JsonUtility.ToJson(board, true);
        var savePath = Path.Combine(Application.dataPath, "Levels");
        Directory.CreateDirectory(Path.Combine(Application.dataPath, "Levels"));
        File.WriteAllText(Path.Combine(savePath, "level.json"), boardData);
    }
}
