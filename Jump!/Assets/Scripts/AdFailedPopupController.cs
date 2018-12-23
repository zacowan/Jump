using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdFailedPopupController : MonoBehaviour {

	public static AdFailedPopupController controller;

	public GameObject m_panel;

	void Awake()
    {
        if (controller == null)
        {
            controller = this;
        }
        else if (controller != this)
        {
            Destroy(gameObject);
        }
    }

	public void ShowAdFailedPopup () {
		AudioOutput.manager.PlayButtonTapSound();
		m_panel.SetActive(true);
		gameObject.GetComponent<Image>().enabled = true;
	}

	public void HideAdFailedPopup () {
		AudioOutput.manager.PlayButtonTapSound();
		m_panel.SetActive(false);
		gameObject.GetComponent<Image>().enabled = false;
	}
}
