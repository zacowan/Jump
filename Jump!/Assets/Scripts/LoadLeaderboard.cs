using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadLeaderboard : MonoBehaviour {
	public void LoadTheLeaderboard () {
		AudioOutput.manager.PlayButtonTapSound();
		SocialPlatformController.controller.DisplayLeaderboard();
	}
}
