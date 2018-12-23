using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupController : MonoBehaviour {

    private float m_timer = 0.0f;
    private float m_waitTime;
    public GameObject m_adPanel;

	// Use this for initialization
	void Start () {
        m_waitTime = Player.info.GetDeathTimerLength() + 2.0f;
    }
	
	// Update is called once per frame
	void Update () {
		if (Player.info.CheckIfPlayerDead())
        {
            if (m_timer < m_waitTime)
            {
                m_timer += Time.deltaTime;
            }
            if (m_timer >= m_waitTime)
            {
                m_adPanel.SetActive(true);
                gameObject.SetActive(false);
            }
        }
	}
}
