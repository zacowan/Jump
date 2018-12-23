using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsPanelController : MonoBehaviour {

	public Text m_highScoreText;
	// Use this for initialization
	void Start () {
		object highScore = (int)(float)NewDataPersistence.data.GetValue("High Score", false);
		m_highScoreText.text = highScore.ToString();
	}
}
