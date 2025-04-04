
using UnityEngine;

public class FadeOutTransition : Transition
{
	public SpriteRenderer fadeSquare;

	private bool _reverse;

	private float _t;

	private float _initialOpacity;
	private float _targetOpacity;

	public override bool IsComplete => _t >= 1;

	public override void Setup(bool reverse)
	{
		_reverse = reverse;
		_t = 0;

		_initialOpacity = reverse ? 1 : 0;
		_targetOpacity = reverse ? 0 : 1;
	}

	public void Update()
	{
		Color color = fadeSquare.color;
		color.a = Mathf.Lerp(_initialOpacity, _targetOpacity, _t);
		fadeSquare.color = color;

		_t += Time.deltaTime;
		_t = Mathf.Clamp01(_t);
	}

}
