
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CultistAttack : RegisteredBehaviour<CultistAttack>
{
	public SpriteRenderer baseSprite;
	public SpriteRenderer glowSprite;

	public float attackTime;
	private float _attackTimer = 0;

	public SpriteRenderer bombEffectPrefab;
	private List<SpriteRenderer> bombEffect = new();

	public Action onAttackComplete;

	public CircleCollider2D circle2D;

	private bool _playerInsideExplosion = false;
	private bool _damageApplied = false;

	public Cultist owner;
	public GameObject indicator;

	protected override void Start()
	{
		base.Start();
		StartCoroutine(Explode());

		for(int i = 0; i < 20; ++i)
		{
			var effect = Instantiate(bombEffectPrefab);
			effect.gameObject.SetActive(true);
			effect.transform.parent = this.transform;
			effect.transform.localScale = Vector3.one * 0.75f;
			effect.transform.localPosition = new Vector3(0, 0.05f * i, 0f);
			effect.enabled = false;

			bombEffect.Add(effect);
		}
	}

	public void Update()
	{
		if(owner != null && owner)
		{
			Vector2 dir = owner.transform.position - indicator.transform.position;
			float angleRad = Mathf.Atan2(dir.y, dir.x);
			indicator.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * angleRad);
		}
		else 
		{
			indicator.SetActive(false);	
		}
	}

	IEnumerator Explode()
	{
		bool soundPlayed = false;
		while(_attackTimer < attackTime)
		{
			float t = _attackTimer / attackTime;
			{
				var baseColor = baseSprite.color;
				baseColor.a = t;
				baseSprite.color = baseColor;
			}
			{
				var glowColor = glowSprite.color;
				glowColor.a = t;
				glowSprite.color = glowColor;
			}

			if(t > 0.3f && !soundPlayed)
			{
				soundPlayed = true;
				AudioManager.Instance.PlaySFX(AudioManager.Instance.cultistAttackCharge);
			}

			float scale = Mathf.Lerp(1f, 1.2f, t);
			glowSprite.transform.localScale = Vector3.one * scale;

			_attackTimer += Time.deltaTime;
			yield return null;
		}

		AudioManager.Instance.PlaySFX(AudioManager.Instance.cultistAttackExplode);
		glowSprite.color = Color.white;
		for (int i = 0; i < bombEffect.Count; ++i)
		{
			bombEffect[i].enabled = true;
			yield return new WaitForSeconds(1.0f / 120f);
		}

		if (_playerInsideExplosion && !_damageApplied)
		{
			if(Game.Instance.player.ApplyDamage(10))
				_damageApplied = true;
		}
			

		Destroy(this.gameObject);
		onAttackComplete?.Invoke();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{

		if(Game.Instance.player && collision.gameObject == Game.Instance.player.gameObject) 
			_playerInsideExplosion = true;
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (Game.Instance.player && collision.gameObject == Game.Instance.player.gameObject)
			_playerInsideExplosion = false;
	}
}