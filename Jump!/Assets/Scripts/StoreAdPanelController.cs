using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class StoreAdPanelController : MonoBehaviour {

	public GameObject m_rewardsPanel;
	public GameObject m_promptPanel;
	public GameObject m_adPanel;
	public GameObject m_exitButton;
	public GameObject m_watchButton;
	public Text m_description;
	public Text m_title;

	private void Update()
    {
        if (AdController.controller.CheckIfAdRewardsAvailable("Currency"))
		{
			m_watchButton.SetActive(true);
			m_description.text = "Watch an ad to earn in-game currency.";
			m_title.text = "Earn Rewards!";
		}
		else
        {
			m_watchButton.SetActive(false);
            int adTime = (int)(AdController.controller.GetTimeBetweenAds("Currency") - NewDataPersistence.data.ComputeTimeDifference(NewDataPersistence.data.timeSinceLastAdRewardTypeCurrency, System.DateTime.Now)) + 1;
			string units = "minutes";
			if (adTime == 1) 
			{
				units = "minute"; 
			}
            m_description.text = "You can watch another ad in " + adTime.ToString() + " " + units + " to earn in-game currency.";
			m_title.text = "Come Back Later!";
		}
    }

	public void ShowRewardedAd()
	{
		AudioOutput.manager.PlayButtonTapSound();
		if (Advertisement.IsReady("rewardedVideo"))
		{
			m_exitButton.SetActive(false);
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
				m_exitButton.SetActive(true);
				break;
			case ShowResult.Failed:
				Debug.LogError("The ad failed to be shown.");
				AdFailedPopupController.controller.ShowAdFailedPopup();
				m_exitButton.SetActive(true);
				break;
		}
	}

	private void ShowRewards() {
		m_rewardsPanel.SetActive(true);
		m_promptPanel.SetActive(false);
		m_exitButton.SetActive(true);
	}

	public void RewardPlayer(string type) {
		AudioOutput.manager.PlayButtonTapSound();
		AdController.controller.RewardPlayerWithCurrency(type);
        m_rewardsPanel.SetActive(false);
        m_promptPanel.SetActive(true);
	}
}
