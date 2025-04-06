
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PerkUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
	public Image icon;
	public TextMeshProUGUI titleText;
	public TextMeshProUGUI descriptionText;
	public Image borderImage;
	public Image decoration;

	public PerkTemplate perkData { get; private set; }

	public void Apply(PerkTemplate template)
	{
		perkData = template;

		icon.sprite = template.icon;
		titleText.text = template.title;
		descriptionText.text = template.description;
		borderImage.color = template.borderColor;
		decoration.gameObject.SetActive(template.madnessThreshold > 0);
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		PerkUIController.Instance.SelectPerk(perkData);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		borderImage.rectTransform.localScale = Vector3.one * 1.02f;
		decoration.rectTransform.localScale = Vector3.one * 1.02f;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		borderImage.rectTransform.localScale = Vector3.one;
		decoration.rectTransform.localScale = Vector3.one;
	}
}