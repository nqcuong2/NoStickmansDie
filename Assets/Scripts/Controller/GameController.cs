using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

	public static GameController instance;

	public GameObject mainCam;
	public GameObject stickMan;
	public GameObject groundBlock;
	public Sprite[] groundBlocks;
	public GameObject[] modes;
	public GameObject[] inputs;
	public GameObject[] particles;
	public List<GameObject> grounds;
	public List<GameObject> trash;

	GameObject dividedScreen;
	GameObject inputController;
	int particleIndex;
	bool speedIncrease;
	float groundSize = 3.501f;

	public float GroundSize {
		get {
			return groundSize;
		}
	}

	AudioSource music;

	public AudioSource Music {
		get {
			return music;
		}
	}

	GameModel gameModel;

	public GameModel GameModel {
		get {
			return gameModel;
		}
		set {
			gameModel = value;
		}
	}

	static float[] scoreSystem = { 0.15f, 0.16f, 0.17f, 0.18f, 0.19f, 0.2f };

	void Awake ()
	{
		instance = this;
	}

	// Use this for initialization
	void Start ()
	{
		music = GetComponent<AudioSource> ();
		music.Play ();
		gameModel = new GameModel ();
		trash = new List<GameObject> ();
		Input.multiTouchEnabled = false;
		speedIncrease = false;
		OnOffMusic (PlayerPrefs.GetInt ("Music", 1));
	}
		
	public void OnOffMusic(int trigger) {
		if (trigger == 1) {
			music.volume = 0.25f;
		} else {
			music.volume = 0;
		}

		UIController.instance.soundNMusic [1].sprite = UIController.instance.toggleBtn [trigger];
	}

	public void IncreaseScore() {
		float waitTime = 0.2f;
		gameModel.score += scoreSystem [Random.Range (0, scoreSystem.Length)];
		UIController.instance.UpdateScore (gameModel.score);
		int roundedScore = (int)gameModel.score;

		if (roundedScore != 0 && roundedScore % 25 == 0 && !speedIncrease) {
			gameModel.speed += 1f;
			waitTime -= 0.03f;
			speedIncrease = true;
		} else if (roundedScore % 25 != 0) {
			speedIncrease = false;
		}

		Invoke ("IncreaseScore", waitTime);
	}

	public void OnStart ()
	{
		if (PlayerPrefs.GetInt ("FirstPlay", 0) == 0) {
			ScrollSnapRect.instance.DecelerateTime = Time.deltaTime;
			UIController.instance.onClickTutorial ();
			PlayerPrefs.SetInt ("FirstPlay", 1);
		} else {
			Time.timeScale = 1;
			gameModel.speed = 5f;
			int parIndex = Random.Range (0, particles.Length);
			while (parIndex == particleIndex) {
				parIndex = Random.Range (0, particles.Length);
			}
			particleIndex = parIndex;

			groundBlock.GetComponent<SpriteRenderer> ().sprite = groundBlocks [particleIndex];

			int groundNumbs;
			UIController.instance.baseBG.gameObject.SetActive (true);

			if (gameModel.GameMode == GameModel.Gamemode.NORMAL) {
				dividedScreen = Instantiate (modes [0], mainCam.transform);
				inputController = inputs [0];
				groundNumbs = 2;
				gameModel.bestScore = PlayerPrefs.GetFloat ("BestNormal", 0f);
			} else if (gameModel.GameMode == GameModel.Gamemode.CONFUSE) {
				dividedScreen = Instantiate (modes [1], mainCam.transform);
				inputController = inputs [1];
				groundNumbs = 3;
				gameModel.bestScore = PlayerPrefs.GetFloat ("BestConfuse", 0f);
			} else if (gameModel.GameMode == GameModel.Gamemode.MADNESS) {
				dividedScreen = Instantiate (modes [2], mainCam.transform);
				inputController = inputs [2];
				groundNumbs = 4;
				gameModel.bestScore = PlayerPrefs.GetFloat ("BestMadness", 0f);
			} else {
				dividedScreen = Instantiate (modes [3], mainCam.transform);
				inputController = inputs [3];
				groundNumbs = 5;
				gameModel.bestScore = PlayerPrefs.GetFloat ("BestInsane", 0f);
			}

			inputController.SetActive (true);
			UIController.instance.ChangeBGColor (inputController, groundNumbs, parIndex);
			GameObject particle = Instantiate (particles [parIndex]);
			trash.Add (particle);

			foreach (Transform child in dividedScreen.transform) {
				grounds.Add (child.gameObject);
				BoxCollider2D childColl = child.gameObject.GetComponent<BoxCollider2D> ();

				GameObject ground = Instantiate (groundBlock, new Vector3 (groundBlock.transform.localPosition.x, childColl.offset.y - childColl.size.y / 2, groundBlock.transform.position.z), Quaternion.identity);
				trash.Add (ground);

				for (int j = 0; j < 2; j++) {
					ground = Instantiate (groundBlock, new Vector3 (ground.transform.localPosition.x + groundSize - 0.2f, childColl.offset.y - childColl.size.y / 2, groundBlock.transform.position.z), Quaternion.identity);

					if (j % 2 == 0) {
						ground.transform.Rotate (new Vector3 (0, 180, 0));
					}
					
					trash.Add (ground);
				}

				child.gameObject.GetComponent<GroundController> ().currGround = ground;
				GameObject newSM = Instantiate (stickMan, new Vector3 (stickMan.transform.position.x, childColl.offset.y - childColl.size.y / 2 + 1f,
					                  stickMan.transform.position.z), Quaternion.identity);
				newSM.GetComponent<PlayerController> ().PlayerModel.CurrGround = child.gameObject;

				if (child.gameObject.tag != "First") {
					newSM.GetComponent<PlayerController> ().PlayerModel.NextGround = grounds [grounds.IndexOf (child.gameObject) - 1];
				} else {
					newSM.GetComponent<PlayerController> ().PlayerModel.NextGround = null;
				}

				child.gameObject.GetComponent<GroundController> ().lastRotate = (int)ground.transform.localRotation.y;
				child.gameObject.GetComponent<GroundController> ().stickMans = new List<GameObject> ();
				child.gameObject.GetComponent<GroundController> ().stickMans.Add (newSM);

				trash.Add (newSM);
			}

			int i = 0;
			foreach (Transform child in inputController.transform) {
				child.gameObject.GetComponent<InputController> ().ground = grounds [i++];
				child.gameObject.GetComponent<InputController> ().touched = false;
			}

			Input.multiTouchEnabled = true;
			GameController.instance.GameModel.GameState = GameModel.Gamestate.PLAY;
			IncreaseScore ();
		}
	}

	public void OnRestart() {
		OnToMenu ();
		OnStart ();
	}

	public void OnToMenu() {
		music.Play ();
		foreach (GameObject child in trash) {
			Destroy (child);
		}
		trash.Clear ();

		foreach (GameObject ground in grounds) {
			Destroy (ground);
		}
		grounds.Clear ();

		Destroy (dividedScreen);
		UIController.instance.baseBG.gameObject.SetActive (false);
		inputController.SetActive (false);
		CancelInvoke ();
		gameModel.score = 0;
		transform.position = Vector3.zero;
		mainCam.transform.position = new Vector3 (0, 0, -10f);
		Time.timeScale = 1;
	}

	void OnTriggerExit2D(Collider2D other) {
		Destroy (other.gameObject);
	}

}
