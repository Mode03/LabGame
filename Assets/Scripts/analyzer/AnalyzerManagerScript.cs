using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AnalyzerManagerScript : MonoBehaviour
{

    public int[,] analyzerItems = new int[5,12];
    public bool[] isBought = new bool[12];
    public float coins;
    public TMP_Text CoinsTxt;

    public Dictionary<int, string> ingredientDescriptions = new Dictionary<int, string>
    {
        {0, "Still water – No side effects. Just pure hydration. (Boring but safe.)"},
        {1, "Skibidite – Causes uncontrollable body movement and goofy dancing (Skibidi animation)."},
        {2, "Sigma Extract – Transforms the user into an 'alpha' mindset. Gains ultimate confidence."},
        {3, "Gyaatium – Stretches hips and legs for enhanced GYATT proportions. Pure thicc energy."},
        {4, "Toilet Core – Morphs the user's body into a toilet-shaped anomaly. Skibidi style achieved."},
        {5, "Neuron Dust – Decreases intelligence. Speech and behavior become progressively more goofy."},
        {6, "Ohio Crystal – Triggers glitch effects and unpredictable movement. Certified Ohio behavior."},
        {7, "Mew Juice – Enhances jawline angle. Mewing effect activated. Become the Chad."},
        {8, "Bomboclat Root – Induces rage outbursts and random yelling. Caribbean fury unlocked."},
        {9, "Crocodiline Oil – Gives user scaly skin and a deep crocodile voice. Reptilian mode ON."},
        {10, "Pre-GTA6 Essence – Temporarily hypes user about unreleased content. Enters delusional hype state."}
    };


    void Start()
    {
        CoinsTxt.text = "Coins:" + coins;

        // ID's
        for (int i = 1; i <= 11; i++)
        {
            analyzerItems[1, i] = i;
        }

        // Price
        analyzerItems[2, 0] = 5; // Still Water - pigesnis pvz
        analyzerItems[2, 1] = 10;
        analyzerItems[2, 2] = 20;
        analyzerItems[2, 3] = 30;
        analyzerItems[2, 4] = 40;
        analyzerItems[2, 5] = 50;
        analyzerItems[2, 6] = 60;
        analyzerItems[2, 7] = 70;
        analyzerItems[2, 8] = 80;
        analyzerItems[2, 9] = 90;
        analyzerItems[2, 10] = 100;
    }

    public void Buy()
    {
        GameObject ButtonRef = GameObject.FindGameObjectWithTag("Event").GetComponent<EventSystem>().currentSelectedGameObject;
        int id = ButtonRef.GetComponent<AnalyzerButtonInfo>().ItemID;

        if (!isBought[id]) // Tik jei dar nepirktas
        {
            if (coins >= analyzerItems[2, id])
            {
                coins -= analyzerItems[2, id];
                isBought[id] = true;

                CoinsTxt.text = "Coins:" + coins;

                // Pasikeicia mygtuko tekstas i "Purchased" (pvz.)
                ButtonRef.GetComponentInChildren<TMP_Text>().text = "Purchased";
            }
        }
    }
}
