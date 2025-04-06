using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
	[Range(1, 100)]
	public int Health;

	[Range(0.01f, 1f)]
	public float hitDisplayTime = 0.05f;
	private float hitTimer = 0;

	public SpriteRenderer sr;
	public Rigidbody2D rb;

	public bool movementOverride = false;

	public void ApplyDamage(int damage)
	{
		hitTimer = hitDisplayTime;
		Health -= damage;
		if(Health <= 0)
		{
			Destroy(this.gameObject);
		}
	}

	public virtual void Update()
	{
		float t = hitTimer / hitDisplayTime;
		sr.color = Color.Lerp(Color.white, Color.red, t);
		hitTimer -= Time.deltaTime;
		hitTimer = Mathf.Clamp01(hitTimer);

		// intentional: do nothing when velocity is 0
		if (rb.linearVelocityX < 0)
			sr.flipX = true;
		else if (rb.linearVelocityX > 0)
			sr.flipX = false;

		if(MathUtils.ApproximatelyZero(rb.linearVelocity.sqrMagnitude, 0.1f))
		{
			movementOverride = false;
			rb.bodyType = RigidbodyType2D.Kinematic;
		}
	}

	public void OnPlayerHit(Vector2 position, float maxDistance)
	{
		movementOverride = true;
		Vector2 dir = (rb.position - position);
		float t = Mathf.Clamp01(1.0f - dir.magnitude / maxDistance);
		
		rb.bodyType = RigidbodyType2D.Dynamic;
		rb.linearDamping = 5;
		rb.AddForce(dir.normalized * t * 200);
	}
}