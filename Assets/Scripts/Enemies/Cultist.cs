
using Unity.VisualScripting;
using UnityEngine;

public class Cultist : Enemy
{
	[Range(1f, 100f)]
	public float Force;

	private enum CultistState
	{
		Moving,
		Attacking,
		Cooldown,
	}

	private CultistState _currentState = CultistState.Moving;

	public CultistAttack attackPrefab;

	private CultistAttack _activeAttack;

	[Range(1f, 100f)]
	public float attackRange;

	public float cooldownTime = 1;
	private float _cooldownTimer = 0;

	public float attackTime = 0;

	private Vector2 _targetP;

	public override void Update()
	{
		base.Update();

		if (movementOverride)
		{
			// cancel charge attack? 
			return;
		}

		if (_currentState == CultistState.Attacking)
		{
			rb.bodyType = RigidbodyType2D.Static;
			if (_activeAttack == null)
			{
				_activeAttack = Instantiate(attackPrefab);
				_activeAttack.owner = this;
				_activeAttack.transform.position = new Vector3(_targetP.x, _targetP.y, 0);
				_activeAttack.attackTime = attackTime;
				_activeAttack.onAttackComplete = () =>
				{
					this._currentState = CultistState.Cooldown;
					_cooldownTimer = cooldownTime;
				};
			}
		}
		else if (_currentState == CultistState.Cooldown)
		{
			rb.bodyType = DefaultBodyType;
			_cooldownTimer -= Time.deltaTime;
			if(_cooldownTimer <= 0)
			{
				_cooldownTimer = 0;
				_currentState = CultistState.Moving;
			}
		}
	}

	public void FixedUpdate()
	{
		if (_currentState != CultistState.Moving)
			return;

		if (movementOverride)
			return;


		rb.bodyType = DefaultBodyType;
		_targetP = Game.Instance.player.rb.position + Game.Instance.player.rb.linearVelocity;
		if(Vector2.Distance(_targetP, rb.position) <= attackRange)
		{
			_currentState = CultistState.Attacking;
		}

		Vector2 dir = (_targetP - this.rb.position).normalized;
		rb.AddForce(dir * Force);
	}


	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, attackRange);
	}

	public override void DropPickup()
	{
		var pickup = Instantiate(Game.Instance.pickups[1]);
		pickup.transform.position = transform.position;
	}
}