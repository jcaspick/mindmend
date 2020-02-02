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
    ConnectionsDecay
}

public enum GameColor
{
    Neutral,
    Red,
    Blue
}

public class GameController : MonoBehaviour
{
    public static bool USE_PLACEHOLDER_NODE = true;
    public static bool USE_PLACEHOLDER_CONNECTION = true;
    public static bool USE_PLACEHOLDER_SIGNAL = true;
    public static bool USE_PLACEHOLDER_GOAL = true;

    public GameObject mainCamera;
    public ClickableSpace clickableSpacePrefab;

    public Node nodePrefab;
    public Connection connectionPrefab;
    public Signal signalPrefab;
    public Goal goalPrefab;

    public NodeVisual nodeVisualPrefab;
    public ConnectionVisual connectionVisualPrefab;
    public SignalVisual signalVisualPrefab;
    public GoalVisual goalVisualPrefab;

    public int maxConnectionHealth;

    private Transform nodeHolder;
    private Transform clickableHolder;

    private Board board;
    private List<Node> nodes;
    private Signal redSignal;
    private Signal blueSignal;
    private List<Connection> connections;
    private List<Goal> goals;
    private GamePhase phase;
    private GameColor currentPlayer = GameColor.Red;
    private Node firstClicked;
    private Node secondClicked;

    void Start()
    {
        EventManager.AddListener(EventType.SpaceClicked, HandleSpaceClicked);

        // load the level
        var boardPath = Path.Combine(Application.dataPath, "Levels", "level.json");
        board = BoardUtility.LoadFromJson(boardPath);

        // initialize some stuff
        nodes = new List<Node>();
        connections = new List<Connection>();
        goals = new List<Goal>();
        nodeHolder = new GameObject("nodes").transform;
        clickableHolder = new GameObject("clickable spaces").transform;
        phase = GamePhase.SelectStart;

        // create the red and blue signals
        redSignal = Instantiate(signalPrefab);
        redSignal.gridCoordinates = board.redSignalStart;
        redSignal.transform.position = new Vector3(board.redSignalStart.x, board.redSignalStart.y, 0);
        redSignal.transform.SetParent(nodeHolder);
        redSignal.CreateVisuals(signalVisualPrefab);
        redSignal.SetColor(GameColor.Red);

        blueSignal = Instantiate(signalPrefab);
        blueSignal.gridCoordinates = board.blueSignalStart;
        blueSignal.transform.position = new Vector3(board.blueSignalStart.x, board.blueSignalStart.y, 0);
        blueSignal.transform.SetParent(nodeHolder);
        blueSignal.CreateVisuals(signalVisualPrefab);
        blueSignal.SetColor(GameColor.Blue);

        // place the red and blue goals
        //var goal = Instantiate(goalPrefab);
        //goal.gridCoordinates = new Vector2Int(7, 7);
        //goal.transform.position = new Vector3(7, 7, 0);
        //goal.transform.SetParent(nodeHolder);
        //goals.Add(goal);

        for (int i = 0; i < 6; i++)
        {
            var goal = Instantiate(goalPrefab);
            goal.gridCoordinates = board.redGoals[i];
            goal.transform.position = new Vector3(board.redGoals[i].x, board.redGoals[i].y, 0);
            goal.transform.SetParent(nodeHolder);
            goal.CreateVisuals(goalVisualPrefab);
            goal.SetColor(GameColor.Red);
            goals.Add(goal);
        }

        for (int i = 0; i < 6; i++)
        {
            var goal = Instantiate(goalPrefab);
            goal.gridCoordinates = board.blueGoals[i];
            goal.transform.position = new Vector3(board.blueGoals[i].x, board.blueGoals[i].y, 0);
            goal.transform.SetParent(nodeHolder);
            goal.CreateVisuals(goalVisualPrefab);
            goal.SetColor(GameColor.Blue);
            goals.Add(goal);
        }

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
                node.CreateVisuals(nodeVisualPrefab);

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
    }

    void HandleSpaceClicked(EventDetails details)
    {
        switch (phase)
        {
            case GamePhase.SelectStart:
                ChooseFirstNode(GetNodeAtCoordinates(details.coordinates));
                break;
            case GamePhase.SelectEnd:
                ChooseSecondNode(GetNodeAtCoordinates(details.coordinates));
                break;
        }
    }

    void ChooseFirstNode(Node node)
    {
        if (IsValidFirstNode(node))
        {
            firstClicked = node;
            node.SetColor(currentPlayer);
            node.Select();
            phase = GamePhase.SelectEnd;
        }
    }

