using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connection : MonoBehaviour
{
    public Node a;
    public Node b;
    public int health;

    // health will be a value from 0 - 1 representing percentage of highest possible health
    public void SetAppearance(float health)
    {
        float length = transform.localScale.x;
        transform.localScale = new Vector3(length, health, health);
    }
}