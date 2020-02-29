using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SignalVisualBase : MonoBehaviour
{
    public abstract void SetPosition(Vector3 pos);
    public abstract void SetColor(GameColor color);
}