﻿using System.Collections;
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

public enum GameColor
{
    Neutral,
    Red,
    Blue
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
        Debug.Log("start");

        EventManager.AddListener(EventType.NodeClicked, HandleNodeClicked);

        // initialize some stuff
        nodes = new List<Node>();
        connections = new List<Connection>();
        goals = new List<Goal>();
        nodeHolder = new GameObject("nodes").transform;
        phase = GamePhase.SelectStart;

        // create the red and blue signals
        redSignal = Instantiate(signalPrefab);
        redSignal.gridCoordinates = new Vector2Int(3, 3);
        redSignal.transform.position = new Vector3(3, 3, 0);
        redSignal.transform.SetParent(nodeHolder);
        redSignal.SetColor(GameColor.Red);
        blueSignal = Instantiate(signalPrefab);
        blueSignal.gridCoordinates = new Vector2Int(0, 6);
        blueSignal.transform.position = new Vector3(0, 6, 0);
        blueSignal.transform.SetParent(nodeHolder);
        blueSignal.SetColor(GameColor.Blue);

        //var goal = Instantiate(goalPrefab);
        //goal.gridCoordinates = new Vector2Int(7, 7);
        //goal.transform.position = new Vector3(7, 7, 0);
        //goal.transform.SetParent(nodeHolder);
        //goals.Add(goal);

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                var node = Instantiate(nodePrefab);
                node.transform.position = new Vector3(x, y, 0);
                node.gridCoordinates = new Vector2Int(x, y);
                node.transform.SetParent(nodeHolder);

                if (node.gridCoordinates == blueSignal.gridCoordinates)
                    node.SetColor(GameColor.Blue);
                else if (node.gridCoordinates == redSignal.gridCoordinates)
                    node.SetColor(GameColor.Red);
                else
                    node.SetColor(GameColor.Neutral);

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

        if (firstClicked.IsDiagonal(secondClicked))
        {
            connection.transform.localScale = new Vector3(1.414f, 1.0f, 1.0f);
        }

        connection.a = firstClicked;
        connection.b = secondClicked;
        connection.health = maxConnectionHealth;
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
                connection.SetAppearance((float)connection.health / (float)maxConnectionHealth);
            }
        }

        yield return null;

        firstClicked = null;
        secondClicked = null;
        currentPlayer = currentPlayer == GameColor.Red ? GameColor.Blue : GameColor.Red;
        phase = GamePhase.SelectStart;
    }
}