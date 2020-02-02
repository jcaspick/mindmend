using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollTexture : MonoBehaviour
{
    private Renderer renderer;
    public float scrollSpeed = 1.0f;

    private float random;

    private void Start() {
        renderer = gameObject.GetComponent<Renderer>();

        random = Random.Range(-1.0f, 1.0f);
    }

    void FixedUpdate()
    {
        var offset = Time.time * scrollSpeed;
        renderer.material.mainTextureOffset = new Vector2(offset + random, 0);
    }
}
