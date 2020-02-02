using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeSpriteToScreen : MonoBehaviour
{
    private Vector3 cameraPosition;

    void Start() {
        SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
        if (renderer == null) return;

        cameraPosition = Camera.main.transform.position;

        float width = renderer.sprite.bounds.size.x;
        float height = renderer.sprite.bounds.size.y;

        float worldScreenHeight = Camera.main.orthographicSize * 2.0f;
        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        gameObject.transform.localScale = new Vector2(worldScreenWidth / width, worldScreenHeight / height);
        gameObject.transform.position = new Vector2(cameraPosition.x, cameraPosition.y);
    }
}
