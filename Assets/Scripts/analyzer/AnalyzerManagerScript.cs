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
    public PlayerDataManager player; 

    public float[] revealTimers = new float[12];
    public float[] ingredientAnalyzeDurations = new float[12]
    {
        15f, 30f, 45f, 60f, 90f, 120f, 140f, 160f, 180f, 240f, 300f, 0f
    };

    public Dictionary<int, string> ingredientDescriptions = new Dictionary<int, string>
    {
        {0, "Still water – No side effects. Just pure hydration. (Boring but safe.)<br><b><color=#CCCCCC>Tier Use:</color></b> <color=#CCCCCC>Common</color>, <color=#4AA6FF>Rare</color>, <color=#B86BFF>Epic</color><br><b><color=#CCCCCC>Included in:</color></b> Goofy Ahh Serum, Sigma Juice Deluxe, Toilet Rage Serum, Tralalero Tralala Water, Crocodilo Bombardilo Brew, Low Taper Fade Elixir, Cooked Neuron Smoothie"},

        {1, "Skibidite – Causes uncontrollable body movement and goofy dancing (Skibidi animation).<br><b><color=#CCCCCC>Tier Use:</color></b> <color=#CCCCCC>Common</color>, <color=#FF3B3B>Forbidden</color><br><b><color=#CCCCCC>Included in:</color></b> Goofy Ahh Serum, Shrek's Swamp Juice"},

        {2, "Sigma Extract – Transforms the user into an 'alpha' mindset. Gains ultimate confidence.<br><b><color=#CCCCCC>Tier Use:</color></b> <color=#CCCCCC>Common</color>, <color=#4AA6FF>Rare</color>, <color=#B86BFF>Epic</color>, <color=#FF3B3B>Forbidden</color><br><b><color=#CCCCCC>Included in:</color></b> Sigma Juice Deluxe, Low Taper Fade Elixir, GYATT-O-RATE Ultra Edition, Gyatt Gravity Reducer, GTA 6 Pre-Release Elixir"},

        {3, "Gyaatium – Stretches hips and legs for enhanced GYATT proportions. Pure thicc energy.<br><b><color=#CCCCCC>Tier Use:</color></b> <color=#CCCCCC>Common</color>, <color=#B86BFF>Epic</color><br><b><color=#CCCCCC>Included in:</color></b> Tralalero Tralala Water, GYATT-O-RATE Ultra Edition, Gyatt Gravity Reducer"},

        {4, "Toilet Core – Morphs the user's body into a toilet-shaped anomaly. Skibidi style achieved.<br><b><color=#CCCCCC>Tier Use:</color></b> <color=#CCCCCC>Common</color>, <color=#B86BFF>Epic</color>, <color=#FF3B3B>Forbidden</color><br><b><color=#CCCCCC>Included in:</color></b> Toilet Rage Serum, Ohio Disappearo, Shrek's Swamp Juice"},

        {5, "Neuron Dust – Decreases intelligence. Speech and behavior become progressively more goofy.<br><b><color=#CCCCCC>Tier Use:</color></b> <color=#CCCCCC>Common</color>, <color=#4AA6FF>Rare</color>, <color=#FF3B3B>Forbidden</color><br><b><color=#CCCCCC>Included in:</color></b> Goofy Ahh Serum, Sigma Juice Deluxe, Cooked Neuron Smoothie, GTA 6 Pre-Release Elixir"},

        {6, "Ohio Crystal – Triggers glitch effects and unpredictable movement. Certified Ohio behavior.<br><b><color=#CCCCCC>Tier Use:</color></b> <color=#CCCCCC>Common</color>, <color=#4AA6FF>Rare</color>, <color=#B86BFF>Epic</color>, <color=#FF3B3B>Forbidden</color><br><b><color=#CCCCCC>Included in:</color></b> Tralalero Tralala Water, Cooked Neuron Smoothie, Ohio Disappearo, Gyatt Gravity Reducer, GTA 6 Pre-Release Elixir"},

        {7, "Mew Juice – Enhances jawline angle. Mewing effect activated. Become the Chad.<br><b><color=#CCCCCC>Tier Use:</color></b> <color=#4AA6FF>Rare</color>, <color=#B86BFF>Epic</color><br><b><color=#CCCCCC>Included in:</color></b> Low Taper Fade Elixir, GYATT-O-RATE Ultra Edition"},

        {8, "Bomboclat Root – Induces rage outbursts and random yelling. Caribbean fury unlocked.<br><b><color=#CCCCCC>Tier Use:</color></b> <color=#CCCCCC>Common</color>, <color=#4AA6FF>Rare</color>, <color=#FF3B3B>Forbidden</color><br><b><color=#CCCCCC>Included in:</color></b> Toilet Rage Serum, Crocodilo Bombardilo Brew, Shrek's Swamp Juice"},

        {9, "Crocodiline Oil – Gives user scaly skin and a deep crocodile voice. Reptilian mode ON.<br><b><color=#CCCCCC>Tier Use:</color></b> <color=#4AA6FF>Rare</color>, <color=#FF3B3B>Forbidden</color><br><b><color=#CCCCCC>Included in:</color></b> Crocodilo Bombardilo Brew, Shrek's Swamp Juice"},

        {10, "Pre-GTA6 Essence – Temporarily hypes user about unreleased content. Enters delusional hype state.<br><b><color=#CCCCCC>Tier Use:</color></b> <color=#B86BFF>Epic</color>, <color=#FF3B3B>Forbidden</color><br><b><color=#CCCCCC>Included in:</color></b> Ohio Disappearo, GTA 6 Pre-Release Elixir"}
    };



    public static AnalyzerManagerScript Instance;

    void Awake()
    {
        Instance = this;
    }


    void Start()
    {

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
    void Update()
    {
         if (player != null)
        {
            CoinsTxt.text = "Coins: " + player.GetCurrency().ToString();
        }
        else
        {
            if (player == null)
            Debug.LogError("PlayerDataManager (player) is null.");

            if (CoinsTxt == null)
            Debug.LogError("CoinsTxt is null.");
        }
    }
    public void Buy()
    {
        GameObject ButtonRef = GameObject.FindGameObjectWithTag("Event").GetComponent<EventSystem>().currentSelectedGameObject;
        int id = ButtonRef.GetComponent<AnalyzerButtonInfo>().ItemID;

        if (!isBought[id]) // Tik jei dar nepirktas
        {
            // Patikrinam ar kitas analizes procesas vyksta
            for (int i = 0; i < revealTimers.Length; i++)
            {
                if (isBought[i] && Time.time < revealTimers[i])
                {
                    Debug.Log("Another analysis is already underway.");
                    return; // Iseinam is funkcijos � leidziam tik viena analize vienu metu
                }
            }

            // Jei turim pakankamai monetu
            if (player.GetCurrency() >= analyzerItems[2, id])
            {
                player.SubtractCurrency(analyzerItems[2, id]);
                isBought[id] = true;
                revealTimers[id] = Time.time + ingredientAnalyzeDurations[id];

                CoinsTxt.text = "Coins:" + player.GetCurrency().ToString();

                // Pasikeicia mygtuko tekstas i "Purchased" (pvz.)
                ButtonRef.GetComponentInChildren<TMP_Text>().text = "Purchased";

                AudioManager.Instance.PlaySFX(AudioManager.Instance.analyzerStartClip);
            }
        }
    }

}
