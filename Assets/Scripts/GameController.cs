using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public enum GamePhase
{
    SelectStart,
    SelectEnd,
    SignalsMove,
    CheckWinConditions,
    ConnectionsDecay,
    Win,
    Lose
}

public enum GameColor
{
    Neutral,
    Red,
    Blue,
    Green,
    Purple
}

public class GameController : MonoBehaviour
{
    public GameObject mainCamera;
    public ClickableSpace clickableSpacePrefab;
    public ConnectionPreview connectionPreview;

    public Node nodePrefab;
    public Connection connectionPrefab;
    public Signal signalPrefab;
    public Goal goalPrefab;

    public NodeAudio nodeAudioPrefab;
    public GoalAudio goalAudioPrefab;

    public int baseConnectionHealth;
    public int connectionHealthPerGoal;

    private Transform nodeHolder;
    private Transform clickableHolder;

    private Board board;
    private List<Node> nodes;
    private Signal redSignal;
    private Signal blueSignal;
    private List<Connection> connections;
    private Goal[] redGoals;
    private Goal[] blueGoals;

    private GamePhase phase;
    private GameColor currentPlayer = GameColor.Red;
    private int nextRedGoal = 0;
    private int nextBlueGoal = 0;
    private Node firstClicked;
    private Node secondClicked;
    private Node underMouse;

