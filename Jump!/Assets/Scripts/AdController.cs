using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;

public class AdController : MonoBehaviour {

	private const int KEYS_REWARD_VALUE = 5;
	private const int MAX_KEYS_REWARD_VALUE = KEYS_REWARD_VALUE*5;
	private const int COINS_REWARD_VALUE = 250;
	private const int MAX_COINS_REWARD_VALUE = COINS_REWARD_VALUE*5;
	private const float REWARD_PERCENTAGE = .05f;
    private const double TIME_BETWEEN_AD_TYPE_CURRENCY = 20; //in minutes
	private const double TIME_BETWEEN_AD_TYPE_EXTRA_LIFE = 10; //in minutes
	private float m_endTimeScale;
	public static AdController controller;
	public bool m_rewardedExtraLife = false;
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

	public void RewardPlayerWithCurrency(string type) {
		int rewardValue = 0;
		if (type == "Coins")
		{
			rewardValue = COINS_REWARD_VALUE;
		}
		else if (type == "Keys")
		{
			rewardValue = KEYS_REWARD_VALUE;
		}
		rewardValue = rewardValue + ((int)((int)NewDataPersistence.data.GetValue(type, false)*REWARD_PERCENTAGE));
		rewardValue = RewardValueCheck(rewardValue, type);
		Analytics.CustomEvent("adRewardTypeCurrency", new Dictionary<string, object> 
		{
			{"reward", (rewardValue.ToString() + " " + type.ToString())},
			{"playerBank", (NewDataPersistence.data.GetValue(type, false).ToString() + " " + type)}
		});
		NewDataPersistence.data.SetValue(type, (int)NewDataPersistence.data.GetValue(type, false) + rewardValue, false);
		NewDataPersistence.data.Save();
		NewDataPersistence.data.timeSinceLastAdRewardTypeCurrency = System.DateTime.Now;
		Debug.Log("Rewarded " + rewardValue.ToString() + " " + type);
	}

	private int RewardValueCheck(int reward, string type) {
		if (type == "Coins")
		{
			if (reward > MAX_COINS_REWARD_VALUE)
			{
				reward = MAX_COINS_REWARD_VALUE;
			}
		}
		else if (type == "Keys")
		{
			if (reward > MAX_KEYS_REWARD_VALUE)
			{
				reward = MAX_KEYS_REWARD_VALUE;
			}
		}
		return reward;
	}

	public void RewardPlayerWithExtraLife() {
		m_rewardedExtraLife = true;
		SceneManager.LoadScene("Gameplay");
		NewDataPersistence.data.timeSinceLastAdRewardTypeExtraLife = System.DateTime.Now;
		Analytics.CustomEvent("adRewardTypeExtraLife", new Dictionary<string, object> 
		{
			{"roundScore", NewDataPersistence.data.GetValue("High Score", true)},
			{"timeScale", m_endTimeScale}
		});
	}

	public void SetEndGameTimeScale(float endTimeScale) {
		m_endTimeScale = endTimeScale;
		Debug.Log("End game time scale: " + m_endTimeScale.ToString());
	}

	public float GetEndGameTimeScale() {
		return m_endTimeScale;
	}

	public bool CheckIfAdRewardsAvailable(string type) {
		if (type == "Currency")
		{
			if (NewDataPersistence.data.ComputeTimeDifference(NewDataPersistence.data.timeSinceLastAdRewardTypeCurrency, System.DateTime.Now) > TIME_BETWEEN_AD_TYPE_CURRENCY)
			{
				return true;
			} 
			else
			{
				return false;
			}
		}
		else 
		{
			if (NewDataPersistence.data.ComputeTimeDifference(NewDataPersistence.data.timeSinceLastAdRewardTypeExtraLife, System.DateTime.Now) > TIME_BETWEEN_AD_TYPE_EXTRA_LIFE)
			{
				return true;
			} 
			else	
			{
				return false;
			}
		}
	}

	public double GetTimeBetweenAds(string type) {
		if (type == "Currency")
		{
			return TIME_BETWEEN_AD_TYPE_CURRENCY;
		}
		else
		{
			return TIME_BETWEEN_AD_TYPE_EXTRA_LIFE;
		}
	}
	
}
