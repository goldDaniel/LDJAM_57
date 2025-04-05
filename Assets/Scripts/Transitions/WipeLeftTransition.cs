using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class WipeLeftTransition : Transition
{
	public GameObject wipeSquare;

	private bool _reverse;

	private float _t;

	private Vector3 _initial;
	private Vector3 _target;

	public override bool IsComplete => _t >= 1;

	public override void Setup(bool reverse)
	{
		_reverse = reverse;
		_t = 0;

		_initial = reverse ? new Vector3(0, 0, 0) : new Vector3(-20, 0, 0);
		_target = reverse ? new Vector3(-20, 0, 0) : new Vector3(0, 0, 0);


		var quadHeight = Camera.main.orthographicSize * 2.0f;
		var quadWidth = quadHeight * Screen.width / Screen.height;
		wipeSquare.transform.position = _initial;
		wipeSquare.transform.localScale = new Vector3(quadWidth, quadHeight, 1);
		GetComponent<AudioSource>().Play();
	}

	public void Update()
	{
		wipeSquare.transform.position = Vector3.Lerp(_initial, _target, Easing.InCirc(_t));
		_t += Time.deltaTime * 2.5f;
		_t = Mathf.Clamp01(_t);
	}
}