    void Start()
    {
        //EventManager.AddListener(EventType.SpaceClicked, HandleSpaceClicked);
        EventManager.AddListener(EventType.SpaceMouseDown, HandleSpaceMouseDown);
        EventManager.AddListener(EventType.SpaceMouseEnter, HandleSpaceMouseEnter);
        EventManager.AddListener(EventType.SpaceMouseExit, HandleSpaceMouseExit);

        // load the level
        var boardPath = Path.Combine(Application.persistentDataPath, "Levels", "level.json");
        if (!File.Exists(boardPath))
        {
            Debug.Log("loading level from resources");
            TextAsset boardData = (TextAsset)Resources.Load("level");
            board = JsonUtility.FromJson<Board>(boardData.text);
        } else
        {
            Debug.Log("loading custom level from persistent data path");
            board = BoardUtility.LoadFromJson(boardPath);
        }

        baseConnectionHealth = board.startingHealth;
        connectionHealthPerGoal = board.healthPerGoal;

        // initialize some stuff
        nodes = new List<Node>();
        connections = new List<Connection>();
        redGoals = new Goal[5];
        blueGoals = new Goal[5];
        nodeHolder = new GameObject("nodes").transform;
        clickableHolder = new GameObject("clickable spaces").transform;
        phase = GamePhase.SelectStart;

        // create the red and blue signals
        redSignal = Instantiate(signalPrefab);
        redSignal.gridCoordinates = board.redSignalStart;
        redSignal.transform.position = new Vector3(board.redSignalStart.x, board.redSignalStart.y, 0);
        redSignal.transform.SetParent(nodeHolder);
        redSignal.CreateVisuals();
        redSignal.SetColor(GameColor.Red);

        blueSignal = Instantiate(signalPrefab);
        blueSignal.gridCoordinates = board.blueSignalStart;
        blueSignal.transform.position = new Vector3(board.blueSignalStart.x, board.blueSignalStart.y, 0);
        blueSignal.transform.SetParent(nodeHolder);
        blueSignal.CreateVisuals();
        blueSignal.SetColor(GameColor.Blue);

        // build the notes array, one for each spot on the board
        NoteUtility.Setup(board.width, board.height);

        // reveal the first red and blue goals
        RevealGoal(0, GameColor.Red);
        RevealGoal(0, GameColor.Blue);

        int width = board.width;
        int height = board.height;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (!board.HasNode(new Vector2Int(x, y)))
                    continue;

                // create clickable space
                var clickableSpace = Instantiate(clickableSpacePrefab);
                clickableSpace.transform.position = new Vector3(x, y, -4);
                clickableSpace.gridCoordinates = new Vector2Int(x, y);
                clickableSpace.transform.SetParent(clickableHolder);

                // create node
                var node = Instantiate(nodePrefab);
                node.transform.position = new Vector3(x, y, 0);
                node.gridCoordinates = new Vector2Int(x, y);
                node.transform.SetParent(nodeHolder);
                node.CreateVisuals();
                node.CreateAudio(nodeAudioPrefab);

                if (node.gridCoordinates == blueSignal.gridCoordinates)
                    node.SetColor(GameColor.Blue);
                else if (node.gridCoordinates == redSignal.gridCoordinates)
                    node.SetColor(GameColor.Red);
                else
                    node.SetColor(GameColor.Neutral);

                nodes.Add(node);
            }
        }

        mainCamera.transform.position = new Vector3(((float)width - 1) * 0.5f, ((float)height - 1) * 0.5f, -10);
        connectionPreview.gameObject.SetActive(false);
    }

    void Update()
    {
        if (phase == GamePhase.SelectEnd)
        {
            if (underMouse != null && IsValidSecondNode(underMouse))
            {
                connectionPreview.gameObject.SetActive(true);
                connectionPreview.SetTarget(underMouse.transform.position);
            }
            else
            {
                connectionPreview.gameObject.SetActive(false);
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (underMouse != null && IsValidSecondNode(underMouse))
                {
                    firstClicked.Deselect();
                    MakeConnection(firstClicked, underMouse);
                    phase = GamePhase.SignalsMove;
                    var signal = currentPlayer == GameColor.Red ? redSignal : blueSignal;
                    var path = Pathfinder.GetPath(GetNodeAtCoordinates(signal.gridCoordinates), underMouse);
                    StartCoroutine(HandleSignalMovement(currentPlayer, path));
                } else
                {
                    firstClicked = null;
                    phase = GamePhase.SelectStart;
                }

                connectionPreview.gameObject.SetActive(false);
            }
        }
    }

    void RevealGoal(int i, GameColor color)
    {
        if (color == GameColor.Red)
        {
            var goal = Instantiate(goalPrefab);
            goal.gridCoordinates = board.redGoals[i];
            goal.transform.position = new Vector3(board.redGoals[i].x, board.redGoals[i].y, 0);
            goal.transform.SetParent(nodeHolder);
            goal.CreateVisuals();
            goal.CreateAudio(goalAudioPrefab);
            goal.SetColor(GameColor.Red);
            redGoals[i] = goal;

        }
        else if (color == GameColor.Blue)
        {
            var goal = Instantiate(goalPrefab);
            goal.gridCoordinates = board.blueGoals[i];
            goal.transform.position = new Vector3(board.blueGoals[i].x, board.blueGoals[i].y, 0);
            goal.transform.SetParent(nodeHolder);
            goal.CreateVisuals();
            goal.CreateAudio(goalAudioPrefab);
            goal.SetColor(GameColor.Blue);
            blueGoals[i] = goal;
        }
    }

    void HandleSpaceMouseDown(EventDetails details)
    {
        var node = GetNodeAtCoordinates(details.coordinates);
        if (phase == GamePhase.SelectStart && IsValidFirstNode(node))
        {
            firstClicked = node;
            node.SetColor(currentPlayer);
            node.Select();
            phase = GamePhase.SelectEnd;

            connectionPreview.SetOrigin(node.transform.position);
            connectionPreview.SetTarget(node.transform.position);
        }
    }

    void HandleSpaceMouseEnter(EventDetails details)
    {
        var node = GetNodeAtCoordinates(details.coordinates);
        underMouse = node;
        if (phase == GamePhase.SelectStart && IsValidFirstNode(node))
            node.Select();
        if (phase == GamePhase.SelectEnd && IsValidSecondNode(node))
            node.Select();
    }

    void HandleSpaceMouseExit(EventDetails details)
    {
        var node = GetNodeAtCoordinates(details.coordinates);
        underMouse = null;
        node.Deselect();
    }

    void MakeConnection(Node start, Node end)
    {
        // instantiate and set position and orientation
        var connection = Instantiate(connectionPrefab);
        connection.transform.position = start.transform.position;
        float angle = Mathf.Rad2Deg * Mathf.Atan2(end.gridCoordinates.y - start.gridCoordinates.y,
            end.gridCoordinates.x - start.gridCoordinates.x);

        // set game state
        connection.a = start;
        connection.b = end;
        connection.SetHealth(TotalConnectionHealth());
        connection.SetMaxHealth(TotalConnectionHealth());
        start.connections.Add(connection);
        end.connections.Add(connection);

        connections.Add(connection);

        // create visual object
        connection.CreateVisuals(angle);
        connection.SetColor(currentPlayer);
        start.SetColor(currentPlayer);
        end.SetColor(currentPlayer);
    }

    bool IsValidFirstNode(Node node)
    {
        if (currentPlayer == GameColor.Red && node.gridCoordinates == redSignal.gridCoordinates)
            return true;
        if (currentPlayer == GameColor.Blue && node.gridCoordinates == blueSignal.gridCoordinates)
            return true;
        return (node.color == currentPlayer && HasLegalMove(node));
    }

    bool IsValidSecondNode(Node node)
    {
        if (node == firstClicked)
            return false;
        if (!node.IsNeighbor(firstClicked))
            return false;
        if (node.IsConnected(firstClicked))
            return false;
        if (node.color == GameColor.Neutral)
        {
            return true;
        } else
        {
            return false;
        }
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

    IEnumerator HandleSignalMovement(GameColor color, List<Node> path)
    {
        var signal = color == GameColor.Red ? redSignal : blueSignal;

        for (int i = 0; i < path.Count - 1; i++) {
            var start = path[i];
            var end = path[i + 1];
            yield return StartCoroutine(TweenSignal(signal, start, end));
            signal.gridCoordinates = end.gridCoordinates;
            var connection = start.GetConnection(end);
            connection.SetHealth(TotalConnectionHealth());
            connection.SetMaxHealth(TotalConnectionHealth());

            if (i == path.Count - 2) {
                end.audio.Sustain();
            } else {
                end.audio.Pluck();
            }
        }

        phase = GamePhase.CheckWinConditions;
        StartCoroutine(CheckWinConditions(currentPlayer));
    }

    IEnumerator TweenSignal(Signal signal, Node start, Node end)
    {
        float duration = 0.25f;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            signal.transform.position = Vector3.Lerp(
                start.transform.position,
                end.transform.position,
                elapsed / duration);
            yield return null;
        }

        signal.transform.position = end.transform.position;
    }

    IEnumerator CheckWinConditions(GameColor color)
    {
        if (color == GameColor.Red && nextRedGoal < 5)
        {
            var goal = redGoals[nextRedGoal];
            if (goal.gridCoordinates == redSignal.gridCoordinates)
            {
                goal.Achieve();
                yield return StartCoroutine(UI_Controller.instance.ShowMemory(GameColor.Red, nextRedGoal));

                nextRedGoal++;
                foreach (var connection in connections)
                {
                    if (connection.color == GameColor.Red)
                        connection.ModifyHealth(connectionHealthPerGoal);
                }

                if (nextRedGoal < 5)
                    RevealGoal(nextRedGoal, GameColor.Red);
            }
        }
        else if (color == GameColor.Blue && nextBlueGoal < 5)
        {
            var goal = blueGoals[nextBlueGoal];
            if (goal.gridCoordinates == blueSignal.gridCoordinates)
            {
                goal.Achieve();
                yield return StartCoroutine(UI_Controller.instance.ShowMemory(GameColor.Blue, nextBlueGoal));

                nextBlueGoal++;
                foreach (var connection in connections)
                {
                    if (connection.color == GameColor.Blue)
                        connection.ModifyHealth(connectionHealthPerGoal);
                }

                if (nextBlueGoal < 5)
                    RevealGoal(nextBlueGoal, GameColor.Blue);
            }
        }

        if (nextRedGoal == 5 && nextBlueGoal == 5)
        {
            yield return new WaitForSeconds(3.0f);
            yield return StartCoroutine(UI_Controller.instance.ShowFinalMemory());
        } else
        {
            yield return null;
            phase = GamePhase.ConnectionsDecay;
            StartCoroutine(ConnectionsDecay(currentPlayer));
        }
    }

    IEnumerator ConnectionsDecay(GameColor color)
    {
        for (int i = connections.Count - 1; i >= 0; i--)
        {
            var connection = connections[i];
            if (connection.color != color)
                continue;
            connection.ModifyHealth(-1);
            if (connection.GetHealth() <= 0)
            {
                connection.Break();
                connection.a.connections.Remove(connection);
                connection.b.connections.Remove(connection);
                if (connection.a.connections.Count == 0)
                    connection.a.SetColor(GameColor.Neutral);
                if (connection.b.connections.Count == 0)
                    connection.b.SetColor(GameColor.Neutral);
                connections.Remove(connection);
                Destroy(connection.gameObject);
            }
        }

        yield return null;

        firstClicked = null;
        secondClicked = null;
        currentPlayer = currentPlayer == GameColor.Red ? GameColor.Blue : GameColor.Red;
        EventManager.Invoke(EventType.PlayerChange, new EventDetails(currentPlayer));

        if (IsLoseCondition(currentPlayer))
        {
            Debug.Log("You lose!");
        }
        phase = GamePhase.SelectStart;
    }

    bool IsLoseCondition(GameColor color)
    {
        var coloredNodes = new List<Node>();
        foreach (var node in nodes)
        {
            if (node.color == color)
                coloredNodes.Add(node);
        }

        foreach (var node in coloredNodes)
        {
            if (HasLegalMove(node))
                return false;
        }

        return true;
    }

    bool HasLegalMove(Node node)
    {
        foreach (var coordinates in node.GetNeighborCoords())
        {
            if (!board.HasNode(coordinates))
                continue;
            var neighbor = GetNodeAtCoordinates(coordinates);

            if (neighbor.color == GameColor.Neutral)
                return true;
        }

        return false;
    }

    int TotalConnectionHealth()
    {
        return baseConnectionHealth + (connectionHealthPerGoal * (nextRedGoal + nextBlueGoal));
    }
}