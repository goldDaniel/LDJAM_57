using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.XR;


public class BrainCrab : Enemy
{
	public Rigidbody2D rb;

	float noiseOffset;

	private void Awake()
	{
		noiseOffset = Random.value * 10.0f;
	}

	Vector2 GetSeparationVector(Rigidbody2D target)
	{
		var diff = rb.position - target.position;
		var diffLen = diff.magnitude;
		var scaler = Mathf.Clamp01(1.0f - diffLen / BrainSwarmController.Instance.neighborDist);
		return diff * (scaler / diffLen);
	}

	void Update()
	{	
		// intentional: do nothing when velocity is 0
		if (rb.linearVelocityX < 0)
			sr.flipX = true;
		else if (rb.linearVelocityX > 0)
			sr.flipX = false;
	}

	void FixedUpdate()
	{
		var currentPosition = rb.position;

		var noise = Mathf.PerlinNoise(Time.time, noiseOffset) * 2.0f - 1.0f;
		var speed = BrainSwarmController.Instance.velocity * (1.0f + noise * BrainSwarmController.Instance.velocityVariation);

		var separation = Vector2.zero;
		var alignment = rb.linearVelocity;
		var cohesion = rb.position;

		var crabs = BrainSwarmController.Instance.NearbyCrabs(this);

		foreach (var crab in crabs)
		{
			if (crab.gameObject == gameObject) 
				continue;

			var crabRB = crab.GetComponent<Rigidbody2D>();
			var t = crab.transform;
			separation += GetSeparationVector(crabRB);
			cohesion += crabRB.position;

			var crabVelocity = crabRB.linearVelocity;
			if(!float.IsNaN(crabVelocity.x) && !float.IsNaN(crabVelocity.y))
				alignment += crabVelocity;

		}

		separation *= BrainSwarmController.Instance.separationWeight;
		var avg = 1.0f / (crabs.Length == 0 ? 1 : crabs.Length);
		alignment *= avg * BrainSwarmController.Instance.alignmentWeight;
		cohesion *= avg;
		cohesion = (cohesion - currentPosition).normalized * BrainSwarmController.Instance.cohesionWeight;

		var target = (BrainSwarmController.Instance.target.position - currentPosition).normalized * BrainSwarmController.Instance.targetWeight;

		var direction = (separation + alignment + cohesion + target).normalized;
		rb.linearVelocity = direction * speed;
	}
}
