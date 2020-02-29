using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public enum Tool
{
    Place,
    Erase,
    RedSignal,
    BlueSignal,
    PlaceGoal
}

public class LevelEditor : MonoBehaviour
{
    public static LevelEditor instance;

    public int width = 8;
    public int height = 8;
    public int startingHealth = 5;
    public int healthPerGoal = 2;
    public int numGoals = 10;
    public EditorMarker markerPrefab;
    public GameObject goalLabelPrefab;
    public Transform canvas;
    public Dictionary<int, GameObject> goalLabels;
    public GameObject mainCamera;

    public Node nodePrefab;
    public Signal signalPrefab;
    public Goal goalPrefab;

    private Board board;
    private Transform markerHolder;
    private Tool tool = Tool.Place;
    private int goalIndex = 0;
    private bool autoIncrement = true;
    private List<Node> nodes;
    private List<EditorMarker> markers;
    private Signal redSignal;
    private Signal blueSignal;
    private List<Goal> goals;

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
        goals = new List<Goal>();
        goalLabels = new Dictionary<int, GameObject>();

        for (int i = 0; i < numGoals; i++)
        {
            var goal = Instantiate(goalPrefab);
            goal.CreateVisuals();

            if (i == 0)
                goal.SetColor(GameColor.Red);
            else if (i == 1)
                goal.SetColor(GameColor.Blue);
            else
                goal.SetColor(GameColor.Neutral);

            goals.Add(goal);
            SetGoalLabel(i, Vector3.zero);
        }

        redSignal = Instantiate(signalPrefab);
        redSignal.CreateVisuals();
        redSignal.SetColor(GameColor.Red);

        blueSignal = Instantiate(signalPrefab);
        blueSignal.CreateVisuals();
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
        mainCamera.transform.position = new Vector3(((float)width - 1) * 0.5f, ((float)height - 1) * 0.5f, -10);

        board = new Board(width, height);
        board.startingHealth = startingHealth;
        board.healthPerGoal = healthPerGoal;

        for (int i = 0; i < numGoals; i++)
        {
            PlaceGoal(new Vector2Int(i % width, (height - 1) - (i % height)), i);
        }

        board.redSignalStart = new Vector2Int(0, 0);
        board.blueSignalStart = new Vector2Int(width - 1, height - 1);
        redSignal.transform.position = Vector3.zero;
        blueSignal.transform.position = new Vector3(width - 1, height - 1, 0);

