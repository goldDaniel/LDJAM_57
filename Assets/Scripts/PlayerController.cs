using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	[Range(1, 100)]
	public int maxMana;
	private float _currentMana;
	public float CurrentMana
	{
		get => _currentMana;
		set
		{
			if (value >= maxMana)
			{
				value = maxMana;
			}

			if (_currentMana != value)
			{
				_currentMana = value;
				manaBar.SetPercentage(_currentMana / maxMana);
			}
		}
	}

	[Range(1f, 100f)]
	public float manaRegenPerSecond = 5;

	[Range(1, 100)]
	public int manaCost = 1;

	[Range(1, 100)]
	public int maxHealth;
	private int _currentHealth;
	public int CurrentHealth
	{
		get => _currentHealth;
		set 
		{
			if (value >= maxHealth)
			{
				value = maxHealth;
			}

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

	public ResourceBar healthBar;
	public ResourceBar manaBar;

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
		CurrentMana = maxMana;
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
				while (attackTimer <= 0 && CurrentMana >= manaCost)
				{
					CurrentMana -= manaCost;
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

		// mana regen
		{
			CurrentMana += manaRegenPerSecond * Time.deltaTime;
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
