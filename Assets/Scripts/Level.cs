using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
	[SerializeField]
	private Wall wallTemplate;

	[Range(0.1f, 3f)]
	public float wallWidth;

	[Range(10, 1000)]
	public float arenaWidth = 50;

	[Range(10, 1000)]
	public float arenaHeight = 30;

	public List<Wall> walls;

	void Awake()
	{
		if (walls.Capacity != 4)
		{
			foreach (var wall in walls)
			{
				Destroy(wall.gameObject);
			}
			walls.Clear();
			for (int i = 0; i < 4; ++i)
			{
				walls.Add(Instantiate(wallTemplate, this.transform));
			}
		}

		CreateLevelPerimeter();
	}

	void OnValidate()
	{
		if (walls.Count > 0)
			CreateLevelPerimeter();
	}

	private void CreateLevelPerimeter()
	{
		{
			var topWall = walls[0];
			topWall.transform.position = new Vector3(0, arenaHeight / 2, 0);
			topWall.transform.localScale = new Vector3(arenaWidth, wallWidth, 1);
		}

		{
			var bottomWall = walls[1];
			bottomWall.transform.position = new Vector3(0, -arenaHeight / 2, 0);
			bottomWall.transform.localScale = new Vector3(arenaWidth, wallWidth, 1);
		}

		{
			var rightWall = walls[2];
			rightWall.transform.position = new Vector3(arenaWidth / 2, 0, 0);
			rightWall.transform.localScale = new Vector3(wallWidth, arenaHeight, 1);
		}

		{
			var leftWall = walls[3];
			leftWall.transform.position = new Vector3(-arenaWidth / 2, 0, 0);
			leftWall.transform.localScale = new Vector3(wallWidth, arenaHeight, 1);
		}
	}
}
