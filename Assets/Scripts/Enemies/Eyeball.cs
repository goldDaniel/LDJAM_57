
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Eyeball : Enemy
{
	[Range(1f, 100f)]
	public float Force;

	private enum EyeballState
	{
		Moving,
		Attacking,
		Cooldown,
	}

	private EyeballState _currentState = EyeballState.Moving;

	public GameObject attackPrefab;
	public Collider2D attackCollider;

	private GameObject _activeAttack;

	private Color defaultColor;
	public SpriteRenderer attackSprite;

	[Range(1f, 100f)]
	public float attackRange;

	public int attackDamage = 15;

	public float attackTime = 3;
	private float _attackTimer = 0;

	public float cooldownTime = 1;
	private float _cooldownTimer = 0;

	void Awake()
	{
		attackPrefab.SetActive(false);

		_activeAttack = Instantiate(attackPrefab);
		_activeAttack.transform.SetParent(this.transform);
		_activeAttack.SetActive(false);
		attackCollider = _activeAttack.GetComponentInChildren<BoxCollider2D>();
		attackSprite = _activeAttack.GetComponentInChildren<SpriteRenderer>();
		defaultColor = attackSprite.color;
	}

	public override void Update()
	{
		base.Update();
		attackCollider.enabled = false;
		if (_currentState == EyeballState.Attacking)
		{
			_attackTimer -= Time.deltaTime;
			
			if(_attackTimer > attackTime / 2)
			{
				Vector2 dir = (Game.Instance.player.rb.position - rb.position).normalized;
				float desiredAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
				float currentAngle = _activeAttack.transform.localEulerAngles.z;
				_activeAttack.transform.rotation = Quaternion.Euler(0, 0, MathUtils.ExpDecayAngle(currentAngle, desiredAngle, 5f, Time.deltaTime));
			}

			if(_attackTimer <= 0.3f)
			{
				if(attackSprite.color != Color.white)
				{
					attackCollider.enabled = true;
					attackSprite.color = Color.white;
				}
			}

			if (_attackTimer <= 0)
			{
				attackCollider.enabled = false;
				attackSprite.color = defaultColor;
				_attackTimer = 0;
				_currentState = EyeballState.Cooldown;
				_cooldownTimer = cooldownTime;
			}
		}

		if (_currentState == EyeballState.Cooldown)
		{
			_activeAttack.gameObject.SetActive(false);
			_cooldownTimer -= Time.deltaTime;
			if (_cooldownTimer <= 0)
			{
				_cooldownTimer = 0;
				_currentState = EyeballState.Moving;
			}
		}
	}

	void SetupAttack()
	{
		_attackTimer = attackTime;

		_activeAttack.transform.GetChild(0).localScale = new Vector3(attackRange, 1, 1);
		_activeAttack.transform.GetChild(0).localPosition = new Vector3(attackRange / 2.0f, 0, 0);
		Vector2 dir = (Game.Instance.player.rb.position - rb.position).normalized;
		float desiredAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		_activeAttack.transform.rotation = Quaternion.Euler(0, 0, desiredAngle);
		_activeAttack.gameObject.SetActive(true);
	}

	public void FixedUpdate()
	{
		if(_currentState != EyeballState.Attacking && _currentState != EyeballState.Cooldown)
		{
			Vector2 target = Game.Instance.player.rb.position;
			if (Vector2.Distance(target, rb.position) <= (attackRange - 10))
			{
				_currentState = EyeballState.Attacking;
				SetupAttack();
			}
		}

		Vector2 dir = (Game.Instance.player.rb.position - this.rb.position).normalized;
		float scalar = (_currentState == EyeballState.Attacking || _currentState == EyeballState.Cooldown) ? 0.1f : 1f;
		rb.AddForce(dir * Force * scalar);
	}


	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, attackRange);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.gameObject.tag == "Player")
		{
			Game.Instance.player.ApplyDamage(attackDamage);
			this.attackCollider.enabled = false;
		}
	}
}