using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundMoving : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (GameController.instance.GameModel.GameState == GameModel.Gamestate.PLAY) {
			if (gameObject.transform.localRotation.y == -1) {
				transform.Translate (Vector3.right * GameController.instance.GameModel.speed * Time.deltaTime); 
			} else {
				transform.Translate (Vector3.left * GameController.instance.GameModel.speed * Time.deltaTime); 
			}
		}
	}

}