        GenerateMarkers();
        GenerateNodes();
    }

    void LoadBoard(Board b)
    {
        board = new Board(b.width, b.height);

        ClearNodes();

        startingHealth = board.startingHealth = b.startingHealth;
        healthPerGoal = board.healthPerGoal = b.healthPerGoal;

        // create the red and blue signals
        redSignal.gridCoordinates = board.redSignalStart = b.redSignalStart;
        redSignal.transform.position = new Vector3(b.redSignalStart.x, b.redSignalStart.y, 0);

        blueSignal.gridCoordinates = board.blueSignalStart = b.blueSignalStart;
        blueSignal.transform.position = new Vector3(b.blueSignalStart.x, b.blueSignalStart.y, 0);

        width = b.width;
        height = b.height;

        // place the nodes
        for (int x = 0; x < b.width; x++) {
            for (int y = 0; y < b.height; y++) {
                if (!b.HasNode(new Vector2Int(x, y)))
                    continue;

                PlaceNode(new Vector2Int(x, y));
            }
        }

        // the goals
        board.redGoals = b.redGoals;
        board.blueGoals = b.blueGoals;

        //for (int i = 0; i < board.redGoals.Length; i++) {
        //    redGoals[i].transform.position = new Vector3(b.redGoals[i].x, b.redGoals[i].y, 0);
        //}
        //for (int i = 0; i < board.blueGoals.Length; i++) {
        //    blueGoals[i].transform.position = new Vector3(b.blueGoals[i].x, b.blueGoals[i].y, 0);
        //}

        GenerateMarkers();

        mainCamera.transform.position = new Vector3(((float)b.width - 1) * 0.5f, ((float)b.height - 1) * 0.5f, -10);
    }

    void SetGoalLabel(int i, Vector3 pos)
    {
        if (goalLabels.ContainsKey(i))
        {
            var label = goalLabels[i];
            label.transform.position = Camera.main.WorldToScreenPoint(pos);
        } else
        {
            var label = Instantiate(goalLabelPrefab);
            label.transform.SetParent(canvas);
            label.transform.position = Camera.main.WorldToScreenPoint(pos);
            label.GetComponent<Text>().text = (i + 1).ToString();
            goalLabels.Add(i, label);
        }
    }

    void GenerateMarkers()
    {
        // clear existing markers first
        for (int i = markers.Count - 1; i >= 0; i--) {
            var marker = markers[i];
            markers.Remove(marker);
            Destroy(marker.gameObject);
        }

        // make grid
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                PlaceMarker(new Vector2Int(x, y));
            }
        }
    }

    void PlaceMarker(Vector2Int gridCoordinates) {
        var marker = Instantiate(markerPrefab);
        marker.transform.position = new Vector3(gridCoordinates.x, gridCoordinates.y, -4);
        marker.gridCoordinates = new Vector2Int(gridCoordinates.x, gridCoordinates.y);
        marker.transform.SetParent(markerHolder);
        markers.Add(marker);
    }

    void GenerateNodes()
    {
        ClearNodes();

        // make grid
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                PlaceNode(new Vector2Int(x, y));
            }
        }
    }

    void ClearNodes() {
        // clear existing nodes first
        for (int i = nodes.Count - 1; i >= 0; i--) {
            var node = nodes[i];
            nodes.Remove(node);
            Destroy(node.gameObject);
        }
    }

    void PlaceNode(Vector2Int gridCoordinates)
    {
        var existing = GetNodeAtCoordinates(gridCoordinates);
        if (existing != null)
            return;

        var node = Instantiate(nodePrefab);
        node.transform.SetParent(markerHolder);
        node.transform.localPosition = new Vector3(gridCoordinates.x, gridCoordinates.y, 10);
        node.gridCoordinates = gridCoordinates;
        nodes.Add(node);
        board.AddNode(gridCoordinates);
        node.CreateVisuals();
        node.SetColor(GameColor.Neutral);
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

    void PlaceGoal(Vector2Int gridCoordinates, int index)
    {
        Vector3 pos = new Vector3(gridCoordinates.x, gridCoordinates.y, 0);
        goals[index].gridCoordinates = gridCoordinates;
        goals[index].transform.position = pos;
        board.goals[index] = gridCoordinates;
        SetGoalLabel(index, pos + new Vector3(0.25f, 0.25f, 0.0f));
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

    public void SetNumGoals(int numGoals)
    {
        this.numGoals = numGoals;

        if (goals.Count > numGoals)
        {
            for (int i = goals.Count - 1; i >= numGoals; i--)
            {
                var goal = goals[i];
                goals.Remove(goal);
                if (goalLabels.ContainsKey(i))
                    Destroy(goalLabels[i].gameObject);
                Destroy(goal.gameObject);
            }
        } else if (goals.Count < numGoals)
        {
            for (int i = goals.Count; i < numGoals; i++)
            {
                var goal = Instantiate(goalPrefab);
                goal.CreateVisuals();
                goal.SetColor(GameColor.Neutral);
                goals.Add(goal);
                PlaceGoal(new Vector2Int(i % width, (height - 1) - (i % height)), i);
            }
        }
    }

    public void SetAutoIncrement(bool val)
    {
        autoIncrement = val;
    }

    public void SetGoalIndex(int goalIndex)
    {
        this.goalIndex = goalIndex;
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
            case Tool.PlaceGoal:
                PlaceGoal(details.coordinates, goalIndex);
                if (autoIncrement)
                {
                    goalIndex++;
                    if (goalIndex >= numGoals)
                        goalIndex = 0;
                }
                break;
        }
    }

    void HandleToolChange(EventDetails details)
    {
        tool = details.tool;
    }

    public void LoadBoard() {
        var boardPath = Path.Combine(Application.persistentDataPath, "Levels", "level.json");
        board = BoardUtility.LoadFromJson(boardPath);
        LoadBoard(board);
    }

    void UpdateBoardData()
    {
        // basic data
        board.numGoals = numGoals;

        // goal positions
        board.goals = new Vector2Int[numGoals];
        for (int i = 0; i < goals.Count; i++)
        {
            board.goals[i] = goals[i].gridCoordinates;
        }
    }

    public void SaveBoard()
    {
        UpdateBoardData();

        Debug.Log("Saving board!");
        var boardData = JsonUtility.ToJson(board, true);
        var savePath = Path.Combine(Application.persistentDataPath, "Levels");
        Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "Levels"));
        File.WriteAllText(Path.Combine(savePath, "level.json"), boardData);
    }
}
