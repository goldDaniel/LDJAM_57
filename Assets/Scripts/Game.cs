using JetBrains.Annotations;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Game : MonoBehaviour
{
	public static Game Instance;

	public PlayerController player;

	public Texture2D cursorTexture;

	public GameObject brainCrabPrefab;

	public Level level;

	public List<PerkTemplate> currentPerkPool;

    public List<PerkTemplate> madnessPerkPool;

    void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
		{
			Debug.LogError("Cannot instantiate multiple games. Something is wrong!");
			Destroy(this.gameObject);
		}
	}

	void Start()
	{
		Cursor.lockState = CursorLockMode.Confined;
		Cursor.SetCursor(cursorTexture, new Vector2(cursorTexture.width / 2, cursorTexture.height / 2), CursorMode.Auto);

		Bounds levelBounds = new();
		levelBounds.min = new Vector3(-level.arenaWidth / 2 + level.wallWidth, -level.arenaHeight / 2 + level.wallWidth, 0);
		levelBounds.max = new Vector3(level.arenaWidth / 2 - level.wallWidth, level.arenaHeight / 2 - level.wallWidth, 0);

		for(int i = 0; i < 10; ++i)
		{
			var brainCrab = Instantiate(brainCrabPrefab);
			Vector2 pos = new();
			pos.x = Random.Range(levelBounds.min.x + brainCrab.transform.localScale.x, levelBounds.max.x - brainCrab.transform.localScale.x);
			pos.y = Random.Range(levelBounds.min.y + brainCrab.transform.localScale.y, levelBounds.max.y - brainCrab.transform.localScale.y);
			brainCrab.transform.position = pos;
		}
	}

	private void OnDestroy()
	{
		Cursor.lockState = CursorLockMode.None;
		Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
	}

	public IEnumerable<Enemy> GetNearbyEnemies(Vector2 position, float radius)
	{
		List<Enemy> result = new();
        foreach (var item in Physics2D.OverlapCircleAll(position, radius, LayerMask.NameToLayer("Enemy")))
        {
			if(item.TryGetComponent(out Enemy enemy))
				result.Add(enemy);
        }
		return result;
	}
}
