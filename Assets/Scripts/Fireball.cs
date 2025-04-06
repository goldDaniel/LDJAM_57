using UnityEngine;

public class Fireball : RegisteredBehaviour<Fireball>
{
	[Range(0, 20f)]
	public float initialSpeed;

	[Range(0f, 100f)]
	public float acceleration;

	public Vector2 direction;

	public Rigidbody2D rb;
	public Collider2D collide;

	[SerializeField]
	private SpriteRenderer sr;

	public Explosion explosionPrefab;

	public ModifiableFloat damage = 1f;

	private new void Start()
	{
		base.Start();

		Debug.Assert(Mathf.Abs(direction.sqrMagnitude - 1.0f) < 0.01f, "Please normalize the direction before setting");

		rb.linearVelocity = direction * initialSpeed;
		sr.gameObject.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(rb.linearVelocityY, rb.linearVelocityX) * Mathf.Rad2Deg);
	}

	private void OnEnable()
	{
		collide.enabled = true;
		rb.simulated = true;
	}

	private void OnDisable()
	{
		collide.enabled = false;
		rb.simulated = false;
	}

	void FixedUpdate()
	{
		rb.linearVelocity += direction * acceleration * Time.deltaTime;
	}

	// when something hits a fireball
	void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.TryGetComponent<Enemy>(out var enemy))
		{
			enemy.ApplyDamage(damage.Modified());
			enemy.KnockBackFrom(rb.position, 30);
		}

		if(!collision.TryGetComponent(out CultistAttack _))
		{
			
			Destroy(this.gameObject);
			var explosion = Instantiate(explosionPrefab);
			explosion.transform.position = this.transform.position;

			Vector3 p = Camera.main.WorldToScreenPoint(this.transform.position);
			if(p.x > -10 && p.x < Screen.width + 10 &&
			   p.y > -10 && p.y < Screen.height + 10)
			{
				AudioManager.Instance.PlaySFX(AudioManager.Instance.fireballExplosion);
			}	
		}
	}
}
