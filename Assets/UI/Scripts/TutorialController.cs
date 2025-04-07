using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
	private int _currentIndex = 0;
	public List<GameObject> pages;

	public Button nextBtn;
	public Button prevBtn;
	public Button mainMenuBtn;

	void Awake()
	{
		foreach(var page in pages)
		{
			page.SetActive(false);
		}

		pages[_currentIndex].SetActive(true);
		nextBtn.interactable = (_currentIndex < pages.Count - 1);
		prevBtn.interactable = (_currentIndex > 0);
	}

	public void OnNextPressed()
	{
		pages[_currentIndex].SetActive(false);
		_currentIndex++;

		prevBtn.interactable = (_currentIndex > 0);
		nextBtn.interactable = (_currentIndex < pages.Count - 1);
		pages[_currentIndex].SetActive(true);
	}

	public void OnPrevPressed()
	{
		
		pages[_currentIndex].SetActive(false);
		_currentIndex--;

		prevBtn.interactable = (_currentIndex > 0);
		nextBtn.interactable = (_currentIndex < pages.Count - 1);
		pages[_currentIndex].SetActive(true);
	}

	public void OnMainMenuPressed()
	{
		SceneTransitions.Instance.LoadScene("MainMenu", SceneTransition.FadeOut);
	}
}