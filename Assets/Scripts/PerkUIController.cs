
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PerkUIController : MonoBehaviour
{
	public static PerkUIController Instance { get; private set; }

	public List<PerkUI> uiElements;

	public CanvasGroup uiElementContainer;

	public TextMeshProUGUI reasonText;

	void Awake()
	{
		if (Instance != null)
		{
			Debug.LogError("Cannot have more than one PerkUIController. Something is wrong!");
			Destroy(this);
			return;
		}

		Instance = this;
		uiElementContainer.gameObject.SetActive(false);
	}

	public void ActivatePerkSelection(bool levelUp)
	{
		Game.Instance.IsPaused = true;

		if (levelUp) reasonText.text = "LEVEL UP";
		else         reasonText.text = "PERK PICKUP";

		var perks = Game.Instance.SelectPerks(uiElements.Count);
		for (int i = 0; i < uiElements.Count; ++i)
		{
			uiElements[i].Apply(perks[i]);
		}
		uiElementContainer.alpha = 0.0f;
		uiElementContainer.gameObject.SetActive(true);

		StartCoroutine(FadeInPerkSelection());
	}

	public void SelectPerk(PerkTemplate perk)
	{
		Game.Instance.RemovePerkFromPool(perk);
		Game.Instance.player.ApplyPerk(perk);

		uiElementContainer.alpha = 1.0f;
		StartCoroutine(FadeOutPerkSelection());
	}

	private IEnumerator FadeInPerkSelection()
	{
		Time.timeScale = 0;
		float alpha = uiElementContainer.alpha;
		while (alpha < 1.0f)
		{
			alpha += (1f / 60f) * (1.0f / 0.5f);
			uiElementContainer.alpha = alpha;
			yield return null;
		}
		uiElementContainer.alpha = 1.0f;
	}

	private IEnumerator FadeOutPerkSelection()
	{
		float alpha = uiElementContainer.alpha;
		while (alpha > 0.0f)
		{
			alpha -= (1f / 60f) * (1.0f / 0.5f);
			uiElementContainer.alpha = alpha;
			yield return null;
		}
		uiElementContainer.alpha = 0.0f;
		yield return null;
		uiElementContainer.gameObject.SetActive(false);
		Game.Instance.IsPaused = false;

		Time.timeScale = 1.0f;
	}
}
