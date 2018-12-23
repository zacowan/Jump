using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    private const float MAX_TIME_SCALE = 2.0f;
    private const float TIME_SCALE_STEP = 0.00005f;
    private const float DEFAULT_TIME_SCALE = 1.0f;

    private float m_scrollSpeed = 3f;
    private bool m_gamePaused = false;
    private float m_timeScale = DEFAULT_TIME_SCALE;

    public Text m_scoreText;
    public Text m_endScoreText;
    public Text m_highScoreText;
    public Text m_roundCoinText;
    public Text m_roundKeyText;
    public GameObject m_pauseButton;
    public GameObject m_pausePanel;
    public Text m_immuneTimer;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start()
    {
        Instantiate(Resources.Load(PlayerPrefs.GetString("Character"), typeof(GameObject)));
        if (AdController.controller.m_rewardedExtraLife)
        {
            NewDataPersistence.data.ModifiedResetGame();
            m_timeScale = AdController.controller.GetEndGameTimeScale();
            Time.timeScale = m_timeScale;
            Debug.Log("New time scale: " + Time.timeScale.ToString());
            AdController.controller.m_rewardedExtraLife = false;
        }
        else
        {
            NewDataPersistence.data.ResetGame();
        }
        AudioOutput.manager.PlayClip("game music", 1, 0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!Player.info.CheckIfPlayerDead() && !CheckIfGamePaused())
        {
            object score = (float)NewDataPersistence.data.GetValue("High Score", true);
            float scoreStep = Mathf.Log10(m_timeScale*Mathf.PI*2);
            NewDataPersistence.data.SetValue("High Score", (float)score + scoreStep, true);
            object roundedScore = (int)(float)NewDataPersistence.data.GetValue("High Score", true);
            m_scoreText.text = roundedScore.ToString();
            if (m_timeScale < MAX_TIME_SCALE)
            {
                m_timeScale += TIME_SCALE_STEP;
                Time.timeScale = m_timeScale;
            }
        }
    }

    public float GetScrollSpeed()
    {
        return m_scrollSpeed;
    }

    public void PlayerDead()
    {
        AudioOutput.manager.StopMusic();
        AudioOutput.manager.PlayClip("death sound", 1, 1);
        object roundScore = (int)(float)NewDataPersistence.data.GetValue("High Score", true);
        m_endScoreText.text = roundScore.ToString();
        NewDataPersistence.data.PlayerDead();
        object highScore = (int)(float)NewDataPersistence.data.GetValue("High Score", false);
        m_highScoreText.text = highScore.ToString();
        AdController.controller.SetEndGameTimeScale(m_timeScale);
        Time.timeScale = DEFAULT_TIME_SCALE;
    }

    public void DisplayRoundItems(int coins, int keys)
    {
        m_roundCoinText.text = coins.ToString();
        m_roundKeyText.text = keys.ToString();
    }

    public bool CheckIfGamePaused()
    {
        return m_gamePaused;
    }

    public void ChangeGamePauseState(bool value)
    {
        if (!Player.info.CheckIfPlayerDead())
        {
            AudioOutput.manager.PlayButtonTapSound();
            if (value)
            {
                m_gamePaused = true;
                AudioOutput.manager.StopMusic();
                Time.timeScale = 0;
                m_pauseButton.SetActive(false);
                m_pausePanel.SetActive(true);
            }
            else if (!value)
            {
                AudioOutput.manager.PlayClip("game music", 1, 0);
                m_gamePaused = false;
                Time.timeScale = m_timeScale;
                m_pauseButton.SetActive(true);
                m_pausePanel.SetActive(false);
            }
        }
    }

    public void DisplayTimer(float timeRemaining) {
        if (timeRemaining > 0)
        {
            m_immuneTimer.text = ((int)timeRemaining).ToString();
        }
        else
        {
            m_immuneTimer.text = "";
        }
    }

    public float GetTimeScale() {
        return m_timeScale;
    }

    public void SetTimeScale(float newValue) {
        m_timeScale = newValue;
    }
}
