using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public abstract class Enemy : RegisteredBehaviour<Enemy>
{
	[Range(1, 100)]
	public float Health;

	[Range(0.01f, 1f)]
	public float hitDisplayTime = 0.05f;
	private float hitTimer = 0;

	public SpriteRenderer sr;
	public Rigidbody2D rb;
	public Collider2D collide;
	public float xpGain;
	public float hitDamage;

	public bool movementOverride = false;

	public RigidbodyType2D DefaultBodyType = RigidbodyType2D.Kinematic;

	public void ApplyDamage(float damage)
	{
        hitTimer = hitDisplayTime;
		Game.Instance.player.CurrentHealth += damage * Game.Instance.player.lifeSteal;
		// Need to add logic to not apply to bosses
		if (UnityEngine.Random.Range(0f, 1f) < Game.Instance.player.instakillChance)
		{
			this.onDeath();
			Destroy(this.gameObject);
		}
		else
		{
			Health -= damage;
			if (Health <= 0)
			{
				this.onDeath();
				Destroy(this.gameObject);
			}
		}
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
			rb.bodyType = DefaultBodyType;
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

	public void KnockBackFrom(Vector2 position, float force)
	{
		movementOverride = true;
		Vector2 dir = (rb.position - position).normalized;
		rb.bodyType = RigidbodyType2D.Dynamic;
		rb.linearDamping = 6;
		rb.AddForce(dir.normalized * force);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}
	public virtual void onDeath()
	{
        Game.Instance.player.addXP(xpGain);
        DropPickup();
		float cascadeChance = Game.Instance.player.cascadeChance;
		if (UnityEngine.Random.Range(0f, 1f) < cascadeChance)
		{
			for (int i = 0; i < 6; i++)
			{
				var fireball = Instantiate(Game.Instance.player.fireballPrefab);
				fireball.transform.position = this.transform.position;
				var angle = Mathf.Deg2Rad * (60 * i);
				fireball.direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
			}
		}
		Game.Instance.player.GetKill();
    }
	public virtual void DropPickup() { }
}