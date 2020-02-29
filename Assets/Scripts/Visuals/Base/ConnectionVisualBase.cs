using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ConnectionVisualBase : MonoBehaviour
{
    public abstract void Create(Vector3 startNode, Vector3 endNode, float angle);
    public abstract void Break();
    public abstract void SetColor(GameColor color);
    public abstract void SetHealth(int health, int maxHealth);
}
