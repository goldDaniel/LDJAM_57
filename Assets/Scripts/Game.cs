using UnityEngine;

public class Game : MonoBehaviour
{
	public PlayerController player;

	public Texture2D cursorTexture;

	public GameObject brainCrabPrefab;

	public Level level;

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

	// Update is called once per frame
	void Update()
	{
		
	}

	private void OnDestroy()
	{
		Cursor.lockState = CursorLockMode.None;
		Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
	}
}
