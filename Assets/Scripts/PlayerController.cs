using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	[Range(1f, 200f)]
	public float Force = 25;

	private Rigidbody2D rb;

	private InputAction moveAction;
	private InputAction aimAction;

	private Vector2 moveDir = Vector2.zero;
	private float desiredMoveAngle = 0f;
	private int colliderTouchCount = 0;
	private bool touchingTerrain => colliderTouchCount > 0;

	void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		moveAction = InputSystem.actions.FindAction("Move");
		aimAction = InputSystem.actions.FindAction("Look");
	}

	void Update()
	{
		// Aim
		{
			Vector2 aimDir = aimAction.ReadValue<Vector2>();
			if (aimDir.sqrMagnitude > 0)
			{
				
			}
			else
			{

			}
		}

		// Move
		{
			moveDir = moveAction.ReadValue<Vector2>();
		}
	}

	void FixedUpdate()
	{
		rb.AddForce(moveDir * Force);
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		colliderTouchCount++;
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		colliderTouchCount--;
	}
}
