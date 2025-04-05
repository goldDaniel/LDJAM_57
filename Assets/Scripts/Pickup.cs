using UnityEngine;

public class Pickup : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D collision)
	{
		// This can only happen with the player layer, but fireballs are a part of the player layer as well
		if (collision.gameObject.tag == "Player")
		{
			// TODO (danielg): trigger powerup menu
		}
	}
}