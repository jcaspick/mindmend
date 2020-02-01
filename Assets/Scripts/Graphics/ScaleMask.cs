using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleMask : MonoBehaviour
{
    private RectTransform maskRect;

    private float fullLength;
    public float drawSpeed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        maskRect = gameObject.GetComponent<RectTransform>();
        fullLength = maskRect.localScale.x;

        maskRect.localScale = new Vector2(0, maskRect.localScale.y); // collapse the mask on x
        maskRect.localPosition = new Vector2(fullLength/2, 0); // position to start node

        StartCoroutine("ScaleMaskToLength");
    }

    public IEnumerator ScaleMaskToLength() {
        for (float f = 0.0f; f < 1.0f; f += Time.deltaTime / drawSpeed) {
            maskRect.localScale = new Vector2(fullLength * f, maskRect.localScale.y); // draw the line
            maskRect.localPosition = new Vector2(fullLength - maskRect.localScale.x, 0); // offset the edge to look like it's drawing from left

            yield return null;
        }
    }
}
