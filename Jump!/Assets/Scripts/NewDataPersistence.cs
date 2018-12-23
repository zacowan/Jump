using UnityEngine;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SocialPlatforms;

public class NewDataPersistence : MonoBehaviour {

    public static NewDataPersistence data;

    private const string FILE_NAME = "/playerInfo.dat";

    private List<DataObject> m_dataList = new List<DataObject>();
    private List<DataObject> m_roundDataList = new List<DataObject>();

	private const string DEFAULT_CHARACTER_NAME = "Green Alien";
    public DateTime currentDate;
    public DateTime oldDate;
    public DateTime timeSinceLastAdRewardTypeCurrency;
    public DateTime timeSinceLastAdRewardTypeExtraLife;

    //Makes the object a singleton (there can only be one)
    void Awake()
    {
        if (data == null)
        {
            DontDestroyOnLoad(gameObject);
            data = this;
        }
        else if (data != this)
        {
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start () {
        m_dataList.Add(new DataObject("int", "Coins", 0));
        m_dataList.Add(new DataObject("int", "Keys", 0));
        m_dataList.Add(new DataObject("float", "High Score", 0f));
        m_dataList.Add(new DataObject("bool", DEFAULT_CHARACTER_NAME, true));
        m_dataList.Add(new DataObject("bool", "Beige Alien", false));
        m_dataList.Add(new DataObject("bool", "Blue Alien", false));
        m_dataList.Add(new DataObject("bool", "Pink Alien", false));
        m_dataList.Add(new DataObject("bool", "Yellow Alien", false));
        m_dataList.Add(new DataObject("bool", "Red Alien", false));
        m_dataList.Add(new DataObject("bool", "Panda", false));
        m_roundDataList.Add(new DataObject("int", "Coins", 0));
        m_roundDataList.Add(new DataObject("int", "Keys", 0));
        m_roundDataList.Add(new DataObject("float", "High Score", 0f));
        Load();
		if (PlayerPrefs.GetString ("Character") == "") {
			Debug.Log ("Set default character");
			PlayerPrefs.SetString ("Character", DEFAULT_CHARACTER_NAME);
		}
        currentDate = System.DateTime.Now;
        if (PlayerPrefs.GetString("oldDate") != "")
        {
            long temp = Convert.ToInt64(PlayerPrefs.GetString("oldDate"));
            oldDate = DateTime.FromBinary(temp);
            Debug.Log("Old Date: " + oldDate);
        }
        if (PlayerPrefs.GetString("timeSinceLastAdRewardTypeCurrency") != "") {
            long temp = Convert.ToInt64(PlayerPrefs.GetString("timeSinceLastAdRewardTypeCurrency"));
            timeSinceLastAdRewardTypeCurrency = DateTime.FromBinary(temp);
            Debug.Log("Time Since Last Ad Reward Type Currency: " + timeSinceLastAdRewardTypeCurrency);
        }
        if (PlayerPrefs.GetString("timeSinceLastAdRewardTypeExtraLife") != "") {
            long temp = Convert.ToInt64(PlayerPrefs.GetString("timeSinceLastAdRewardTypeExtraLife"));
            timeSinceLastAdRewardTypeExtraLife = DateTime.FromBinary(temp);
            Debug.Log("Time Since Last Ad Reward Type Extra Life: " + timeSinceLastAdRewardTypeExtraLife);
        }
        SocialPlatformController.controller.SetHighScore(GetValue("High Score", false));
    }

    public void Save()
    {
        if (File.Exists(Application.persistentDataPath + FILE_NAME))
        {
            File.Delete(Application.persistentDataPath + FILE_NAME);
            Debug.Log("Old save file deleted");
        }
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + FILE_NAME);
        NewPlayerData saveData = new NewPlayerData();
        foreach (DataObject listedData in m_dataList)
        {
            saveData.m_savedDataList.Add(listedData);
        }
        bf.Serialize(file, saveData);
        file.Close();
        Debug.Log("Saved");
    }
	
	public void Load()
    {
        if (File.Exists(Application.persistentDataPath + FILE_NAME))
        {
            //File.Delete(Application.persistentDataPath + FILE_NAME);
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + FILE_NAME, FileMode.Open);
            NewPlayerData loadData = (NewPlayerData)bf.Deserialize(file);
            file.Close();
            foreach (DataObject savedData in loadData.m_savedDataList)
            {
                foreach (DataObject defaultData in m_dataList)
                {
                    if (defaultData.GetName() == savedData.GetName())
                    {
                        defaultData.SetValue(savedData.GetValue());
                    }
                }
            }
            Debug.Log("Loaded");
        } else
        {
            Debug.Log("No save data found");
        }
    }

