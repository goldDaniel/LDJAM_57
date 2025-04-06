
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PerkUI : MonoBehaviour
{
	public Image icon;
	public TextMeshProUGUI titleText;
	public TextMeshProUGUI descriptionText;
	public Image borderImage;
	public Image decoration;

	void Apply(PerkTemplate template)
	{
		icon.sprite = template.icon;
		titleText.text = template.title;
		descriptionText.text = template.description;
		borderImage.color = template.borderColor;
		decoration.gameObject.SetActive(template.showDecoration);
	}
}