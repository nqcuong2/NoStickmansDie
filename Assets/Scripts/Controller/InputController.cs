using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

	public GameObject ground;

	[HideInInspector]
	public bool touched;
	int pointerId;

	void Awake() 
	{
		touched = false;
	}

	public void OnPointerDown(PointerEventData data)
	{
		if (!touched) {
			touched = true;
			ground.GetComponent<GroundController> ().OnJump ();
			pointerId = data.pointerId;
		}
	}

	public void OnPointerUp(PointerEventData data) {
		if (data.pointerId == pointerId) {
			touched = false;
		}
	}
}
