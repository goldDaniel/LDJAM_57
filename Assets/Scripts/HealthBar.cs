using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
	public Image image;

	void Awake()
	{
		image.material = new Material(image.material); // by copy plz
		image.material.SetTexture("_MainTexture", image.sprite.texture);
	}

	public void SetPercentage(float percent)
	{
		image.material.SetFloat("_UV_Clip", percent);
	}
}
