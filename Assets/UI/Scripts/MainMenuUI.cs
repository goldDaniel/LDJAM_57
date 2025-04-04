using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuUI : MonoBehaviour
{
	public MeshRenderer mr;
	public UIDocument uiDoc;

	public AudioSource audioSource;

	private int prevScreenWidth;
	private int prevScreenHeight;

	IEnumerator Start()
	{
		prevScreenWidth = Screen.width;
		prevScreenHeight = Screen.height;

		var uiTarget = CreateFullScreenRT();
		uiDoc.panelSettings.targetTexture = uiTarget;
		mr.material.mainTexture = uiTarget;

		Button play = uiDoc.rootVisualElement.Q("PlayButton") as Button;
		play.RegisterCallback((ClickEvent e) =>
		{
			SceneTransitions.Instance.LoadScene("Gameplay", SceneTransition.WipeLeft);
		});
		play.RegisterCallback((PointerEnterEvent e) =>
		{
			audioSource.Play();
		});

		var quadHeight = Camera.main.orthographicSize * 2.0f;
		var quadWidth = quadHeight * Screen.width / Screen.height;
		transform.localScale = new Vector3(quadWidth, quadHeight, 1);

		yield return null;
		mr.enabled = true;
	}

	void Update()
	{
		if(prevScreenWidth != Screen.width || prevScreenHeight != Screen.height)
		{
			prevScreenWidth = Screen.width;
			prevScreenHeight = Screen.height;

			// release old RT
			{
				var oldRT = uiDoc.panelSettings.targetTexture;
				uiDoc.panelSettings.targetTexture = null;
				oldRT.Release();
			}

			var uiTarget = CreateFullScreenRT();
			uiDoc.panelSettings.targetTexture = uiTarget;
			mr.material.mainTexture = uiTarget;

			var quadHeight = Camera.main.orthographicSize * 2.0f;
			var quadWidth = quadHeight * Screen.width / Screen.height;
			transform.localScale = new Vector3(quadWidth, quadHeight, 1);
		}
	}

	RenderTexture CreateFullScreenRT()
	{
		var rt = new RenderTexture(Screen.width, Screen.height, 32, RenderTextureFormat.ARGB32);
		rt.wrapMode = TextureWrapMode.Clamp;
		rt.filterMode = FilterMode.Bilinear;
		rt.antiAliasing = 8;
		rt.Create();
		{
			RenderTexture prevRT = RenderTexture.active;
			RenderTexture.active = rt;
			GL.Clear(true, true, Color.clear);
			RenderTexture.active = prevRT;
		}

		return rt;
	}
}
