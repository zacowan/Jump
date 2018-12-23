using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.GameCenter;

public class SocialPlatformController : MonoBehaviour {

	public static SocialPlatformController controller;
	private static string LEADERBOARD_ID = "Jump.highScoreLeaderboard";
	public bool m_loginSuccessful;

	void Awake()
    {
        if (controller == null)
        {
            DontDestroyOnLoad(gameObject);
            controller = this;
        }
        else if (controller != this)
        {
            Destroy(gameObject);
        }
    }

	// Use this for initialization
	void Start () {
		Social.localUser.Authenticate(ProcessAuthentication);
	}

	public void ProcessAuthentication (bool success) {
		if (success) {
			Debug.Log("Authenticated user.");
		}
		else
		{
			Debug.LogError("Failed to authenticate user.");
		}
		m_loginSuccessful = success;
	}

	public void DisplayLeaderboard() {
		//Social.ShowLeaderboardUI();
		SetHighScore(NewDataPersistence.data.GetValue("High Score", false));
		if (m_loginSuccessful)
		{
			GameCenterPlatform.ShowLeaderboardUI(LEADERBOARD_ID, TimeScope.AllTime);
		}
		else
		{
			Social.localUser.Authenticate(ProcessAuthentication);
			if (m_loginSuccessful)
			{
				GameCenterPlatform.ShowLeaderboardUI(LEADERBOARD_ID, TimeScope.AllTime);
			}
		}
	}

	public void SetHighScore(object score) {
		long highScore = (long)(int)(float)score;
		Debug.Log("Trying to report a high score of " + highScore.ToString() + " for a leaderboard with the ID [" + LEADERBOARD_ID + "]");
		if (m_loginSuccessful)
		{
			Social.ReportScore(highScore, LEADERBOARD_ID, CheckIfScoreWasReported);
		}
		else 
		{
            Social.localUser.Authenticate(success => {
                if (success) {
                    Social.ReportScore(highScore, LEADERBOARD_ID, CheckIfScoreWasReported);
                }
                else{
                    Debug.LogError("Failed authentication, cannot report score.");
                }
            });
		}
	}

	private static void CheckIfScoreWasReported(bool wasReported) {
		if (wasReported) {
			Debug.Log("Successfully reported high score.");
		}
		else
		{
			Debug.LogError("Failed to report high score.");
		}
	}
}
