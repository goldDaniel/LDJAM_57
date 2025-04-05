using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MadnessFOV : MonoBehaviour
{
	[Range(1f, 100f)]
	public float minRadius = 3; // radius around the player when madness is at its greatest
	[Range(5f, 100f)]
	public float maxRadius = 10; // radius around the player when madness is at its lowest

	[Range(0f, 1f)]
	public float madness;

	public Tentacle TentaclePrefab;

	[Range(10, 1000)]
	public int tentacleCount = 100;

	private List<Tentacle> tentacles = new();

	private void Awake()
	{
		float distance = Mathf.Lerp(maxRadius, minRadius, madness);
		for(int i = 0; i < tentacleCount; ++i)
		{
			float angle = (i / (float)tentacleCount) * Mathf.PI * 2;
			Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;
			offset += new Vector2(Random.Range(-2f, 2f), Random.Range(-2f, 2f));

			var tentacle = Instantiate(TentaclePrefab);
			tentacle.offset = offset;
			tentacle.rotation = -Mathf.Atan2(offset.y, -offset.x);

			tentacle.transform.parent = this.transform;
			tentacle.transform.localPosition = offset;
			tentacle.transform.localRotation = Quaternion.Euler(0, 0, tentacle.rotation * Mathf.Rad2Deg);
			tentacle.GetComponent<SpriteRenderer>().sortingOrder = Random.Range(100, 200);
			tentacle.GetComponent<SimpleSpriteAnimator>().Randomize();

			tentacles.Add(tentacle);
		}
	}

	private float lastMadness = 0;
	private void Update()
	{
		if(lastMadness != madness)
		{
			lastMadness = madness;
			float distance = Mathf.Lerp(maxRadius, minRadius, madness);
			for (int i = 0; i < tentacleCount; ++i)
			{
				float angle = (i / (float)tentacleCount) * Mathf.PI * 2;
				Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;
				offset += new Vector2(Random.value * 2.0f - 1.0f, Random.value * 2.0f - 1.0f);

				var tentacle = tentacles[i];
				tentacle.offset = offset;
				tentacle.rotation = -Mathf.Atan2(offset.y, -offset.x);
			}
		}
	}
}
