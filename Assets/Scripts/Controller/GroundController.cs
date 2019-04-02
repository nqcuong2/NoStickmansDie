using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class GroundController : MonoBehaviour
{

	public List<GameObject> stickMans;
	[HideInInspector]
	public GameObject currGround;
	[HideInInspector]
	public int lastRotate;
//	public InputController input;

	Vector3 originPos;
	float offSet = 2.8f;
	float fallingDistance;
	bool beforeFalling;
	BoxCollider2D groundColl;
	float smanSpace;

	// Use this for initialization
	void Start ()
	{
		beforeFalling = false;
		groundColl = GetComponent<BoxCollider2D> ();
		smanSpace = 0.16f;

	}
	
	// Update is called once per frame
	void Update ()
	{
		if (GameController.instance.GameModel.GameState == GameModel.Gamestate.PLAY) {
			SpawnGround ();

			if (stickMans.Count > 0) {
				Move ();
				originPos = stickMans [0].transform.position;
			}

			if (stickMans.Count == 0) {
				SpawnSM ();
			}

		}
	}

	void Move ()
	{
		stickMans [0].transform.position = new Vector3 (GameController.instance.mainCam.transform.position.x - 1.25f, stickMans [0].transform.position.y);
		for (int i = 1; i < stickMans.Count; i++) {
			if (!stickMans [i].GetComponent<PlayerController> ().PlayerModel.Grounded && stickMans [i].gameObject.GetComponent<PlayerController> ().PlayerModel.Fall) {
//				stickMans [i].GetComponent<BoxCollider2D> ().isTrigger = true;
				stickMans [i].GetComponent<Rigidbody2D> ().velocity = new Vector2 (stickMans [i].GetComponent<Rigidbody2D> ().velocity.x - fallingDistance, stickMans [i].GetComponent<Rigidbody2D> ().velocity.y);
				stickMans [i].GetComponent<PlayerController> ().PlayerModel.Fall = false;
				stickMans [i].GetComponent<PlayerController> ().PlayerModel.AfterFall = true;
			}

			if (stickMans [i].GetComponent<PlayerController> ().PlayerModel.Grounded && stickMans [i].GetComponent<PlayerController> ().PlayerModel.AfterFall) {
				stickMans [i].GetComponent<PlayerController> ().PlayerModel.AfterFall = false;
			}

			if (!stickMans [i].GetComponent<PlayerController> ().PlayerModel.AfterFall) {
				stickMans [i].transform.position = new Vector3 (stickMans [i - 1].transform.position.x - smanSpace, stickMans [i].transform.position.y);
			}
		}
	}

	void SpawnSM ()
	{
		float smOffSet = 1f;
		GameObject newSM = Instantiate (GameController.instance.stickMan, new Vector3 (originPos.x, originPos.y + smOffSet, originPos.z), Quaternion.identity);
		AudioController.instance.Play (AudioController.instance.audioClips [0]);
		newSM.GetComponent<PlayerController> ().PlayerModel.CurrGround = gameObject;

		if (gameObject.tag != "First") { 
			newSM.GetComponent<PlayerController> ().PlayerModel.NextGround = GameController.instance.grounds [GameController.instance.grounds.IndexOf (gameObject) - 1];
		}

		stickMans.Add (newSM);
		GameController.instance.trash.Add (newSM);
		newSM.GetComponent<Rigidbody2D> ().velocity = new Vector2 (newSM.GetComponent<Rigidbody2D> ().velocity.x, newSM.GetComponent<Rigidbody2D> ().velocity.y + 5.5f);
	}

	void SpawnGround ()
	{
		if ((currGround.transform.position.x - GameController.instance.GroundSize / 2) < GameController.instance.mainCam.transform.position.x + offSet) {
			float offSet = 0.2f;
			int grounds = Random.Range (1, 5);
			int space;
			float distance = 0f;
			space = Random.Range (0, 2);

			if (space == 1) {
				distance = Random.Range (1.2f, 1.35f);
			}

			GameObject ground = null;
			if (lastRotate == 0) {
				for (int i = 0; i < grounds; i++) {
					if (i % 2 == 0) {
						if (i == 0) {
							ground = Instantiate (GameController.instance.groundBlock, new Vector3 (currGround.transform.position.x + GameController.instance.GroundSize - offSet + distance, currGround.transform.position.y, currGround.transform.position.z), new Quaternion(0, -180, 0, 0));
						} else {
							ground = Instantiate (GameController.instance.groundBlock, new Vector3 (ground.transform.position.x + GameController.instance.GroundSize - offSet + distance, ground.transform.position.y, ground.transform.position.z), new Quaternion(0, -180, 0, 0));
						}
					} else {
						ground = Instantiate (GameController.instance.groundBlock, new Vector3 (ground.transform.position.x + GameController.instance.GroundSize - offSet + distance, ground.transform.position.y, ground.transform.position.z), Quaternion.identity);
					}
					GameController.instance.trash.Add (ground);
				}
			} else {
				for (int i = 0; i < grounds; i++) {
					if (i % 2 == 1) {
						ground = Instantiate (GameController.instance.groundBlock, new Vector3 (ground.transform.position.x + GameController.instance.GroundSize - offSet + distance, ground.transform.position.y, ground.transform.position.z), new Quaternion (0, -180, 0, 0));
					} else {
						if (i == 0) {
							ground = Instantiate (GameController.instance.groundBlock, new Vector3 (currGround.transform.position.x + GameController.instance.GroundSize - offSet + distance, currGround.transform.position.y, currGround.transform.position.z), Quaternion.identity);
						} else {
							ground = Instantiate (GameController.instance.groundBlock, new Vector3 (ground.transform.position.x + GameController.instance.GroundSize - offSet + distance, ground.transform.position.y, ground.transform.position.z), Quaternion.identity);
						}
					}
					GameController.instance.trash.Add (ground);
				}
			}

			lastRotate = (int)ground.transform.localRotation.y;
			currGround = ground;
		}
	}

	public void OnJump ()
	{
		for (int i = 0; i < stickMans.Count; i++) {
			stickMans [i].GetComponent<PlayerController> ().Jump ();
		}
	}

	void OnTriggerExit2D (Collider2D other)
	{
		if (GameController.instance.GameModel.GameState == GameModel.Gamestate.PLAY && other.gameObject.tag == "Player") {
			if (gameObject.tag != "First" && other.gameObject.GetComponent<PlayerController> ().PlayerModel.CurrGround == gameObject) {
				if (!beforeFalling) {
					float waitTime;
					beforeFalling = true;
					int belowSMCount = other.gameObject.GetComponent<PlayerController> ().PlayerModel.NextGround.GetComponent<GroundController> ().stickMans.Count;

					if (GameController.instance.GameModel.GameMode == GameModel.Gamemode.NORMAL) {
						if (belowSMCount < 4) {
							other.gameObject.GetComponent<PlayerController> ().PlayerModel.NextGround.GetComponent<GroundController> ().fallingDistance = 1.2f;
						} else if (belowSMCount < 7) {
							other.gameObject.GetComponent<PlayerController> ().PlayerModel.NextGround.GetComponent<GroundController> ().fallingDistance = 2.5f;
						} else {
							other.gameObject.GetComponent<PlayerController> ().PlayerModel.NextGround.GetComponent<GroundController> ().fallingDistance = 4f;
						}

						waitTime = 2f;

					} else if (GameController.instance.GameModel.GameMode == GameModel.Gamemode.CONFUSE) {
						if (belowSMCount < 4) {
							other.gameObject.GetComponent<PlayerController> ().PlayerModel.NextGround.GetComponent<GroundController> ().fallingDistance = 2f;
						} else if (belowSMCount < 7) {
							other.gameObject.GetComponent<PlayerController> ().PlayerModel.NextGround.GetComponent<GroundController> ().fallingDistance = 4.25f;
						} else {
							other.gameObject.GetComponent<PlayerController> ().PlayerModel.NextGround.GetComponent<GroundController> ().fallingDistance = 6f;
						}

						waitTime = 1.5f;

					} else if (GameController.instance.GameModel.GameMode == GameModel.Gamemode.MADNESS) {
						if (belowSMCount < 4) {
							other.gameObject.GetComponent<PlayerController> ().PlayerModel.NextGround.GetComponent<GroundController> ().fallingDistance = 2.25f;
						} else if (belowSMCount < 7) {
							other.gameObject.GetComponent<PlayerController> ().PlayerModel.NextGround.GetComponent<GroundController> ().fallingDistance = 4.75f;
						} else {
							other.gameObject.GetComponent<PlayerController> ().PlayerModel.NextGround.GetComponent<GroundController> ().fallingDistance = 7.5f;
						}

						waitTime = 1f;

					} else {
						if (belowSMCount < 4) {
							other.gameObject.GetComponent<PlayerController> ().PlayerModel.NextGround.GetComponent<GroundController> ().fallingDistance = 2.75f;
						} else if (belowSMCount < 7) {
							other.gameObject.GetComponent<PlayerController> ().PlayerModel.NextGround.GetComponent<GroundController> ().fallingDistance = 6.75f;
						} else {
							other.gameObject.GetComponent<PlayerController> ().PlayerModel.NextGround.GetComponent<GroundController> ().fallingDistance = 9f;
						}

						waitTime = 0.5f;
					}

					Invoke ("SetBeforeFalling", waitTime);
				}

				stickMans.Remove (other.gameObject);
				other.gameObject.GetComponent<PlayerController> ().PlayerModel.NextGround.GetComponent<GroundController> ().stickMans.Add (other.gameObject);
				other.gameObject.GetComponent<PlayerController> ().PlayerModel.CurrGround = GameController.instance.grounds [GameController.instance.grounds.IndexOf (gameObject) - 1];

				if (GameController.instance.grounds.IndexOf (gameObject) - 2 >= 0) {
					other.gameObject.GetComponent<PlayerController> ().PlayerModel.NextGround = GameController.instance.grounds [GameController.instance.grounds.IndexOf (gameObject) - 2];		
				}

				other.gameObject.GetComponent<PlayerController> ().PlayerModel.Fall = true;
			} 

			if (gameObject.tag == "First" && other.gameObject.transform.position.y <= (groundColl.offset.y - groundColl.size.y / 2)) {
				AudioController.instance.Play (AudioController.instance.audioClips [2]);
				GameController.instance.Music.Stop ();
				UIController.instance.OnGameOver ();
			}
		}
	}

	void SetBeforeFalling ()
	{
		beforeFalling = false;
	}

}
