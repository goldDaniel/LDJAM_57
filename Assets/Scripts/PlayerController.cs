using System.Linq;
using TreeEditor;
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
	public ModifiableFloat manaCost = 1;

	public ModifiableFloat healthRegenPerSecond = 0;

	public ModifiableFloat maxHealth;
	private float _currentHealth;
	public float CurrentHealth
	{
		get => _currentHealth;
		set 
		{
			float modifiedMax = maxHealth.Modified();
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

	[Range(0f, 1f)]
	public float madness = 0f;

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

		// health regen
		{
			CurrentHealth += healthRegenPerSecond.Modified() * Time.deltaTime;
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

	public void ApplyPerk(PerkTemplate perk)
	{
		// health
		if (!MathUtils.ApproximatelyZero(perk.maxHealthMultiplier))
			maxHealth.Mul(perk.maxHealthMultiplier);
		if (!MathUtils.ApproximatelyZero(perk.maxManaAdditive))
			maxHealth.Add(perk.maxManaAdditive);
		if (!MathUtils.ApproximatelyZero(perk.healthRegen))
			healthRegenPerSecond.Add(perk.healthRegen);

		// damage
		if (!MathUtils.ApproximatelyZero(perk.damage))
			fireballPrefab.damage.Add(perk.damage);
		if (!MathUtils.ApproximatelyZero(perk.damageMultiplier))
			fireballPrefab.damage.Mul(perk.damageMultiplier);

		// movement speed
		if (!MathUtils.ApproximatelyZero(perk.movementSpeed))
			Force.Mul(perk.movementSpeed);

		// mana 
		if (!MathUtils.ApproximatelyZero(perk.manaRegen))
			manaRegenPerSecond.Mul(perk.manaRegen);
		if (!MathUtils.ApproximatelyZero(perk.manaCost))
			manaCost.Mul(perk.manaCost);
		if (!MathUtils.ApproximatelyZero(perk.maxManaAdditive))
			manaCost.Add(perk.manaCost);

		// cast speed
		if (!MathUtils.ApproximatelyZero(perk.castSpeed))
			attacksPerSecond.Mul(perk.castSpeed);

		// madness
		if (!MathUtils.ApproximatelyZero(perk.madnessGained))
			madness += perk.madnessGained;
	}
}
