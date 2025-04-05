using UnityEditor.ShaderGraph;
using UnityEngine;

public class Fireball : MonoBehaviour
{
	[Range(0, 20f)]
	public float initialSpeed;

	[Range(0f, 100f)]
	public float acceleration;

	public Vector2 direction;

	private Rigidbody2D rb;

	void Start()
	{
		Debug.Assert(Mathf.Abs(direction.sqrMagnitude - 1.0f) < 0.01f, "Please normalize the direction before setting");

		rb = GetComponent<Rigidbody2D>();
		rb.linearVelocity = direction * initialSpeed;
	}

	void FixedUpdate()
	{
		rb.linearVelocity += direction * acceleration * Time.deltaTime;
	}

	// when something hits a fireball
	void OnTriggerEnter2D(Collider2D collision)
	{
		Destroy(this.gameObject);
	}
}
