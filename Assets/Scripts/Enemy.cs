using UnityEngine;

public class Enemy : MonoBehaviour
{
	[Range(1, 100)]
	public int Health;

	[Range(0.01f, 1f)]
	public float hitDisplayTime = 0.05f;
	private float hitTimer = 0;

	public SpriteRenderer sr;

	public void ApplyDamage(int damage)
	{
		hitTimer = hitDisplayTime;
		this.enabled = true;

		Health -= damage;
		if(Health <= 0)
		{
			Destroy(this.gameObject);
		}
	}

	void Update()
	{
		float t = hitTimer / hitDisplayTime;
		sr.color = Color.Lerp(Color.white, Color.red, t);
		hitTimer -= Time.deltaTime;
		hitTimer = Mathf.Clamp01(hitTimer);

		if(hitTimer == 0)
		{
			this.enabled = false;
		}
	}
}