using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionPreview : MonoBehaviour
{
    public void SetTarget(Vector3 pos)
    {
        var lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetPosition(1, pos);
    }

    public void SetOrigin(Vector3 pos)
    {
        var lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, pos);
    }
}