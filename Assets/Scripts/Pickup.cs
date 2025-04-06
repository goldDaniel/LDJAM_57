using UnityEngine;

public class Pickup : MonoBehaviour
{
	public enum pickupType { Level, Madness, Sanity}
	public pickupType type;
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.tag == "Player")
		{
			if (type == pickupType.Level)
			{
				PerkUIController.Instance.ActivatePerkSelection();
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