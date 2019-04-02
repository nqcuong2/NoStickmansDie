using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel {

	bool grounded;

	public bool Grounded {
		get {
			return grounded;
		}
		set {
			grounded = value;
		}
	}

	bool fall;

	public bool Fall {
		get {
			return fall;
		}
		set {
			fall = value;
		}
	}

	bool afterFall;

	public bool AfterFall {
		get {
			return afterFall;
		}
		set {
			afterFall = value;
		}
	}

	GameObject nextGround;

	public GameObject NextGround {
		get {
			return nextGround;
		}
		set {
			nextGround = value;
		}
	}

	GameObject currGround;

	public GameObject CurrGround {
		get {
			return currGround;
		}
		set {
			currGround = value;
		}
	}

	//Constructor
	public PlayerModel() {
		fall = false;
		nextGround = null;
	}
}