    bool IsValidFirstNode(Node node)
    {
        return (node.color == currentPlayer);
    }

    void ChooseSecondNode(Node node)
    {
        if (IsValidSecondNode(node))
        {
            secondClicked = node;
            firstClicked.Deselect();
        } else
        {
            return;
        }

        var connection = Instantiate(connectionPrefab);
        connection.transform.position = firstClicked.transform.position;
        float angle = Mathf.Rad2Deg * Mathf.Atan2(secondClicked.gridCoordinates.y - firstClicked.gridCoordinates.y,
            secondClicked.gridCoordinates.x - firstClicked.gridCoordinates.x);
        connection.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        connection.a = firstClicked;
        connection.b = secondClicked;
        connection.health = maxConnectionHealth;

        connection.CreateVisuals(connectionVisualPrefab, angle);

        if (firstClicked.IsDiagonal(secondClicked))
        {
            connection.transform.localScale = new Vector3(1.414f, 1.0f, 1.0f);
        }

        connection.SetColor(currentPlayer);

        connections.Add(connection);
        firstClicked.connections.Add(connection);
        secondClicked.connections.Add(connection);
        firstClicked.SetColor(currentPlayer);
        secondClicked.SetColor(currentPlayer);

        phase = GamePhase.SignalsMove;
        StartCoroutine(HandleSignalMovement(currentPlayer));
    }

    bool IsValidSecondNode(Node node)
    {
        if (node == firstClicked)
            return false;
        if (!node.IsNeighbor(firstClicked))
            return false;
        if (node.color == currentPlayer || node.color == GameColor.Neutral)
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

    IEnumerator HandleSignalMovement(GameColor color)
    {
        yield return null;

        var signal = color == GameColor.Red ? redSignal : blueSignal;
        List<Node> visited = new List<Node>();

        while (true)
        {
            var currentNode = GetNodeAtCoordinates(signal.gridCoordinates);
            visited.Add(currentNode);
            var connections = currentNode.connections;

            connections.Sort((a, b) => { return a.health < b.health ? 1 : -1; });
            if (connections.Count == 0)
            {
                break;
            }

            Node destination = null;

            foreach (var connection in connections)
            {
                if (connection.a != currentNode)
                {
                    if (visited.Contains(connection.a))
                        continue;
                    else
                    {
                        destination = connection.a;
                        break;
                    }
                }
                else if (connection.b != currentNode)
                {
                    if (visited.Contains(connection.b))
                        continue;
                    else
                    {
                        destination = connection.b;
                        break;
                    }
                }
            }

            if (destination == null)
                break;
            else
            {
                signal.gridCoordinates = destination.gridCoordinates;
                yield return StartCoroutine(TweenSignal(signal, currentNode, destination));
            }

            yield return null;
        }

        phase = GamePhase.CheckWinConditions;
        CheckWinConditions(currentPlayer);
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

    void CheckWinConditions(GameColor color)
    {
        var signal = color == GameColor.Red ? redSignal : blueSignal;
        for (int i = goals.Count - 1; i >= 0; i--)
        {
            var goal = goals[i];
            if (goal.gridCoordinates == signal.gridCoordinates)
            {
                Debug.Log("goal reached!");
                goal.Achieve();
                goals.Remove(goal);
            }
        }

        phase = GamePhase.ConnectionsDecay;
        StartCoroutine(ConnectionsDecay(currentPlayer));
    }

    IEnumerator ConnectionsDecay(GameColor color)
    {
        for (int i = connections.Count - 1; i >= 0; i--)
        {
            var connection = connections[i];
            if (connection.color != color)
                continue;
            connection.health--;
            if (connection.health <= 0)
            {
                connection.a.connections.Remove(connection);
                connection.b.connections.Remove(connection);
                if (connection.a.connections.Count == 0)
                    connection.a.SetColor(GameColor.Neutral);
                if (connection.b.connections.Count == 0)
                    connection.b.SetColor(GameColor.Neutral);
                connections.Remove(connection);
                Destroy(connection.gameObject);
            }
            else
            {
                connection.SetHealthPercentage((float)connection.health / (float)maxConnectionHealth);
            }
        }

        yield return null;

        firstClicked = null;
        secondClicked = null;
        currentPlayer = currentPlayer == GameColor.Red ? GameColor.Blue : GameColor.Red;
        phase = GamePhase.SelectStart;
    }
}