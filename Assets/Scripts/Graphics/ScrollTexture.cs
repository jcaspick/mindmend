using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollTexture : MonoBehaviour
{
    private Renderer renderer;
    public float scrollSpeed = 1.0f;

    private void Start() {
        renderer = gameObject.GetComponent<Renderer>();  
    }

    void FixedUpdate()
    {
        var offset = Time.time * scrollSpeed;
        renderer.material.mainTextureOffset = new Vector2(offset, 0);
    }
}
