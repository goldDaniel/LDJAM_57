using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Explosion : RegisteredBehaviour<Explosion>
{
	[Range(0.1f, 1f)]
	public float animationTime;

	public List<GameObject> levels = new();

	private float _t = 0;

	int prevLevel = -1;

	void Awake()
	{
		foreach(var level in levels)
			level.SetActive(false);
	}

	void Update()
	{
		float levelInterval = animationTime / (levels.Count);

		int currentLevel = Mathf.FloorToInt(_t / levelInterval);

		if(currentLevel != prevLevel)
		{
			prevLevel = currentLevel;
			levels[currentLevel].SetActive(true);
			levels[currentLevel].transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
		}

		_t += Time.deltaTime;
		if(_t >= animationTime)
		{
			Destroy(this.gameObject);
		}
	}
}