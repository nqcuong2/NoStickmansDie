using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModel {

	public float score;
	public float speed;
	public float bestScore;

	Gamestate gameState;

	public Gamestate GameState {
		get {
			return gameState;
		}
		set {
			gameState = value;
		}
	}

	Gamemode gameMode;

	public Gamemode GameMode {
		get {
			return gameMode;
		}
		set {
			gameMode = value;
		}
	}

	public enum Gamestate { TITLE, START, PLAY, PAUSE, TUT, SETTINGS, OVER, QUIT };
	public enum Gamemode { NORMAL, CONFUSE, MADNESS, INSANE };

	//Constructor
	public GameModel () {
		score = 0;
		speed = 1f;
		gameState = Gamestate.TITLE;
	}
		

}
