using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class GameAdController : MonoBehaviour {

	public GameObject m_rewardsPanel;
	public GameObject m_promptPanel;
	public GameObject m_gameOverPopup;

	void OnEnable()
	{
		if (AdController.controller.CheckIfAdRewardsAvailable("Extra Life"))
		{
			m_promptPanel.SetActive(true);
		}
		else
        {
			m_gameOverPopup.SetActive(true);
			gameObject.SetActive(false);
		}
	}

	public void ShowRewardedAd()
	{
		AudioOutput.manager.PlayButtonTapSound();
		if (Advertisement.IsReady("rewardedVideo"))
		{
			var options = new ShowOptions { resultCallback = HandleShowResult };
			Advertisement.Show("rewardedVideo", options);
		}
		else
		{
			AdFailedPopupController.controller.ShowAdFailedPopup();
		}
	}

	private void HandleShowResult(ShowResult result)
	{
		switch (result)
		{
			case ShowResult.Finished:
				Debug.Log("The ad was successfully shown.");
				ShowRewards();
				break;
			case ShowResult.Skipped:
				Debug.Log("The ad was skipped before reaching the end.");
				break;
			case ShowResult.Failed:
				Debug.LogError("The ad failed to be shown.");
				AdFailedPopupController.controller.ShowAdFailedPopup();
				break;
		}
	}

	private void ShowRewards() {
		m_rewardsPanel.SetActive(true);
		m_promptPanel.SetActive(false);
	}

	public void ClosePanel()
	{
		AudioOutput.manager.PlayButtonTapSound();
		m_rewardsPanel.SetActive(false);
		m_gameOverPopup.SetActive(true);
		gameObject.SetActive(false);
	}

	public void RewardPlayer() {
		AudioOutput.manager.PlayButtonTapSound();
		AdController.controller.RewardPlayerWithExtraLife();
	}
}
