using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneTransition
{
	FadeOut,
	WipeLeft,
}

public abstract class Transition : MonoBehaviour
{
	public virtual bool IsComplete { get; }

	public abstract void Setup(bool reverse);
}

public class SceneTransitions : MonoSingleton<SceneTransitions>
{
	private bool _inTransition = false;

	public Transition FadeOutTransition;
	public Transition LeftWipeTransition;

	private Transition TransitionFor(SceneTransition transition) => transition switch
	{
		SceneTransition.FadeOut => FadeOutTransition,
		SceneTransition.WipeLeft => LeftWipeTransition,
		_ => null
	};

	public void LoadScene(string scene, SceneTransition type)
	{
		if(!_inTransition)
		{
			_inTransition = true;
			StartCoroutine(TransitionToScene(scene, type));
		}
	}

	private IEnumerator TransitionToScene(string scene, SceneTransition type)
	{
		var transition = Instantiate(TransitionFor(type));
		DontDestroyOnLoad(transition);
		transition.Setup(false);

		while (!transition.IsComplete)
			yield return null;

		transition.Setup(true);

		var currentScene = SceneManager.GetActiveScene();
		var loadOp = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single);
		while (!loadOp.isDone)
			yield return null;

		while (!transition.IsComplete)
			yield return null;

		Destroy(transition.gameObject);
		_inTransition = false;
	}
}
