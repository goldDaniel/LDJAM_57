using UnityEngine;

public class CameraController : MonoBehaviour
{
	public Transform cam;

	public Transform target;

	[Range(1f, 25f)]
	public float decay;

	void Update()
	{
		cam.position = MathUtils.ExpDecay(cam.position.xy(), target.position.xy(), decay, Time.deltaTime);
	}
}
