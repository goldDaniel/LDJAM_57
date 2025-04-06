using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	public ModifiableFloat maxMana;
	private float _currentMana;
	public float CurrentMana
	{
		get => _currentMana;
		set
		{
			float modifiedMax = maxMana.Modified();
			if (value >= modifiedMax)
			{
				value = modifiedMax;
			}

			if (_currentMana != value)
			{
				_currentMana = value;
				manaBar.SetPercentage(_currentMana / modifiedMax);
			}
		}
	}

	public ModifiableFloat manaRegenPerSecond = 5;
	public ModifiableInt manaCost = 1;

	public ModifiableInt maxHealth;
	private int _currentHealth;
	public int CurrentHealth
	{
		get => _currentHealth;
		set 
		{
			int modifiedMax = maxHealth.Modified();
			if (value >= modifiedMax)
			{
				value = modifiedMax;
			}

			if(value <= 0)
			{
				// kill player
			}

			if(_currentHealth != value)
			{
				_currentHealth = value;
				healthBar.SetPercentage(_currentHealth / (float)modifiedMax);
			}
		}
	}

	public ResourceBar healthBar;
	public ResourceBar manaBar;

	public ModifiableFloat Force = 25;

	private Rigidbody2D rb;

	private InputAction moveAction;
	private InputAction aimAction;
	private InputAction attackAction;

	private Vector2 moveDir = Vector2.zero;
	private int colliderTouchCount = 0;
	private bool touchingTerrain => colliderTouchCount > 0;

	public Fireball fireballPrefab;

	public ModifiableFloat attacksPerSecond = 0.1f;
	private float attackTimer = 0;

	[Range(2f, 20f)]
	public float collisionPushRadius = 10f;

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		moveAction = InputSystem.actions.FindAction("Move");
		aimAction = InputSystem.actions.FindAction("Look");
		attackAction = InputSystem.actions.FindAction("Attack");

		CurrentHealth = maxHealth.Modified();
		CurrentMana = maxMana.Modified();
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
				while (attackTimer <= 0 && CurrentMana >= manaCost.Modified())
				{
					CurrentMana -= manaCost.Modified();
					attackTimer += 1.0f / attacksPerSecond.Modified();

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
			CurrentMana += manaRegenPerSecond.Modified() * Time.deltaTime;
		}
	}

	void FixedUpdate()
	{
		rb.AddForce(moveDir * Force.Modified());
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
