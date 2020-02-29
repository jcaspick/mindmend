using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NodeVisualBase : MonoBehaviour
{
    public abstract void SetPosition(Vector3 pos);
    public abstract void SetColor(GameColor color);
    public abstract void Select();
    public abstract void Deselect();
}
