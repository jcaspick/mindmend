using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flicker : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Renderer renderer;
    public float minWaitTime = 0.2f;
    public float maxWaitTime = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        renderer = gameObject.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (spriteRenderer) {
            spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, Random.Range(minWaitTime, maxWaitTime));
        } else {
            for (var i = 0; i < renderer.materials.Length; i++) {
                renderer.materials[i].color = new Color(1.0f, 1.0f, 1.0f, Random.Range(minWaitTime, maxWaitTime));
            }
        }
    }
}
