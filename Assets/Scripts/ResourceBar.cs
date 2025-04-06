using UnityEngine;
using UnityEngine.UI;

public class ResourceBar : MonoBehaviour
{
	public Image image;

	public void SetPercentage(float percent)
	{
		var size = image.rectTransform.sizeDelta;
		size.x = percent;
		image.rectTransform.sizeDelta = size;
	}
}
