using System.Collections.Generic;
using UnityEngine;

public class Tentacle : MonoBehaviour
{
	public Vector2 offset;
	public float rotation;

	private float timeOffset0;
	private float timeOffset1;

	public List<Sprite> potentialSprites;

	void Awake()
	{
		timeOffset0 = Random.Range(0, Mathf.PI * 32);
		timeOffset1 = Random.Range(0, Mathf.PI * 32);

		GetComponent<SpriteRenderer>().sprite = potentialSprites[Random.Range(0, potentialSprites.Count)];
	}

	void Update()
	{
		Vector2 dir = offset.normalized;

		transform.localPosition = offset + dir * Mathf.Sin(Time.realtimeSinceStartup * 2.7f + timeOffset0) * 2f;
		transform.localRotation = Quaternion.Euler(0, 0, rotation * Mathf.Rad2Deg + Mathf.Sin(Time.realtimeSinceStartup * Mathf.PI + timeOffset1) * 10);
	}
}
