using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public static GameSettings instance;

    public bool USE_PLACEHOLDER_NODE = false;
    public bool USE_PLACEHOLDER_CONNECTION = false;
    public bool USE_PLACEHOLDER_SIGNAL = false;
    public bool USE_PLACEHOLDER_GOAL = false;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
}
