using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IterateSprites : MonoBehaviour
{
	private SpriteRenderer renderer;

	public Sprite[] sprites;
	private int spriteIndex = 0;

	public float iterateSpeed;

	// Use this for initialization
	void Start() {
		renderer = gameObject.GetComponent<SpriteRenderer>();

		InvokeRepeating("Iterate", 0, iterateSpeed );
	}

	void Iterate() {
		renderer.sprite = sprites[ Random.Range(0, sprites.Length - 1) ];
	}


}
