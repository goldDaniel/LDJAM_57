using UnityEngine;

public class Game : MonoBehaviour
{
	public PlayerController player;

	public Texture2D cursorTexture;

	void Start()
	{
		Cursor.lockState = CursorLockMode.Confined;
		Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
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
