using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Analytics;
using System.Collections.Generic;

public class PurchaseButton : MonoBehaviour {

    private Text m_ownedText;
    private GameObject m_choices;
    private GameObject m_failedPopup;

    private int m_cost;
    private string m_name;
    private string m_currency;

	// Use this for initialization
	void Start () {
        for (int i = 0; i < transform.parent.childCount; i++)
        {
            Transform currentChild = transform.parent.GetChild(i);
            if (currentChild.transform.name == "Name")
            {
                m_name = currentChild.transform.GetComponent<Text>().text;
            }
            else if (currentChild.transform.name == "Cost")
            {
                m_cost = int.Parse(currentChild.GetChild(0).transform.GetComponent<Text>().text);
                if (currentChild.GetChild(1).gameObject.activeSelf)
                {
                    m_currency = "Coins";
                }
                else if (currentChild.GetChild(2).gameObject.activeSelf)
                {
                    m_currency = "Keys";
                }
            }
            else if (currentChild.transform.name == "Owned")
            {
                m_ownedText = currentChild.transform.GetComponent<Text>();
            }
            else if (currentChild.transform.name == "Choices")
            {
                m_choices = currentChild.gameObject;
            }
        }
        m_choices.SetActive(false);
        gameObject.SetActive(true);
        if ((bool)NewDataPersistence.data.GetValue(m_name, false))
        {
            OwnedItem();
        } else
        {
            m_ownedText.gameObject.SetActive(false);
            gameObject.SetActive(true);
        }
        m_failedPopup = gameObject.transform.parent.parent.parent.parent.parent.GetChild(4).gameObject;
	}
	
	public void PurchaseItem()
    {
        int totalCurrency = (int)NewDataPersistence.data.GetValue(m_currency, false);
        //Debug.Log("Trying to purchase an item for " + m_cost.ToString() + " " + m_currency + " with " + totalCurrency.ToString() + " " + m_currency);
        if (totalCurrency >= m_cost)
        {
            AudioOutput.manager.PlayButtonTapSound();
            NewDataPersistence.data.SetValue(m_name, true, false);
            NewDataPersistence.data.SetValue(m_currency, (int)NewDataPersistence.data.GetValue(m_currency, false) - m_cost, false);
            //Debug.Log(m_name + " purchased for " + m_cost.ToString() + " " + m_currency);
            Analytics.CustomEvent("itemPurchase", new Dictionary<string, object> 
            {
                {"item", m_name},
                {"cost", (m_cost.ToString() + " " + m_currency)},
                {"playerBank", (totalCurrency.ToString() + " " + m_currency)}
            });
            NewDataPersistence.data.Save();
            HideChoices();
            OwnedItem();
        }
        else {
            m_failedPopup.SetActive(true);
            HideChoices();
        }
    }

    public void ShowChoices()
    {
        AudioOutput.manager.PlayButtonTapSound();
        m_choices.SetActive(true);
        gameObject.SetActive(false);
    }

    public void HideChoices()
    {
        AudioOutput.manager.PlayButtonTapSound();
        m_choices.SetActive(false);
        gameObject.SetActive(true);
    }

    private void OwnedItem()
    {
        m_ownedText.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
