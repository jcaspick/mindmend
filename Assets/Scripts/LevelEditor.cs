using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public enum Tool
{
    Place,
    Erase,
    RedSignal,
    BlueSignal,
    RedGoal,
    BlueGoal
}

public class LevelEditor : MonoBehaviour
{
    public static LevelEditor instance;

    public int width = 8;
    public int height = 8;
    public int startingHealth = 5;
    public int healthPerGoal = 2;
    public EditorMarker markerPrefab;
    public GameObject mainCamera;

    public Node nodeVisualPrefab;
    public Signal signalVisualPrefab;
    public Goal goalPrefab;

    private Board board;
    private Transform markerHolder;
    private Tool tool = Tool.Place;
    private int goalIndex = 0;
    private List<Node> nodes;
    private List<EditorMarker> markers;
    private Signal redSignal;
    private Signal blueSignal;
    private Goal[] redGoals;
    private Goal[] blueGoals;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        nodes = new List<Node>();
        markers = new List<EditorMarker>();
        redGoals = new Goal[5];
        blueGoals = new Goal[5];

        for (int i = 0; i < 5; i++)
        {
            redGoals[i] = Instantiate(goalPrefab);
            redGoals[i].SetColor(GameColor.Red);
        }
        for (int i = 0; i < 5; i++)
        {
            blueGoals[i] = Instantiate(goalPrefab);
            blueGoals[i].SetColor(GameColor.Blue);
        }

        redSignal = Instantiate(signalVisualPrefab);
        redSignal.SetColor(GameColor.Red);

        blueSignal = Instantiate(signalVisualPrefab);
        blueSignal.SetColor(GameColor.Blue);

        markerHolder = new GameObject("markers").transform;

        EventManager.AddListener(EventType.EditorMarkerEnter, HandleMarkerEnter);
        EventManager.AddListener(EventType.EditorMarkerExit, HandleMarkerExit);
        EventManager.AddListener(EventType.EditorMarkerClick, HandleMarkerClick);
        EventManager.AddListener(EventType.EditorToolChange, HandleToolChange);

        CreateDefaultBoard();
    }

    void CreateDefaultBoard()
    {
        board = new Board(width, height);
        board.startingHealth = startingHealth;
        board.healthPerGoal = healthPerGoal;

        for (int i = 0; i < 5; i++)
        {
            redGoals[i].gridCoordinates = new Vector2Int(0, height - 1);
            redGoals[i].transform.position = new Vector3(0, height - 1, 0);
            board.redGoals[i] = redGoals[i].gridCoordinates;
        }
        for (int i = 0; i < 5; i++)
        {
            blueGoals[i].gridCoordinates = new Vector2Int(width - 1, 0);
            blueGoals[i].transform.position = new Vector3(width - 1, 0, 0);
            board.blueGoals[i] = blueGoals[i].gridCoordinates;
        }

        board.redSignalStart = new Vector2Int(0, 0);
        board.blueSignalStart = new Vector2Int(width - 1, height - 1);
        redSignal.transform.position = Vector3.zero;
        blueSignal.transform.position = new Vector3(width - 1, height - 1, 0);

        GenerateMarkers();
        GenerateNodes();

        mainCamera.transform.position = new Vector3(((float)width - 1) * 0.5f, ((float)height - 1) * 0.5f, -10);
    }

    void GenerateMarkers()
    {
        // clear existing markers first
        for (int i = markers.Count - 1; i >= 0; i--)
        {
            var marker = markers[i];
            markers.Remove(marker);
            Destroy(marker.gameObject);
        }

        // make grid
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var marker = Instantiate(markerPrefab);
                marker.transform.position = new Vector3(x, y, -4);
                marker.gridCoordinates = new Vector2Int(x, y);
                marker.transform.SetParent(markerHolder);
                markers.Add(marker);
            }
        }
    }

    void GenerateNodes()
    {
        // clear existing nodes first
        for (int i = nodes.Count - 1; i >= 0; i--)
        {
            var node = nodes[i];
            nodes.Remove(node);
            Destroy(node.gameObject);
        }

        // make grid
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

    void SetRedGoal(Vector2Int gridCoordinates, int index)
    {
        board.redGoals[index] = gridCoordinates;
        redGoals[index].transform.position = new Vector3(gridCoordinates.x, gridCoordinates.y, 0);
    }

    void SetBlueGoal(Vector2Int gridCoordinates, int index)
    {
        board.blueGoals[index] = gridCoordinates;
        blueGoals[index].transform.position = new Vector3(gridCoordinates.x, gridCoordinates.y, 0);
    }

    public void ChangeWidth(int width)
    {
        this.width = width;
        CreateDefaultBoard();
    }

    public void ChangeHeight(int height)
    {
        this.height = height;
        CreateDefaultBoard();
    }

    public void SetStartingHealth(int health)
    {
        startingHealth = health;
        board.startingHealth = health;
    }

    public void SetHealthPerGoal(int health)
    {
        healthPerGoal = health;
        board.healthPerGoal = health;
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
            case Tool.RedGoal:
                SetRedGoal(details.coordinates, goalIndex);
                break;
            case Tool.BlueGoal:
                SetBlueGoal(details.coordinates, goalIndex);
                break;
        }
    }

    void HandleToolChange(EventDetails details)
    {
        tool = details.tool;
        goalIndex = details.intData;
    }

    public void SaveBoard()
    {
        Debug.Log("Saving board!");
        var boardData = JsonUtility.ToJson(board, true);
        var savePath = Path.Combine(Application.persistentDataPath, "Levels");
        Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "Levels"));
        File.WriteAllText(Path.Combine(savePath, "level.json"), boardData);
    }
}
