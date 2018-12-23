using UnityEngine;
using UnityEngine.UI;

public class TotalCurrency : MonoBehaviour {

    public Text m_coinText;
    public Text m_keyText;
    
    private void UpdateText()
    {
        object coins = (int)NewDataPersistence.data.GetValue("Coins", false);
        object keys = (int)NewDataPersistence.data.GetValue("Keys", false);
        m_coinText.text = coins.ToString();
        m_keyText.text = keys.ToString();
    }

	// Update is called once per frame
	void LateUpdate () {
        UpdateText();
	}
}
