using System;
using UnityEngine;
using TMPro;
[Serializable]
public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance { get; private set; }

    public PlayerData playerData;
    public TMP_Text CoinsTxt;

    public delegate void OnCurrencyChanged(int newAmount);
    public event OnCurrencyChanged CurrencyChanged;
    private void Start()
    {
         CoinsTxt.text = "Coins: " + playerData.Currency.ToString();
    }
    private void Awake()
    {
        // Ensure only one instance exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        CurrencyChanged += (newAmount) => ShowCurrency(); // auto update CoinsTxt
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
    public void ShowCurrency()
    {
        if (CoinsTxt != null)
        {
            CoinsTxt.text = "Coins: " + playerData.Currency.ToString();
        }
        else
        {
            Debug.LogError("CoinsTxt reference is not assigned in PlayerDataManager!");
        }
    }
}
