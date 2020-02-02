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
    Blue
}

public class GameController : MonoBehaviour
{
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

    void Start()
    {
        EventManager.AddListener(EventType.SpaceClicked, HandleSpaceClicked);

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

        maxConnectionHealth = board.startingHealth;
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
        redSignal.CreateVisuals(signalVisualPrefab);
        redSignal.SetColor(GameColor.Red);

        blueSignal = Instantiate(signalPrefab);
        blueSignal.gridCoordinates = board.blueSignalStart;
        blueSignal.transform.position = new Vector3(board.blueSignalStart.x, board.blueSignalStart.y, 0);
        blueSignal.transform.SetParent(nodeHolder);
        blueSignal.CreateVisuals(signalVisualPrefab);
        blueSignal.SetColor(GameColor.Blue);

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

    void RevealGoal(int i, GameColor color)
    {
        if (color == GameColor.Red)
        {
            var goal = Instantiate(goalPrefab);
            goal.gridCoordinates = board.redGoals[i];
            goal.transform.position = new Vector3(board.redGoals[i].x, board.redGoals[i].y, 0);
            goal.transform.SetParent(nodeHolder);
            goal.CreateVisuals(goalVisualPrefab);
            goal.SetColor(GameColor.Red);
            redGoals[i] = goal;
        }
        else if (color == GameColor.Blue)
        {
            var goal = Instantiate(goalPrefab);
            goal.gridCoordinates = board.blueGoals[i];
            goal.transform.position = new Vector3(board.blueGoals[i].x, board.blueGoals[i].y, 0);
            goal.transform.SetParent(nodeHolder);
            goal.CreateVisuals(goalVisualPrefab);
            goal.SetColor(GameColor.Blue);
            blueGoals[i] = goal;
        }
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
        if (currentPlayer == GameColor.Red && node.gridCoordinates == redSignal.gridCoordinates)
            return true;
        if (currentPlayer == GameColor.Blue && node.gridCoordinates == blueSignal.gridCoordinates)
            return true;
        return (node.color == currentPlayer && HasLegalMove(node));
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
        connection.health = TotalConnectionHealth();

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
                        connection.health += connectionHealthPerGoal;
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
                        connection.health += connectionHealthPerGoal;
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
            connection.health--;
            if (connection.health <= 0)
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
            else
            {
                connection.SetHealthPercentage((float)connection.health / (float)TotalConnectionHealth());
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
        return maxConnectionHealth + (connectionHealthPerGoal * (nextRedGoal + nextBlueGoal));
    }
}