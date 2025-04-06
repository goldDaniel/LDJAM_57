using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	[Range(1, 100)]
	public int maxHealth;

	private int _currentHealth;
	public int CurrentHealth
	{
		get => _currentHealth;
		set 
		{
			if(value <= 0)
			{
				// kill player
			}


			if(_currentHealth != value)
			{
				_currentHealth = value;
				healthBar.SetPercentage((float)CurrentHealth / (float)maxHealth);
			}
		}
	}

	public HealthBar healthBar;

	[Range(1f, 200f)]
	public float Force = 25;

	private Rigidbody2D rb;

	private InputAction moveAction;
	private InputAction aimAction;
	private InputAction attackAction;

	private Vector2 moveDir = Vector2.zero;
	private int colliderTouchCount = 0;
	private bool touchingTerrain => colliderTouchCount > 0;

	public Fireball fireballPrefab;

	[Range(0.1f, 100f)]
	public float attacksPerSecond = 0.1f;
	private float attackTimer = 0;

	[Range(2f, 20f)]
	public float collisionPushRadius = 10f;

	void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		moveAction = InputSystem.actions.FindAction("Move");
		aimAction = InputSystem.actions.FindAction("Look");
		attackAction = InputSystem.actions.FindAction("Attack");

		CurrentHealth = maxHealth;
	}

	void Update()
	{
		// Aim
		{
			if (attackTimer > 0)
				attackTimer -= Time.deltaTime;

			Vector2 aimDir = aimAction.ReadValue<Vector2>();
			Vector2 cursorPosWorld = Camera.main.ScreenToWorldPoint(aimDir).xy();
			if (attackAction.ReadValue<float>() > 0)
			{
				while (attackTimer <= 0)
				{
					attackTimer += 1.0f / attacksPerSecond;

					// spawn projectile here
					Vector2 attackDir = (cursorPosWorld - this.transform.position.xy()).normalized;
					var fireball = Instantiate(fireballPrefab);
					fireball.transform.position = this.transform.position;
					fireball.direction = attackDir;
				}
			}
		}

		// Move
		{
			moveDir = moveAction.ReadValue<Vector2>();
		}
	}

	void FixedUpdate()
	{
		rb.AddForce(moveDir * Force);
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		colliderTouchCount++;
		if(collision.collider.gameObject.TryGetComponent(out Enemy _))
		{
			CurrentHealth--;
			foreach (var enemy in Game.Instance.GetNearbyEnemies(rb.position, collisionPushRadius))
			{
				enemy.OnPlayerHit(rb.position, collisionPushRadius);
			}
		}
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		colliderTouchCount--;
	}
}
