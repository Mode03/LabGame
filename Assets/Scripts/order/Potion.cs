using System.Collections.Generic;
using UnityEngine;

public enum PotionRarity
{
    Common,
    Rare,
    Epic,
    Forbidden
}

[System.Serializable]
public class Potion
{
    public string name;
    public Dictionary<string, float> ingredients;
    public PotionRarity rarity;
    public int unlockLevel;

    public Potion(string name, Dictionary<string, float> ingredients, PotionRarity rarity, int unlockLevel)
    {
        this.name = name;
        this.ingredients = ingredients;
        this.rarity = rarity;
        this.unlockLevel = unlockLevel;
    }
}

