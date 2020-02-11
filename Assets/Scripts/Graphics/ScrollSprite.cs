using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollSprite : MonoBehaviour
{
    private Renderer renderer;
    public float[] scrollSpeed = { 1.0f };


    private void Start() {
        renderer = gameObject.GetComponent<Renderer>();
    }

    void FixedUpdate()
    {
        for(var i = 0; i < renderer.materials.Length; i++) {
            float offset = Time.time * scrollSpeed[i];

            renderer.materials[i].mainTextureOffset = new Vector2(offset, 0); 
        }
    }
}
