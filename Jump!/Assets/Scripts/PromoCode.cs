using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PromoCode : MonoBehaviour {

	public Text m_codeMessage;
	public InputField m_inputField;
	private Dictionary<string, string> m_codeDict = new Dictionary<string, string>(); //Format: code, type
	private const string DEFAULT_MESSAGE_TEXT = "Enter a promotional code above. Do not use spaces, symbols, or capital letters.";
	private const string VALID_MESSAGE_TEXT = "Code is valid.";
	private const string INVALID_MESSAGE_TEXT = "Code is invalid. Check for unnecessary spaces, symbols, or capital letters.";
	//For a currency reward
	private const string REWARD_TYPE_CURRENCY = "a sack of coins and keys";
	private const float REWARD_PERCENTAGE = .05f;
	private const int COINS_REWARD_VALUE = 1250;
	private const int MAX_COINS_REWARD_VALUE = COINS_REWARD_VALUE*5;
	private const int KEYS_REWARD_VALUE = 25;
	private const int MAX_KEYS_REWARD_VALUE = KEYS_REWARD_VALUE*5;

	// Use this for initialization
	void Start () {
		m_codeDict.Add("GOOSE82999", "owner rewards");
		m_codeDict.Add("w9cyq7a2za", REWARD_TYPE_CURRENCY);
		m_codeDict.Add("erpvjt9y3h", REWARD_TYPE_CURRENCY);
		m_codeDict.Add("fb5pkak78k", REWARD_TYPE_CURRENCY);
		m_codeDict.Add("r7n5k4gf6t", REWARD_TYPE_CURRENCY);
		m_codeDict.Add("ypywe967er", REWARD_TYPE_CURRENCY);
		m_codeMessage.text = DEFAULT_MESSAGE_TEXT;
		m_inputField.text = "";
	}
	
	public void ValidateCode(){
		if (m_inputField.text != "")
		{
			bool codeValidated = false;
			string text = m_inputField.text;
			foreach (KeyValuePair<string, string> dictionary in m_codeDict) {
				if(text == dictionary.Key && PlayerPrefs.GetString(dictionary.Key) != "used") {
					codeValidated = true;
					break; 
				}
			}
			UpdateCodeMessage(codeValidated);
		}
	}

	private void UpdateCodeMessage(bool validated){
		if (validated)
		{
			m_codeMessage.text = VALID_MESSAGE_TEXT;
		}
		else {
			m_codeMessage.text = INVALID_MESSAGE_TEXT;
		}
	}

	public void UseCode() {
		string text = m_inputField.text;
		foreach (KeyValuePair<string, string> dictionary in m_codeDict) {
			if(text == dictionary.Key && PlayerPrefs.GetString(dictionary.Key) != "used") {
				RewardPlayer(dictionary.Key);
				break; 
			}
		}
	}

	private void RewardPlayer(string code) {
		AudioOutput.manager.PlayButtonTapSound();
		string reward = "";
		bool unlimitedUses = false;
		foreach(KeyValuePair<string, string> dictionary in m_codeDict) {
			if (dictionary.Key == code)
			{
				reward = dictionary.Value;
				if (reward == "owner rewards")
				{
					NewDataPersistence.data.SetValue("Keys", (int)NewDataPersistence.data.GetValue("Keys", false) + 500, false);
					NewDataPersistence.data.SetValue("Coins", (int)NewDataPersistence.data.GetValue("Coins", false) + 50000, false);
					unlimitedUses = true;
				}
				else if (reward == REWARD_TYPE_CURRENCY)
				{
					int coinReward = COINS_REWARD_VALUE + (int)((int)NewDataPersistence.data.GetValue("Coins", false)*REWARD_PERCENTAGE);
					int keyReward = KEYS_REWARD_VALUE + (int)((int)NewDataPersistence.data.GetValue("Keys", false)*REWARD_PERCENTAGE); 
					if (coinReward > MAX_COINS_REWARD_VALUE)
					{
						coinReward = MAX_COINS_REWARD_VALUE;
					}
					if (keyReward > MAX_KEYS_REWARD_VALUE)
					{
						keyReward = MAX_KEYS_REWARD_VALUE;
					}
					NewDataPersistence.data.SetValue("Keys", (int)NewDataPersistence.data.GetValue("Keys", false) + keyReward, false);
					NewDataPersistence.data.SetValue("Coins", (int)NewDataPersistence.data.GetValue("Coins", false) + coinReward, false);
					Debug.Log("Coins: " + coinReward.ToString());
					Debug.Log("Keys: " + keyReward.ToString());
					unlimitedUses = true;
				}
				if (!unlimitedUses)
				{
					PlayerPrefs.SetString(code, "used");
					PlayerPrefs.Save();
				}
				m_codeMessage.text = "You have received " + reward + " for the code {" + code + "}.";
				m_inputField.text = "";
				NewDataPersistence.data.Save();
				break;
			}
		}
	}
}
