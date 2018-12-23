using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioOutput : MonoBehaviour
{

    public static AudioOutput manager;
    public AudioClip m_gameMusic;
    public AudioClip m_menuMusic;
    public AudioClip m_jumpSound;
    public AudioClip m_deathSound;
    public AudioClip m_tapSound;
    public AudioClip m_itemSound;

    private AudioSource m_musicSource;
    private AudioSource m_soundSource1;
    private AudioSource m_soundSource2;
    private float m_volume = 1f;

    private Dictionary<string, AudioClip> m_audioDict = new Dictionary<string, AudioClip>();

    void Awake()
    {
        if (manager == null)
        {
            DontDestroyOnLoad(gameObject);
            manager = this;
        }
        else if (manager != this)
        {
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start()
    {
        //Add all of the audio clips to the dictionary
        m_audioDict.Add("game music", m_gameMusic);
        m_audioDict.Add("menu music", m_menuMusic);
        m_audioDict.Add("jump sound", m_jumpSound);
        m_audioDict.Add("death sound", m_deathSound);
        m_audioDict.Add("tap sound", m_tapSound);
        m_audioDict.Add("item sound", m_itemSound);
        //Define the audio sources
        m_musicSource = transform.GetChild(0).GetComponent<AudioSource>();
        m_soundSource1 = transform.GetChild(1).GetComponent<AudioSource>();
        m_soundSource2 = transform.GetChild(2).GetComponent<AudioSource>();
        ChangeVolume();
        //Initializer
        if (SceneManager.GetActiveScene().name != "Gameplay")
        {
            PlayClip("menu music", 1f, 0);
        }
    }

    public void ChangeVolume() {
        float playerVolume = PlayerPrefs.GetFloat("Volume") + 1.0f;
        m_musicSource.volume = playerVolume;
        m_soundSource1.volume = playerVolume;
        m_soundSource2.volume = playerVolume;
    }

    public void PlayClip(string name, float pitch, int source)
    {
        int start = name.IndexOf(" ") + 1;
        string type = name.Substring(start, name.Length - start);
        if (type == "music")
        {
            m_musicSource.Stop();
            m_musicSource.clip = m_audioDict[name];
            m_musicSource.pitch = pitch;
            m_musicSource.Play();
        }
        else if (type == "sound")
        {
            if (source == 1)
            {
                m_soundSource1.Stop();
                m_soundSource1.clip = m_audioDict[name];
                m_soundSource1.pitch = pitch;
                m_soundSource1.Play();
            }
            else if (source == 2)
            {
                m_soundSource2.Stop();
                m_soundSource2.clip = m_audioDict[name];
                m_soundSource2.pitch = pitch;
                m_soundSource2.Play();
            }
        }
    }

    public void PlayButtonTapSound() {
        m_soundSource2.Stop();
        m_soundSource2.clip = m_audioDict["tap sound"];
        m_soundSource2.pitch = 1.5f;
        m_soundSource2.Play();
    }

    public void StopMusic()
    {
        m_musicSource.Stop();
    }
}
