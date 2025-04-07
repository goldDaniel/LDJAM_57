using System.Collections;
using TreeEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

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

	public Rigidbody2D rb;

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

	public float currentXP = 0;
	public float xpNeeded = 0;
	public float xpScaling = 0;

	public float cascadeChance = 0;
	public float instakillChance = 0;
	public float retaliatePercent = 0;
	public float lifeSteal = 0;
	public float xpIncrease = 0;
	public float damageReduction = 1;
	public bool rampage = false;
	public bool equality = false;
	public bool shield = false;
	public bool currentShield = false;

	public int score = 0;
	public float streakDuration = 0;
	public float streakTime = 0;
	public int rampageCount = 0;
	public float rampageDamage = 0;
	public float equalitySpeed = 0;
	public float shieldCooldown = 0;

	public SpriteRenderer shieldSprite;

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		
		CurrentHealth = maxHealth.Modified();
		CurrentMana = maxMana.Modified();
	}

	private void OnEnable()
	{
		moveAction = InputSystem.actions.FindAction("Move");
		aimAction = InputSystem.actions.FindAction("Look");
		attackAction = InputSystem.actions.FindAction("Attack");
	}

	void Update()
	{
		moveAction.Enable();
		aimAction.Enable();
		attackAction.Enable();

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
					fireball.damage = fireballPrefab.damage.Modified();
					fireball.transform.position = this.transform.position;
					fireball.direction = attackDir;
					AudioManager.Instance.PlaySFX(AudioManager.Instance.fireballSwoosh);
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

		// equality
		{
			if (equality)
			{
				if(CurrentMana / maxMana.Modified() > CurrentHealth / maxHealth.Modified())
				{
					CurrentMana -= equalitySpeed * Time.deltaTime;
					CurrentHealth += equalitySpeed * Time.deltaTime;
				}
				else
				{
                    CurrentMana += equalitySpeed * Time.deltaTime;
                    CurrentHealth -= equalitySpeed * Time.deltaTime;
                }
			}
		}

		// rampage
		{
			if (rampage)
			{
				streakTime += Time.deltaTime;
				if (streakTime > streakDuration)
				{
					fireballPrefab.damage.Add(rampageCount * -1 * rampageDamage);
					rampageCount = 0;
					streakTime = 0;
				}
			}
		}
	}

	void FixedUpdate()
	{
		rb.AddForce(moveDir * Force.Modified());
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		colliderTouchCount++;
		if(collision.collider.gameObject.TryGetComponent(out Enemy hitEnemy))
		{
			ApplyDamage(hitEnemy.hitDamage,hitEnemy);
			foreach (var enemy in Game.Instance.GetNearbyEnemies(rb.position, collisionPushRadius))
			{
				enemy.OnPlayerHit(rb.position, collisionPushRadius);
			}
		}
	}

	public void ApplyDamage(float damage, Enemy source = null)
	{
		if (source != null)
		{
			source.ApplyDamage(damage * retaliatePercent);
		}
		if (currentShield)
		{
			StartCoroutine(getShield(shieldCooldown));
			shieldSprite.enabled = false;
		}
		else
		{
			CurrentHealth -= damage * damageReduction;
		}
	}
	private IEnumerator getShield(float cooldown)
	{
        yield return new WaitForSeconds(cooldown);
		currentShield = true;
		shieldSprite.enabled = true;
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
			manaCost.Add(perk.manaCost);
		if (!MathUtils.ApproximatelyZero(perk.manaCostMultiplier))
			manaCost.Mul(perk.manaCostMultiplier);
		if (!MathUtils.ApproximatelyZero(perk.maxManaAdditive))
			manaCost.Add(perk.manaCost);

		// cast speed
		if (!MathUtils.ApproximatelyZero(perk.castSpeed))
			attacksPerSecond.Mul(perk.castSpeed);

        // damage reduction
        if (!MathUtils.ApproximatelyZero(perk.damageReduction))
            damageReduction += perk.damageReduction;

        // madness
        if (!MathUtils.ApproximatelyZero(perk.madnessGained))
			madness += perk.madnessGained;

		if (!MathUtils.ApproximatelyZero(perk.cascadeChance))
			cascadeChance += perk.cascadeChance;
		if (!MathUtils.ApproximatelyZero(perk.instakill))
			instakillChance += perk.instakill;
		if (!MathUtils.ApproximatelyZero(perk.retaliate))
			retaliatePercent += perk.retaliate;
		if (!MathUtils.ApproximatelyZero(perk.lifesteal))
			lifeSteal += perk.lifesteal;
        if (!MathUtils.ApproximatelyZero(perk.XPIncrease))
            xpIncrease += perk.XPIncrease;
        if (!MathUtils.ApproximatelyZero(perk.shieldCooldown))
            shieldCooldown += perk.shieldCooldown;
        if (perk.rampage)
			rampage = perk.rampage;
		if (perk.equality)
			equality = perk.equality;
		if (perk.shield)
		{
			shield = perk.shield;
			currentShield = true;
			shieldSprite.enabled = true;
		}

		if (perk.perksGained > 0)
		{
			for (int i = 0; i < perk.perksGained; i++)
			{
				int index = Random.Range(0, Game.Instance.currentPerkPool.Count);
				ApplyPerk(Game.Instance.currentPerkPool[index]);
			}
		}
    }

	public void addXP(float xp)
	{
		currentXP += xp * (1 + xpIncrease);
		if (currentXP > xpNeeded)
		{
			currentXP -= xpNeeded;
			xpNeeded += xpScaling;
			PerkUIController.Instance.ActivatePerkSelection(true);
			CurrentMana = maxMana.Modified();
			CurrentHealth = maxHealth.Modified();
		}
	}
	public void GetKill()
	{
		score++;
		if (rampage)
		{
			rampageCount++;
			fireballPrefab.damage.Add(rampageDamage);
			streakTime = 0;
		}
	}
}
