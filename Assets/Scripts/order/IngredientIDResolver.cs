using System.Collections.Generic;
using UnityEngine;

public static class IngredientIDResolver
{
    private static Dictionary<string, int> nameToID = new Dictionary<string, int>
    {
        {"Still water", 0},
        {"Skibidite", 1},
        {"Sigma extract", 2},
        {"Gyaatium", 3},
        {"Toilet core", 4},
        {"Neuron dust", 5},
        {"Ohio crystal", 6},
        {"Mew juice", 7},
        {"Bomboclat root", 8},
        {"Crocodiline oil", 9},
        {"Pre-gta6 essence", 10}
    };

    public static int GetIngredientID(string name)
    {
        if (nameToID.TryGetValue(name, out int id))
        {
            return id;
        }

        Debug.LogWarning($"Ingredient ID not found for: {name}");
        return -1;
    }
}

