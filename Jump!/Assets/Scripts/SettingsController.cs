using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour {

	public Text m_muteButton;
	public GameObject m_settingsPanel;
	public GameObject m_settingsButton;

	void OnEnable()
	{
		if (PlayerPrefs.GetFloat("Volume") == -1.0f) {
			m_muteButton.text = "Unmute";
		}
		else{
			m_muteButton.text = "Mute";
		}
	}
	
	public void ChangeVolume() {
		AudioOutput.manager.PlayButtonTapSound();
		if (m_muteButton.text == "Mute")
		{
			PlayerPrefs.SetFloat("Volume", -1.0f);
			m_muteButton.text = "Unmute";
		}
		else
		{
			PlayerPrefs.SetFloat("Volume", 0.0f);
			m_muteButton.text = "Mute";
		}
		PlayerPrefs.Save();
		AudioOutput.manager.ChangeVolume();
	}

	public void ClosePanel() {
		AudioOutput.manager.PlayButtonTapSound();
		m_settingsButton.SetActive(true);
		m_settingsPanel.SetActive(false);
	}

	public void OpenPanel() {
		AudioOutput.manager.PlayButtonTapSound();
		m_settingsButton.SetActive(false);
		m_settingsPanel.SetActive(true);
	}
}
