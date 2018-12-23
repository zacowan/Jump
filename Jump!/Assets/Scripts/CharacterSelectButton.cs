using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectButton : MonoBehaviour {

    private Image m_img;
    private Image m_overlay;

    private void Start()
    {
        m_img = transform.parent.GetChild(1).GetComponent<Image>();
        m_overlay = transform.parent.GetChild(3).GetComponent<Image>();
        SetOverlay();
    }

    private void LateUpdate()
    {
        SetOutline();
    }

    public void SetCharacter()
    {
        if ((bool)NewDataPersistence.data.GetValue(gameObject.name, false))
        {
            AudioOutput.manager.PlayButtonTapSound();
            PlayerPrefs.SetString("Character", gameObject.name);
            PlayerPrefs.Save();
        }
    }

    private void SetOutline()
    {
        if (PlayerPrefs.GetString("Character") == gameObject.name)
        {
            m_img.enabled = false;
        }
        else
        {
            m_img.enabled = true;
        }
    }

    private void SetOverlay() {
        if (gameObject.name != "Null" && (bool)NewDataPersistence.data.GetValue(gameObject.name, false))
        {
            m_overlay.enabled = false;
        }
    }
}
