using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flicker : MonoBehaviour
{
    private SpriteRenderer renderer;
    public float minWaitTime = 0.2f;
    public float maxWaitTime = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        renderer = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        renderer.color = new Color(1.0f, 1.0f, 1.0f, Random.Range(minWaitTime, maxWaitTime));
    }
}
