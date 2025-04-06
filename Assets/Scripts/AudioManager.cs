
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using static Unity.VisualScripting.Member;

public class AudioManager : MonoSingleton<AudioManager>
{
	public AudioSource mainMenuBackground;
	public AudioSource gameplayBackground;

	public AudioSource fireballSwoosh;

	public void StartMainMenuMusic()
	{
		mainMenuBackground.Play();
	}

	public void SwitchToGameplay() => StartCoroutine(SwitchToGameplayInternal());
	public void SwitchToMainMenu() => StartCoroutine(SwitchToMainMenuInternal());

	public void PlaySFX(AudioSource source) => source.PlayOneShot(source.clip);

	private IEnumerator SwitchToGameplayInternal()
	{
		if (mainMenuBackground.isPlaying)
			yield return FadeOutMusic(mainMenuBackground, 2f);

		if (!gameplayBackground.isPlaying)
			yield return FadeInMusic(gameplayBackground, 2f);
	}

	private IEnumerator SwitchToMainMenuInternal()
	{
		if (gameplayBackground.isPlaying)
			yield return FadeOutMusic(gameplayBackground, 2f);

		if (!mainMenuBackground.isPlaying)
			yield return FadeInMusic(mainMenuBackground, 2f);
	}

	private IEnumerator FadeInMusic(AudioSource source, float time)
	{
		float targetVolume = source.volume;
		source.volume = 0;
		source.Play();
		float t = 0;
		while (t < time)
		{
			yield return null;
			t += Time.deltaTime;
			source.volume = Mathf.Clamp01(t / time) * targetVolume;
		}
		source.volume = targetVolume;
	}

	private IEnumerator FadeOutMusic(AudioSource source, float time)
	{
		float restoreVolume = source.volume;
		source.Play();
		float t = time;
		while (t > 0)
		{
			yield return null;
			t -= Time.deltaTime;
			source.volume = Mathf.Clamp01(t / time) * restoreVolume;
		}

		source.Stop();
		source.volume = restoreVolume;
	}
}