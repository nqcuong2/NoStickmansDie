using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{

	public static UIController instance;

	public Text scoreText;
	public Text bestScoreText;
	public GameObject baseBG;
	public Sprite[] baseBGs;
	public Canvas masterCanvas;
	public ScrollRect tutScRect;
	public Sprite[] toggleBtn;
	public Image[] soundNMusic;

	//Ipad Scale
	public RectTransform startBG;
	public Transform gameplayBG;
	public GameObject[] groundControllers;
	public RectTransform[] inputControllers;

	Color[,] allColors;
	GameModel.Gamestate stateB4;

	void Awake ()
	{
		instance = this;
		allColors = new Color[5, 5] 
		{ 
			{	//RedColors
				new Color (0.9411f, 0.8197f, 0.4666f, 1),
				new Color (0.9333f, 0.7019f, 0.3019f, 1),
				new Color (0.7333f, 0.4039f, 0.2313f, 1),
				new Color (0.6784f, 0.2745f, 0.2078f, 1),
				new Color (0.6666f, 0.2117f, 0.2156f, 1)
			}, 
			{	//BlueColors
				new Color (0.6156f, 0.9764f, 0.9725f, 1),
				new Color (0.4745f, 0.8313f, 0.9019f, 1),
				new Color (0.2823f, 0.6078f, 0.7647f, 1),
				new Color (0.1882f, 0.3686f, 0.6627f, 1),
				new Color (0.1822f, 0.2862f, 0.6471f, 1)
			}, 
			{	//GreenColors
				new Color (0.8039f, 0.9803f, 0.4235f, 1),
				new Color (0.6313f, 0.8941f, 0.349f, 1),
				new Color (0.3764f, 0.7058f, 0.2666f, 1),
				new Color (0.2039f, 0.5372f, 0.2117f, 1),
				new Color (0.1098f, 0.3764f, 0.1529f, 1)
			}, 
			{	//EvilColors
				new Color (0.8313f, 0.7803f, 0.847f, 1),
				new Color (0.6862f, 0.6196f, 0.7529f, 1),
				new Color (0.4705f, 0.4274f, 0.5607f, 1),
				new Color (0.2784f, 0.2431f, 0.349f, 1),
				new Color (0.1725f, 0.149f, 0.2431f, 1)
			},
			{	//PinkColors
				new Color (0.9215f, 0.6823f, 0.945f, 1),
				new Color (0.8392f, 0.5058f, 0.9137f, 1),
				new Color (0.6862f, 0.3372f, 0.8313f, 1),
				new Color (0.4588f, 0.1882f, 0.6784f, 1),
				new Color (0.349f, 0.1333f, 0.6431f, 1)
			}
		};
	}

	// Use this for initialization
	void Start () {
		UpdateScreen ();
		Input.multiTouchEnabled = false;
	}

	public void UpdateScreen() {
		Vector2 targetAspect1 = new Vector2(9, 16);
		Vector2 targetAspect2 = new Vector2 (6, 8);

		// Determine ratios of screen/window & target, respectively.
		float screenRatio = Screen.width / (float)Screen.height;
		float targetRatio1 = targetAspect1.x / targetAspect1.y; 
		float targetRatio2 = targetAspect2.x / targetAspect2.y;

		if (!Mathf.Approximately (screenRatio, targetRatio2)) {
			masterCanvas.renderMode = RenderMode.WorldSpace;

			for (int i = 0; i < groundControllers.Length; i++) {
				foreach (Transform child in groundControllers[i].transform) {
					child.gameObject.GetComponent<BoxCollider2D> ().offset = new Vector2(-0.5859184f, child.gameObject.GetComponent<BoxCollider2D> ().offset.y);
					child.gameObject.GetComponent<BoxCollider2D> ().size = new Vector2 (6.796619f, child.gameObject.GetComponent<BoxCollider2D> ().size.y);
				}
			}

			if (Mathf.Approximately (screenRatio, targetRatio1)) {
				// Screen or window is the target aspect ratio: use the whole area.
				GameController.instance.mainCam.GetComponent<Camera> ().rect = new Rect (0, 0, 1, 1);
			} else if (screenRatio > targetRatio1 || screenRatio > targetRatio2) {
				// Screen or window is wider than the target: pillarbox.
				float normalizedWidth = targetRatio1 / screenRatio;
				float barThickness = (1f - normalizedWidth) / 2f;
				GameController.instance.mainCam.GetComponent<Camera> ().rect = new Rect (barThickness, 0, normalizedWidth, 1);
			} else {
				// Screen or window is narrower than the target: letterbox.
				float normalizedHeight = screenRatio / targetRatio1;
				float barThickness = (1f - normalizedHeight) / 2f;
				GameController.instance.mainCam.GetComponent<Camera> ().rect = new Rect (0, barThickness, 1, normalizedHeight);
			}
		} else {
			masterCanvas.renderMode = RenderMode.ScreenSpaceCamera;

			startBG.offsetMin = new Vector2 (startBG.offsetMin.x, -35);
			startBG.offsetMax = new Vector2 (startBG.offsetMax.x, 30);

			gameplayBG.localScale = new Vector3 (1.05f, gameplayBG.localScale.y, gameplayBG.localScale.z);

			for (int i = 0; i < groundControllers.Length; i++) {
				foreach (Transform child in groundControllers[i].transform) {
					child.gameObject.GetComponent<BoxCollider2D> ().offset = new Vector2(-0.8360658f, child.gameObject.GetComponent<BoxCollider2D> ().offset.y);
					child.gameObject.GetComponent<BoxCollider2D> ().size = new Vector2 (9.196086f, child.gameObject.GetComponent<BoxCollider2D> ().size.y);
				}
			}

			float posY = 0;
			float height = 0;

			for (int i = 0; i < inputControllers.Length; i++) {
				if (i == 0) {
					posY = -41.17002f;
					height = 296.03f;
				} else if (i == 1) {
					posY = -90.12f;
					height = 193.42f;
				} else if (i == 2) {
					posY = -115.38f;
					height = 144.41f;
				} else {
					posY = -130.16f;
					height = 115.05f;
				}

				foreach (RectTransform child in inputControllers[i]) {
					if (child.gameObject.tag == "First") {
						child.sizeDelta = new Vector2 (Screen.width, height);
						child.localPosition = new Vector2 (child.localPosition.x, posY);
						posY += height;
					} else {
						child.sizeDelta = new Vector2 (Screen.width, height);
						child.localPosition = new Vector2 (child.localPosition.x, posY);
						posY += height;
					}
				}
			}
		}
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			switch (GameController.instance.GameModel.GameState) {
			case GameModel.Gamestate.PLAY:
				DoozyUI.UIManager.ShowUiElement ("OverlayPanel");
				DoozyUI.UIManager.ShowUiElement ("PausePanel");
				Time.timeScale = 0;
				Input.multiTouchEnabled = false;
				GameController.instance.GameModel.GameState = GameModel.Gamestate.PAUSE;
				break;
			case GameModel.Gamestate.PAUSE:
				DoozyUI.UIManager.HideUiElement ("PausePanel");
				DoozyUI.UIManager.HideUiElement ("OverlayPanel");
				Time.timeScale = 1;
				Input.multiTouchEnabled = true;
				GameController.instance.GameModel.GameState = GameModel.Gamestate.PLAY;
				break;
			case GameModel.Gamestate.OVER:
				DoozyUI.UIManager.ShowUiElement ("StartPanel");
				GameController.instance.GameModel.GameState = GameModel.Gamestate.START;
				onClickToMenu ();
				break;
			case GameModel.Gamestate.START:
				DoozyUI.UIManager.ShowUiElement ("TitlePanel");
				GameController.instance.GameModel.GameState = GameModel.Gamestate.TITLE;
				break;
			case GameModel.Gamestate.TITLE:
				DoozyUI.UIManager.ShowUiElement ("OverlayPanel");
				DoozyUI.UIManager.ShowUiElement ("QuitPanel");
				GameController.instance.GameModel.GameState = GameModel.Gamestate.QUIT;
				break;
			case GameModel.Gamestate.QUIT:
				DoozyUI.UIManager.HideUiElement ("QuitPanel");
				DoozyUI.UIManager.HideUiElement ("OverlayPanel");
				GameController.instance.GameModel.GameState = GameModel.Gamestate.TITLE;
				break;
			case GameModel.Gamestate.TUT:
				DoozyUI.UIManager.HideUiElement ("TutorialPanel");
				GameController.instance.GameModel.GameState = GameModel.Gamestate.PAUSE;
				break;
			case GameModel.Gamestate.SETTINGS:
				DoozyUI.UIManager.HideUiElement ("SettingsPanel");
				DoozyUI.UIManager.HideUiElement ("OverlayPanel");

				if (stateB4 == GameModel.Gamestate.TITLE) {
					GameController.instance.GameModel.GameState = GameModel.Gamestate.TITLE;
				} else {
					GameController.instance.GameModel.GameState = GameModel.Gamestate.PAUSE;
				}

				break;
			}
		}
	}

	public void UpdateScore(float score) {
		scoreText.text = string.Format("{0:0.00}", score);
	}

	public void onClickToMenu() {
		DoozyUI.UIManager.HideUiElement ("OverlayPanel");
		DoozyUI.UIManager.HideUiElement ("GameOverPanel");
		DoozyUI.UIManager.HideUiElement ("PausePanel");
		DoozyUI.UIManager.HideUiElement ("PlayPanel");
		DoozyUI.UIManager.ShowUiElement ("StartPanel");
		GameController.instance.GameModel.GameState = GameModel.Gamestate.START;
		GameController.instance.OnToMenu ();
	}

	public void onClickPause() {
		DoozyUI.UIManager.ShowUiElement ("OverlayPanel");
		DoozyUI.UIManager.ShowUiElement ("PausePanel");
		Input.multiTouchEnabled = false;
		ScrollSnapRect.instance.DecelerateTime = Time.deltaTime;
		Time.timeScale = 0;
		GameController.instance.GameModel.GameState = GameModel.Gamestate.PAUSE;
	}

	public void onClickContinue() {
		DoozyUI.UIManager.HideUiElement ("OverlayPanel");
		DoozyUI.UIManager.HideUiElement ("PausePanel");
		Time.timeScale = 1;
		Input.multiTouchEnabled = true;
		GameController.instance.GameModel.GameState = GameModel.Gamestate.PLAY;
	}

	public void onClickRestart() {
		DoozyUI.UIManager.HideUiElement ("OverlayPanel");
		DoozyUI.UIManager.HideUiElement ("PausePanel");
		DoozyUI.UIManager.HideUiElement ("GameOverPanel");
		GameController.instance.OnRestart ();
		Input.multiTouchEnabled = true;
		GameController.instance.GameModel.GameState = GameModel.Gamestate.PLAY;
	}

	public void onClickYes() {
		Application.Quit ();
	}

	public void onClickNo() {
		DoozyUI.UIManager.HideUiElement ("QuitPanel");
		DoozyUI.UIManager.HideUiElement ("OverlayPanel");
		GameController.instance.GameModel.GameState = GameModel.Gamestate.START;
	}

	public void onClickTutorial() {
		stateB4 = GameController.instance.GameModel.GameState;
		GameController.instance.GameModel.GameState = GameModel.Gamestate.TUT;
		DoozyUI.UIManager.ShowUiElement ("TutorialPanel");
	}

	public void OnGameOver() {
		if (GameController.instance.GameModel.score > GameController.instance.GameModel.bestScore) {
			GameController.instance.GameModel.bestScore = GameController.instance.GameModel.score;

			if (GameController.instance.GameModel.GameMode == GameModel.Gamemode.NORMAL) {
				PlayerPrefs.SetFloat ("BestNormal", GameController.instance.GameModel.score);
			} else if (GameController.instance.GameModel.GameMode == GameModel.Gamemode.CONFUSE) {
				PlayerPrefs.SetFloat ("BestConfuse", GameController.instance.GameModel.score);
			} else if (GameController.instance.GameModel.GameMode == GameModel.Gamemode.MADNESS) {
				PlayerPrefs.SetFloat ("BestMadness", GameController.instance.GameModel.score);
			} else {
				PlayerPrefs.SetFloat ("BestInsane", GameController.instance.GameModel.score);
			}
		}

		Input.multiTouchEnabled = false;
		Time.timeScale = 0;
		bestScoreText.text = string.Format("{0:0.0}", GameController.instance.GameModel.bestScore);
		DoozyUI.UIManager.ShowUiElement ("OverlayPanel");
		DoozyUI.UIManager.ShowUiElement ("GameOverPanel");
		GameController.instance.GameModel.GameState = GameModel.Gamestate.OVER;
	}

	public void OnGameEvent (string gameEvent) {
		switch (gameEvent) {
		case "NormalStart":
			DoozyUI.UIManager.ShowUiElement ("PlayPanel");
			GameController.instance.GameModel.GameMode = GameModel.Gamemode.NORMAL;
			GameController.instance.OnStart ();
			break;
		case "ConfuseStart":
			DoozyUI.UIManager.ShowUiElement ("PlayPanel");
			GameController.instance.GameModel.GameMode = GameModel.Gamemode.CONFUSE;
			GameController.instance.OnStart ();
			break;
		case "MadnessStart":
			DoozyUI.UIManager.ShowUiElement ("PlayPanel");
			GameController.instance.GameModel.GameMode = GameModel.Gamemode.MADNESS;
			GameController.instance.OnStart ();
			break;
		case "InsaneStart":
			DoozyUI.UIManager.ShowUiElement ("PlayPanel");
			GameController.instance.GameModel.GameMode = GameModel.Gamemode.INSANE;
			GameController.instance.OnStart ();
			break;
		case "ToStartPanel":
			GameController.instance.GameModel.GameState = GameModel.Gamestate.START;
			DoozyUI.UIManager.HideUiElement ("TitlePanel");
			break;
		case "ToSettings":
			stateB4 = GameController.instance.GameModel.GameState;
			GameController.instance.GameModel.GameState = GameModel.Gamestate.SETTINGS;
			DoozyUI.UIManager.ShowUiElement ("SettingsPanel");
			DoozyUI.UIManager.ShowUiElement ("OverlayPanel");
			break;
		case "ToTitle":
			DoozyUI.UIManager.ShowUiElement ("TitlePanel");
			GameController.instance.GameModel.GameState = GameModel.Gamestate.TITLE;
			break;
		case "Sound":
			if (PlayerPrefs.GetInt ("Sound", 1) == 1) {
				PlayerPrefs.SetInt ("Sound", 0);
			} else {
				PlayerPrefs.SetInt ("Sound", 1);
			}

			AudioController.instance.OnOffSound (PlayerPrefs.GetInt ("Sound"));
			break;
		case "Music":
			if (PlayerPrefs.GetInt ("Music", 1) == 1) {
				PlayerPrefs.SetInt ("Music", 0);
			} else {
				PlayerPrefs.SetInt ("Music", 1);
			}

			GameController.instance.OnOffMusic (PlayerPrefs.GetInt ("Music"));
			break;	
		case "Close":
			if (GameController.instance.GameModel.GameState == GameModel.Gamestate.TUT) {
				tutScRect.horizontalNormalizedPosition = 0;

				if (PlayerPrefs.HasKey ("FirstPlay")) {
					GameController.instance.OnStart ();
					GameController.instance.GameModel.GameState = GameModel.Gamestate.PLAY;
				}
			} else {
				GameController.instance.GameModel.GameState = stateB4;
			}

			DoozyUI.UIManager.HideUiElement ("SettingsPanel");
			DoozyUI.UIManager.HideUiElement ("TutorialPanel");
			DoozyUI.UIManager.HideUiElement ("OverlayPanel");
			break;
		case "ButtonClick":
			AudioController.instance.Play (AudioController.instance.audioClips [1]);
			break;
		}
	}

	public void ChangeBGColor(GameObject wholeBG, int groundNumbs, int colorIndex) {
		int i = 0;
		baseBG.GetComponent<SpriteRenderer>().sprite = baseBGs [colorIndex];
		foreach (Transform child in wholeBG.transform) {
			if (i < groundNumbs) {
				child.gameObject.GetComponent<Image> ().color = allColors [colorIndex, i++];
			}
		}
	}
}
