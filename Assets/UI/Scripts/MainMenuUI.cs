using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuUI : MonoBehaviour
{
	public GameObject mainMenuFadeIn;
	private CanvasRenderer fadeImage; 

	public MeshRenderer mr;
	public UIDocument uiDoc;

	public AudioSource onHoverSound;
	public AudioSource onClickSound;

	private int prevScreenWidth;
	private int prevScreenHeight;

	IEnumerator Start()
	{
		fadeImage = mainMenuFadeIn.GetComponent<CanvasRenderer>();
		fadeImage.SetAlpha(1);
		fadeInTimer = 4f;

		prevScreenWidth = Screen.width;
		prevScreenHeight = Screen.height;

		var uiTarget = CreateFullScreenRT();
		uiDoc.panelSettings.targetTexture = uiTarget;
		mr.material.mainTexture = uiTarget;

		Button play = uiDoc.rootVisualElement.Q("PlayButton") as Button;
		play.RegisterCallback((PointerEnterEvent _) => onHoverSound.Play());
		play.RegisterCallback((ClickEvent _) =>
		{
			SceneTransitions.Instance.LoadScene("Gameplay", SceneTransition.FadeOut, () => Game.Instance.IsPaused = false);
			onClickSound.Play();
			AudioManager.Instance.SwitchToGameplay();
		});
		

		Button howToPlay = uiDoc.rootVisualElement.Q("HowToPlayButton") as Button;
		howToPlay.RegisterCallback((PointerEnterEvent _) => onHoverSound.Play());
		howToPlay.RegisterCallback((ClickEvent _) =>
		{
			SceneTransitions.Instance.LoadScene("Instructions", SceneTransition.FadeOut);
			onClickSound.Play();
		});

		var quadHeight = Camera.main.orthographicSize * 2.0f;
		var quadWidth = quadHeight * Screen.width / Screen.height;
		transform.localScale = new Vector3(quadWidth, quadHeight, 1);

		yield return null;
		mr.enabled = true;
	}

	private float fadeInTimer = 4f;
	void Update()
	{
		if(fadeInTimer > 0)
		{
			float t = fadeInTimer / 4f;

			fadeImage.SetAlpha(t);

			fadeInTimer -= Time.deltaTime;
			fadeInTimer = Mathf.Clamp01(fadeInTimer);
		}

		if (prevScreenWidth != Screen.width || prevScreenHeight != Screen.height)
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
		}

		var quadHeight = Camera.main.orthographicSize * 2.0f;
		var quadWidth = quadHeight * Screen.width / Screen.height;
		transform.localScale = new Vector3(quadWidth, quadHeight, 1);
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
