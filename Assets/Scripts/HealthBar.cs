using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
	public Image image;

	public void SetPercentage(float percent)
	{
		var size = image.rectTransform.sizeDelta;
		size.x = percent;
		image.rectTransform.sizeDelta = size;
	}
}
