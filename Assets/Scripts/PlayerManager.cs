using System;
using UnityEngine;
using TMPro;
[Serializable]
public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance { get; private set; }

    public PlayerData playerData;
   // public TMP_Text CoinsTxt;

    public delegate void OnCurrencyChanged(int newAmount);
    public event OnCurrencyChanged CurrencyChanged;
    private void Awake()
    {
        // Ensure only one instance exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        //CurrencyChanged += (newAmount) => ShowCurrency(); // auto update CoinsTxt
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddCurrency(int amount)
    {
        playerData.Currency += amount;
        CurrencyChanged?.Invoke(playerData.Currency);
    }

    public void SubtractCurrency(int amount)
    {
        playerData.Currency -= amount;
        CurrencyChanged?.Invoke(playerData.Currency);
    }

    public int GetCurrency()
    {
        return playerData.Currency;
    }
    /*public void ShowCurrency()
    {
        if (CoinsTxt != null)
        {
            CoinsTxt.text = "Coins: " + playerData.Currency.ToString();
        }
        else
        {
            Debug.LogError("CoinsTxt reference is not assigned in PlayerDataManager!");
        }
    }*/

    public void SavePlayer()
    {
        SaveData data = new SaveData();
        data.currency = playerData.Currency;
        data.level = ExperienceManager.Instance.GetLevel();
        data.totalExperience = ExperienceManager.Instance.GetTotalExperience();

        data.isBought = AnalyzerManagerScript.Instance.isBought;

        // Saugokime LIKUS? LAIK?, ne tiksl? laikmat?
        for (int i = 0; i < 12; i++)
        {
            if (AnalyzerManagerScript.Instance.isBought[i])
            {
                float remaining = AnalyzerManagerScript.Instance.revealTimers[i] - Time.time;
                data.revealTimers[i] = Mathf.Max(0, remaining);
            }
            else
            {
                data.revealTimers[i] = 0;
            }
        }

        SaveSystem.SaveGame(data);
    }


    public void LoadPlayer()
    {
        SaveData data = SaveSystem.LoadGame();
        if (data == null)
            return;

        playerData.Currency = data.currency;
        CurrencyChanged?.Invoke(data.currency);

        ExperienceManager.Instance.SetLevelAndXP(data.level, data.totalExperience);

        AnalyzerManagerScript.Instance.isBought = data.isBought;

        // Nustatykime laikmat? naudodami Time.time + likus? laik?
        for (int i = 0; i < 12; i++)
        {
            if (data.isBought[i])
            {
                AnalyzerManagerScript.Instance.revealTimers[i] = Time.time + data.revealTimers[i];
            }
            else
            {
                AnalyzerManagerScript.Instance.revealTimers[i] = 0f;
            }
        }
    }


    void Start()
    {
        LoadPlayer();
    }

    void OnApplicationQuit()
    {
        SavePlayer();
    }

    public void ResetGame()
    {
        // 1. istrinti faila
        SaveSystem.ResetSave();

        // 2. Nustatyti viska nustatyti i default reiksmes
        playerData.Currency = 0;
        CurrencyChanged?.Invoke(0);

        ExperienceManager.Instance.SetLevelAndXP(1, 0); // arba 0 level jei tokia pradzia

        AnalyzerManagerScript.Instance.isBought = new bool[12];
        AnalyzerManagerScript.Instance.revealTimers = new float[12];

        // 3. Issaugoti tuscius duomenis
        SavePlayer();

        Debug.Log("Game set to start state");
    }


}
