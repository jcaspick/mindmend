using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IterateSprites : MonoBehaviour
{
	private SpriteRenderer renderer;

	public Sprite[] sprites;
	private int spriteIndex = 0;

	// Use this for initialization
	void Start() {
		renderer = gameObject.GetComponent<SpriteRenderer>();
	}

	// Update is called once per frame
	void Update() {
		spriteIndex++;
		if (spriteIndex == sprites.Length) {
			spriteIndex = 0;
		}
		renderer.sprite = sprites[spriteIndex];
	}


}
