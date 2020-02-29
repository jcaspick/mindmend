using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GoalVisualBase : MonoBehaviour
{
    public abstract void SetPosition(Vector3 pos);
    public abstract void SetColor(GameColor color);
    public abstract void Achieve(GameColor color);
}
