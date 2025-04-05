using System.Collections.Generic;
using UnityEngine;

public class SimpleSpriteAnimator : MonoBehaviour
{
	public List<Sprite> sprites;

	public SpriteRenderer spriteRenderer;

	[Range(0.05f, 1.0f)]
	public float timePerFrame;

	private float timer = 0;

	private int currentFrameIndex = 0;

	public bool reverse = false;

	void Awake()
	{
		spriteRenderer.sprite = sprites[currentFrameIndex];
	}

	public void Randomize()
	{
		timer = Random.Range(0, timePerFrame);
		currentFrameIndex = Random.Range(0, sprites.Count);
	}

	void Update()
	{
		timer += Time.deltaTime;
		while (timer >= timePerFrame)
		{
			if (!reverse)
			{
				currentFrameIndex = (currentFrameIndex + 1) % sprites.Count;
			}
			else 
			{
				currentFrameIndex = (currentFrameIndex - 1) >= 0 ? (currentFrameIndex - 1) : sprites.Count - 1;
			}

			timer -= timePerFrame;
			spriteRenderer.sprite = sprites[currentFrameIndex];
		}
	}
}