    public void SetValue(string name, object value, bool isRoundValue)
    {
        if (isRoundValue)
        {
            foreach (DataObject listedData in m_roundDataList)
            {
                if (listedData.GetName() == name)
                {
                    listedData.SetValue(value);
                }
            }
            GameController.instance.DisplayRoundItems((int)GetValue("Coins", true), (int)GetValue("Keys", true));
        }
        else
        {
            foreach (DataObject listedData in m_dataList)
            {
                if (listedData.GetName() == name)
                {
                    listedData.SetValue(value);
                }
            }
        }
    }

    public object GetValue(string name, bool isRoundValue)
    {
        object returner = null;
        if (isRoundValue)
        {
            foreach (DataObject listedData in m_roundDataList)
            {
                if (listedData.GetName() == name)
                {
                    returner = listedData.GetValue();
                    break;
                }
            }
        } 
        else
        {
            foreach (DataObject listedData in m_dataList)
            {
                if (listedData.GetName() == name)
                {
                    returner = listedData.GetValue();
                    break;
                }
            }
        }
        return returner;
    }

    public void PlayerDead()
    {
        foreach (DataObject listedData in m_dataList)
        {
            foreach (DataObject roundData in m_roundDataList)
            {
                if (listedData.GetName() == roundData.GetName())
                {
                    if (listedData.GetValueType() == "int")
                    {
                        listedData.SetValue((int)listedData.GetValue() + (int)roundData.GetValue());
                    }
                    else if (listedData.GetValueType() == "float")
                    {
                        if ((int)(float)roundData.GetValue() > (int)(float)listedData.GetValue())
                        {
                            listedData.SetValue(roundData.GetValue());
                            SocialPlatformController.controller.SetHighScore(GetValue("High Score", false));
                        }
                    }
                }
            }
        }
        Save();
    }

    public void ResetGame()
    {
        foreach (DataObject roundData in m_roundDataList)
        {
            if (roundData.GetValueType() == "int")
            {
                roundData.SetValue(0);
            }
            else if (roundData.GetValueType() == "float")
            {
                roundData.SetValue(0f);
            }
        }
        Time.timeScale = 1f;
    }

    //Called when a player got an extra life
    public void ModifiedResetGame()
    {
        foreach (DataObject listedData in m_dataList)
        {
            foreach (DataObject roundData in m_roundDataList)
            {
                if (listedData.GetName() == roundData.GetName())
                {
                    if (listedData.GetValueType() == "int")
                    {
                        listedData.SetValue((int)listedData.GetValue() - (int)roundData.GetValue());
                    }
                }
            }
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            if (GameController.instance && !Player.info.CheckIfPlayerDead())
            {
                GameController.instance.ChangeGamePauseState(true);
            }
            else
            {
                Save();
            }
        }
    }

    private void OnApplicationQuit()
    {
        if (GameController.instance && Player.info.CheckIfPlayerDead())
        {
            Save();
        }
        else if (!GameController.instance)
        {
            Save();
        }
        PlayerPrefs.SetString("oldDate", System.DateTime.Now.ToBinary().ToString());
        PlayerPrefs.SetString("timeSinceLastAdRewardTypeCurrency", timeSinceLastAdRewardTypeCurrency.ToBinary().ToString());
        PlayerPrefs.SetString("timeSinceLastAdRewardTypeExtraLife", timeSinceLastAdRewardTypeExtraLife.ToBinary().ToString());
        Debug.Log("Saved Date: " + System.DateTime.Now);
    }

    public double ComputeTimeDifference(DateTime firstDate, DateTime secondDate) {
        TimeSpan difference = secondDate.Subtract(firstDate);
        return difference.TotalMinutes;
    }
}

[Serializable]
class DataObject
{
    public DataObject(string type, string name, object value)
    {
        m_type = type;
        m_name = name;
        m_value = value;
    }

    private string m_type;
    private string m_name;
    private object m_value;
    
    public string GetName()
    {
        return m_name;
    }

    public object GetValue()
    {
        return m_value;
    }

    public void SetValue(object value)
    {
        m_value = value;
    }

    public string GetValueType()
    {
        return m_type;
    }
}

[Serializable]
class NewPlayerData
{
    public List<DataObject> m_savedDataList = new List<DataObject>();
}
