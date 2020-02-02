using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveIndicator : MonoBehaviour
{
    public SpriteRenderer renderer;
    public Sprite redIndicator;
    public Sprite blueIndicator;

    public float fadeSpeed;

    public void Start() {
        renderer = GetComponent<SpriteRenderer>();
    }

    public void SetColor(GameColor color) {
        if (GameController.USE_PLACEHOLDER_SIGNAL) {

            switch (color) {
                case GameColor.Neutral:
                    StartCoroutine(FadeIndicator(null));
                    break;
                case GameColor.Red:
                    StartCoroutine(FadeIndicator(redIndicator));
                    break;
                case GameColor.Blue:
                    StartCoroutine(FadeIndicator(blueIndicator));
                    break;
            }
        }
    }

    public IEnumerator FadeIndicator(Sprite sprite) {
        // fade out
        for (float f = 0.0f; f < 1.0f; f += Time.deltaTime / fadeSpeed) {
            renderer.material.color = new Color(1.0f, 1.0f, 1.0f, 1.0f - f);
            yield return null;
        }
        renderer.sprite = sprite;

        // fade in
        for (float f = 0.0f; f < 1.0f; f += Time.deltaTime / fadeSpeed) {
            renderer.material.color = new Color(1.0f, 1.0f, 1.0f, f);
            yield return null;
        }
    }
}
