using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GamePhase
{
    SelectStart,
    SelectEnd,
    SignalsMove,
    CheckWinConditions,
    ConnectionsDecay
}

public class GameController : MonoBehaviour
{
    public Node nodePrefab;
    public Connection connectionPrefab;
    public Signal signalPrefab;
    public Goal goalPrefab;
    public int maxConnectionHealth;

    private Transform nodeHolder;
    private List<Node> nodes;
    private List<Signal> signals;
    private List<Connection> connections;
    private List<Goal> goals;
    private GamePhase phase;
    private Node firstClicked;
    private Node secondClicked;

    void Start()
    {
        Debug.Log("start");

        EventManager.AddListener(EventType.NodeClicked, HandleNodeClicked);

        nodes = new List<Node>();
        signals = new List<Signal>();
        connections = new List<Connection>();
        goals = new List<Goal>();
        nodeHolder = new GameObject("nodes").transform;
        phase = GamePhase.SelectStart;

        var signal = Instantiate(signalPrefab);
        signal.gridCoordinates = new Vector2Int(3, 3);
        signal.transform.position = new Vector3(3, 3, 0);
        signal.transform.SetParent(nodeHolder);
        signals.Add(signal);

        var goal = Instantiate(goalPrefab);
        goal.gridCoordinates = new Vector2Int(7, 7);
        goal.transform.position = new Vector3(7, 7, 0);
        goal.transform.SetParent(nodeHolder);
        goals.Add(goal);

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                var node = Instantiate(nodePrefab);
                node.transform.position = new Vector3(x, y, 0);
                node.gridCoordinates = new Vector2Int(x, y);
                node.transform.SetParent(nodeHolder);
                nodes.Add(node);
            }
        }

        nodeHolder.transform.position = new Vector3(-4, -4, 0);
    }

    void HandleNodeClicked(EventDetails details)
    {
        switch (phase)
        {
            case GamePhase.SelectStart:
                ChooseFirstNode(details.node);
                break;
            case GamePhase.SelectEnd:
                ChooseSecondNode(details.node);
                break;
        }
    }

    void ChooseFirstNode(Node node)
    {
        firstClicked = node;
        node.Select();
        phase = GamePhase.SelectEnd;
    }

    void ChooseSecondNode(Node node)
    {
        if (node != firstClicked && node.IsNeighbor(firstClicked))
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
        if (firstClicked.IsDiagonal(secondClicked))
        {
            connection.transform.localScale = new Vector3(1.414f, 1.0f, 1.0f);
        }

        connection.a = firstClicked;
        connection.b = secondClicked;
        connection.health = maxConnectionHealth;

        connections.Add(connection);
        firstClicked.connections.Add(connection);
        secondClicked.connections.Add(connection);

        phase = GamePhase.SignalsMove;
        StartCoroutine(HandleSignalsMove());
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

    IEnumerator HandleSignalsMove()
    {
        yield return null;

        var signal = signals[0];
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
        CheckWinConditions();
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

    void CheckWinConditions()
    {
        var signal = signals[0];
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
        StartCoroutine(ConnectionsDecay());
    }

    IEnumerator ConnectionsDecay()
    {
        for (int i = connections.Count - 1; i >= 0; i--)
        {
            var connection = connections[i];
            connection.health--;
            if (connection.health <= 0)
            {
                connection.a.connections.Remove(connection);
                connection.b.connections.Remove(connection);
                connections.Remove(connection);
                Destroy(connection.gameObject);
            }
            else
            {
                connection.SetAppearance((float)connection.health / (float)maxConnectionHealth);
            }
        }

        yield return null;

        firstClicked = null;
        secondClicked = null;
        phase = GamePhase.SelectStart;
    }
}
