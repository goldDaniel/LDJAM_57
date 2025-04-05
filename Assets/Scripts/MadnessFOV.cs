using System.Collections.Generic;
using UnityEngine;

public class MadnessFOV : MonoBehaviour
{
	[Range(1f, 100f)]
	public float minRadius = 3; // radius around the player when madness is at its greatest
	[Range(5f, 100f)]
	public float maxRadius = 10; // radius around the player when madness is at its lowest

	[Range(0f, 1f)]
	public float madness;

	public GameObject TentaclePrefab;

	[Range(10, 1000)]
	public int tentacleCount = 100;

	private List<GameObject> tentacles = new();

	private void Awake()
	{
		float distance = Mathf.Lerp(maxRadius, minRadius, madness);
		for(int i = 0; i < tentacleCount; ++i)
		{
			float angle = (i / (float)tentacleCount) * Mathf.PI * 2;
			Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;
			offset += new Vector2(Random.value * 2.0f - 1.0f, Random.value * 2.0f - 1.0f);

			float rotation = -Mathf.Atan2(offset.y, -offset.x);

			var tentacle = Instantiate(TentaclePrefab);
			tentacle.transform.parent = this.transform;
			tentacle.transform.localPosition = offset;
			tentacle.transform.rotation = Quaternion.Euler(0, 0, rotation * Mathf.Rad2Deg);
			tentacle.GetComponent<SpriteRenderer>().sortingOrder = Random.Range(100, 200);
			tentacle.GetComponent<SimpleSpriteAnimator>().Randomize();

			tentacles.Add(tentacle);
		}
	}

	void Update()
	{
		float distance = Mathf.Lerp(maxRadius, minRadius, madness);
		for (int i = 0; i < tentacleCount; ++i)
		{
			float angle = (i / (float)tentacleCount) * Mathf.PI * 2;
			Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;

			float rotation = -Mathf.Atan2(offset.y, -offset.x);

			var tentacle = tentacles[i];
			tentacle.transform.parent = this.transform;
			tentacle.transform.localPosition = offset;
			tentacle.transform.rotation = Quaternion.Euler(0, 0, rotation * Mathf.Rad2Deg);
		}
	}
}
