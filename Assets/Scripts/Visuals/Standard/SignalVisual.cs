using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalVisual : SignalVisualBase
{
    public GameObject blueSignal;
    public GameObject redSignal;

    public void Awake() {
        foreach (Transform child in gameObject.transform) {
            child.gameObject.SetActive(false);
        }
    }

    public override void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    public override void SetColor(GameColor color)
    {
        switch (color) {
            case GameColor.Red:
                redSignal.SetActive(true);
                GameObject.Destroy(blueSignal);
                break;
            case GameColor.Blue:
                blueSignal.SetActive(true);
                GameObject.Destroy(redSignal);
                break;
        }
    }
}
