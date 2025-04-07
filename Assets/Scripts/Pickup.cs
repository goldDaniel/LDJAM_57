using System.Collections;
using UnityEngine;

public class Pickup : MonoBehaviour
{
	public enum pickupType { Level, Madness, Sanity}
	public pickupType type;
	public float pickupLife = 15;

    private void Start()
    {
		StartCoroutine(DestroyPickup(pickupLife));
    }

	private IEnumerator DestroyPickup(float duration)
	{
		yield return new WaitForSeconds(duration);
		Destroy(this.gameObject);
	}

    private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.tag == "Player")
		{
			if (type == pickupType.Level)
			{
				PerkUIController.Instance.ActivatePerkSelection(false);
			}
            if (type == pickupType.Madness)
            {
				Game.Instance.player.madness += Random.Range(0.05f, 0.16f);
				if(Game.Instance.player.madness > 1) { Game.Instance.player.madness = 1; }
            }
            if (type == pickupType.Sanity)
            {
				Game.Instance.player.madness -= 0.2f;
                if (Game.Instance.player.madness < 0) { Game.Instance.player.madness = 0; }
            }
			Destroy(this.gameObject);
        }
	}
}