
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

		var quadHeight = Camera.main.orthographicSize * 2.0f;
		var quadWidth = quadHeight * Screen.width / Screen.height;
		fadeSquare.transform.position = Vector3.zero;	
	}

	public void Update()
	{
		var quadHeight = Camera.main.orthographicSize * 2.0f;
		var quadWidth = quadHeight * Screen.width / Screen.height;
		fadeSquare.transform.localScale = new Vector3(quadWidth, quadHeight, 1);

		Color color = fadeSquare.color;
		color.a = Mathf.Lerp(_initialOpacity, _targetOpacity, _t);
		fadeSquare.color = color;

		_t += Time.deltaTime;
		_t = Mathf.Clamp01(_t);
	}

}
