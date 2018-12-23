using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwitterButton : MonoBehaviour {

	private const string TWITTER_URL = "https://twitter.com/zacowan_dev";

	public void OpenTwitter() {
		AudioOutput.manager.PlayButtonTapSound();
		Application.OpenURL(TWITTER_URL);
	}
	
}
