using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MadnessFOV : MonoBehaviour
{
	[Range(1f, 100f)]
	public float minRadius = 3; // radius around the player when madness is at its greatest
	[Range(5f, 100f)]
	public float maxRadius = 10; // radius around the player when madness is at its lowest

	public Tentacle TentaclePrefab;

	[Range(10, 1000)]
	public int tentacleCount = 100;

	private List<Tentacle> tentacles = new();

	[Range(0f, 1f)]
	public float vignetteScale;
	public Volume volume;

	public bool isMainMenu = false;

	[Range(0f, 1f)]
	public float mainMenuT;

	private void Start()
	{
		float distance = Mathf.Lerp(maxRadius, minRadius, isMainMenu ? mainMenuT : Game.Instance.player.madness);
		for(int i = 0; i < tentacleCount; ++i)
		{
			var tentacle = Instantiate(TentaclePrefab);
			tentacles.Add(tentacle);

			float distanceOffset = Random.Range(0, maxRadius * 0.25f);
			distanceOffset = Random.value > 0.8f * (i / (float)tentacleCount) ? 0 : distanceOffset;

			float angle = (i / (float)tentacleCount) * Mathf.PI * 2;
			tentacle.initial = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * (distance + distanceOffset);

			tentacle.distanceOffset = distanceOffset;
			tentacle.transform.parent = this.transform;
			tentacle.offset = new Vector2(Random.Range(-2f, 2f), Random.Range(-2f, 2f)); ;
			tentacle.GetComponent<SpriteRenderer>().sortingOrder = Random.Range(100, 200) + Mathf.CeilToInt(distanceOffset);
		}
	}

	private float lastMadness = -1;
	private void Update()
	{
		if(lastMadness != (isMainMenu ? mainMenuT : Game.Instance.player.madness))
		{
			lastMadness = isMainMenu ? mainMenuT : Game.Instance.player.madness;
			float distance = Mathf.Lerp(maxRadius, minRadius, isMainMenu ? mainMenuT : Game.Instance.player.madness);
			for (int i = 0; i < tentacleCount; ++i)
			{
				var tentacle = tentacles[i];

				float angle = (i / (float)tentacleCount) * Mathf.PI * 2;
				tentacle.initial = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * (distance + tentacle.distanceOffset);
			}

			if(volume.profile.TryGet<Vignette>(out var vignette))
			{
				vignette.intensity.Override((isMainMenu ? mainMenuT : Game.Instance.player.madness) * vignetteScale);
			}
		}
	}
}
