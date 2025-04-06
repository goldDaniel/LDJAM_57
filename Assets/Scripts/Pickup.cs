using UnityEngine;

public class Pickup : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.tag == "Player")
		{
			PerkUIController.Instance.ActivatePerkSelection();
		}
	}
}