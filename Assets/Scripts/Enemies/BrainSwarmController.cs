using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BrainSwarmController : MonoSingleton<BrainSwarmController>
{
	public Rigidbody2D target;

	public GameObject brainPrefab;

	public float spawnRadius = 4.0f;

	[Range(0.1f, 20.0f)]
	public float velocity = 6.0f;

	[Range(0.0f, 0.9f)]
	public float velocityVariation = 0.5f;

	[Range(0.1f, 50.0f)]
	public float neighborDist = 10.0f;

	[Range(0.1f, 10f)]
	public float separationWeight = 0.5f;

	[Range(0.1f, 10f)]
	public float alignmentWeight = 4f;

	[Range(0.1f, 10f)]
	public float cohesionWeight = 2f;

	[Range(0.1f, 10f)]
	public float targetWeight = 8f;

	private LayerMask brainLayer;

	public override void Awake()
	{
		base.Awake();
		brainLayer = LayerMask.NameToLayer("Enemy");
	}

	public GameObject Spawn()
	{
		return Spawn(transform.position + Random.insideUnitSphere * spawnRadius);
	}

	public GameObject Spawn(Vector3 position)
	{
		var boid = Instantiate(brainPrefab, position, Quaternion.identity);
		return boid;
	}

	public List<Collider2D> NearbyCrabs(BrainCrab current)
	{
		return Physics2D.OverlapCircleAll(current.rb.position, neighborDist * 4, brainLayer).Where(c => c.TryGetComponent(typeof(Enemy), out _)).ToList();
	}
}