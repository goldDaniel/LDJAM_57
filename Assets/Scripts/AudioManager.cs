
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using static Unity.VisualScripting.Member;

public class AudioManager : MonoSingleton<AudioManager>
{
	public AudioSource mainMenuBackground;
	public AudioSource gameplayBackground;

	public void StartMainMenuMusic()
	{
		mainMenuBackground.Play();
	}

	public void SwitchToGameplay() => StartCoroutine(SwitchToGameplayInternal());
	public void SwitchToMainMenu() => StartCoroutine(SwitchToMainMenuInternal());

	private IEnumerator SwitchToGameplayInternal()
	{
		if (mainMenuBackground.isPlaying)
			yield return FadeOutMusic(mainMenuBackground, 1f);

		if (!gameplayBackground.isPlaying)
			yield return FadeInMusic(gameplayBackground, 1f);
	}

	private IEnumerator SwitchToMainMenuInternal()
	{
		if (gameplayBackground.isPlaying)
			yield return FadeOutMusic(gameplayBackground, 1f);

		if (!mainMenuBackground.isPlaying)
			yield return FadeInMusic(mainMenuBackground, 1f);
	}

	private IEnumerator FadeInMusic(AudioSource source, float time)
	{
		source.volume = 0;
		source.Play();
		float t = 0;
		while (t < time)
		{
			yield return null;
			t += Time.deltaTime;
			source.volume = Mathf.Clamp01(t / time);
		}
	}

	private IEnumerator FadeOutMusic(AudioSource source, float time)
	{
		source.volume = 1;
		source.Play();
		float t = time;
		while (t > 0)
		{
			yield return null;
			t -= Time.deltaTime;
			source.volume = Mathf.Clamp01(t / time);
		}

		source.Stop();
	}
}