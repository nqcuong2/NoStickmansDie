using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour {

	PlayerModel playerModel;

	public PlayerModel PlayerModel {
		get {
			return playerModel;
		}
	}

	public LayerMask isGround;
	public Transform groundCheck;
	public float groundCheckRadius;

	Animator playerAnim;
	Rigidbody2D playerRb2d;
	BoxCollider2D playerCollider2d;

	void Awake() {
		playerModel = new PlayerModel ();
	}

	// Use this for initialization
	void Start () {
		playerAnim = GetComponent<Animator> ();
		playerRb2d = GetComponent<Rigidbody2D> ();
		playerCollider2d = GetComponent<BoxCollider2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (GameController.instance.GameModel.GameState == GameModel.Gamestate.PLAY) {
			playerModel.Grounded = Physics2D.OverlapCircle (groundCheck.position, groundCheckRadius, isGround);

			if (playerModel.Grounded) {
				playerCollider2d.isTrigger = false;
				playerAnim.Play ("Running");
			} else {
				playerCollider2d.isTrigger = true;
				playerAnim.Play ("Jump");
			}

		}
	}

	public void Jump() {
		if (playerModel.Grounded) {
			if (playerModel.CurrGround.GetComponent<GroundController> ().stickMans [0] == gameObject) {
				AudioController.instance.Play (AudioController.instance.audioClips [0]);
			}

			playerRb2d.velocity = new Vector2 (playerRb2d.velocity.x, playerRb2d.velocity.y + 9f);
		}
	}
		
}
