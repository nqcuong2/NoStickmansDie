using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour {

	public static AudioController instance;

	public AudioClip[] audioClips;

	private AudioSource adSource;

	void Awake() {
		instance = this;
	}

	// Use this for initialization
	void Start () {
		adSource = GetComponent<AudioSource> ();
		OnOffSound (PlayerPrefs.GetInt ("Sound", 1));
	}
	
	// Update is called once per frame
	public void Play (AudioClip clip) {
		adSource.PlayOneShot (clip);
	}

	public void OnOffSound(int trigger) {
		if (trigger == 1) {
			adSource.volume = 1;
		} else {
			adSource.volume = 0;
		}

		UIController.instance.soundNMusic [0].sprite = UIController.instance.toggleBtn [trigger];
	}
}
