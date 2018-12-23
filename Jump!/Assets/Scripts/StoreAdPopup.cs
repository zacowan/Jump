using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class StoreAdPopup : MonoBehaviour {

	public GameObject m_rewardsPanel;
	public GameObject m_promptPanel;
	public GameObject m_exitButton;
    public GameObject m_watchButton;
    public Text m_description;

	void OnEnable()
	{
		m_promptPanel.SetActive(true);
		m_rewardsPanel.SetActive(false);
		//m_exitButton.SetActive(false);
	}

    private void Update()
    {
        if (AdController.controller.CheckIfAdRewardsAvailable("Currency"))
		{
			m_watchButton.SetActive(true);
			m_description.text = "Watch an ad to earn in-game currency.";
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
		}
    }

    public void ClosePanel() {
		AudioOutput.manager.PlayButtonTapSound();
		gameObject.SetActive(false);
		m_promptPanel.SetActive(true);
		m_rewardsPanel.SetActive(false);
		//m_exitButton.SetActive(true);
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

	public void RewardPlayer(string type) {
		AudioOutput.manager.PlayButtonTapSound();
		AdController.controller.RewardPlayerWithCurrency(type);
		ClosePanel();
	}
}
