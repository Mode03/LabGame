using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NUnit.Framework;
using System.Collections.Generic;
using GLTFast.Schema;

public class OrderReceiver : MonoBehaviour
{
    public GameObject orderTextUI;  // „Take Order [E]“
    public TMP_Text orderDisplayUI; // order info
    public Transform playerCamera;  // zaidejo kamera

    private bool isLookingAtOrderPoint = false;
    private string currentOrder;
    public bool orderActive = false;

    private List<Potion> potions = new List<Potion>();

    public OrderNPC npcMovement;

    private Potion currentPotion;

    void Start()
    {
        orderTextUI.SetActive(false);

        // Common
        potions.Add(new Potion("Goofy Ahh Serum", new Dictionary<string, float> {
            { "Still water", 35f },
            { "Neuron dust", 12f },
            { "Skibidite", 37f }
        }, PotionRarity.Common, 0));

        potions.Add(new Potion("Sigma Juice Deluxe", new Dictionary<string, float> {
            { "Still water", 24f },
            { "Sigma extract", 13f },
            { "Neuron dust", 36f }
        }, PotionRarity.Common, 3));

        potions.Add(new Potion("Toilet Rage Serum", new Dictionary<string, float> {
            { "Still water", 22f },
            { "Toilet core", 36f },
            { "Bomboclat root", 11f }
        }, PotionRarity.Common, 5));

        potions.Add(new Potion("Tralalero Tralala Water", new Dictionary<string, float> {
            { "Still water", 17f },
            { "Gyaatium", 34f },
            { "Ohio crystal", 39f }
        }, PotionRarity.Common, 8));

        // Rare
        potions.Add(new Potion("Crocodilo Bombardilo Brew", new Dictionary<string, float> {
            { "Still water", 13f },
            { "Crocodiline oil", 36f },
            { "Bomboclat root", 21f }
        }, PotionRarity.Rare, 10));

        potions.Add(new Potion("Low Taper Fade Elixir", new Dictionary<string, float> {
            { "Still water", 30f },
            { "Mew juice", 35f },
            { "Sigma extract", 30f }
        }, PotionRarity.Rare, 13));

        potions.Add(new Potion("Cooked Neuron Smoothie", new Dictionary<string, float> {
            { "Still water", 35f },
            { "Neuron dust", 30f },
            { "Ohio crystal", 25f }
        }, PotionRarity.Rare, 15));

        // Epic
        potions.Add(new Potion("GYATT-O-RATE Ultra Edition", new Dictionary<string, float> {
            { "Gyaatium", 35f },
            { "Mew juice", 30f },
            { "Sigma extract", 30f }
        }, PotionRarity.Epic, 18));

        potions.Add(new Potion("Ohio Disappearo", new Dictionary<string, float> {
            { "Ohio crystal", 30f },
            { "Toilet core", 35f },
            { "Pre-gta6 essence", 30f }
        }, PotionRarity.Epic, 20));

        potions.Add(new Potion("Gyatt Gravity Reducer", new Dictionary<string, float> {
            { "Gyaatium", 35f },
            { "Ohio crystal", 30f },
            { "Sigma extract", 30f }
        }, PotionRarity.Epic, 23));

        // Forbidden
        potions.Add(new Potion("Shrek's Swamp Juice", new Dictionary<string, float> {
            { "Crocodiline oil", 25f },
            { "Toilet core", 25f },
            { "Bomboclat root", 25f },
            { "Skibidite", 25f }
        }, PotionRarity.Forbidden, 25));

        potions.Add(new Potion("GTA 6 Pre-Release Elixir", new Dictionary<string, float> {
            { "Pre-gta6 essence", 25f },
            { "Ohio crystal", 25f },
            { "Sigma extract", 25f },
            { "Neuron dust", 25f }
        }, PotionRarity.Forbidden, 30));

    }

    void Update()
    {
        CheckForOrderPoint();

        if (isLookingAtOrderPoint && Input.GetKeyDown(KeyCode.E))
        {
            if (!orderActive) // Tik jeigu nera aktyvaus uzsakymo
            {
                GenerateOrder();
                orderActive = true;
                
                if (npcMovement != null) // npc iseina
                {
                    npcMovement.GiveOrder();
                }

            }
        }
    }

    public void CheckForOrderPoint()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 3f)) // max atstumas
        {
            if (hit.collider.CompareTag("OrderPoint")) // Objektas turi tureti sita tag
            {
                orderTextUI.SetActive(true);
                isLookingAtOrderPoint = true;
                return;
            }
        }

        orderTextUI.SetActive(false);
        isLookingAtOrderPoint = false;
    }

    public Dictionary<string, float> currentOrderData = new(); // ingredientas -> kiekis

    private void GenerateOrder()
    {
        currentOrderData.Clear();

        float roll = Random.Range(0f, 100f);
        PotionRarity selectedRarity;

        if (roll < 60f) selectedRarity = PotionRarity.Common;
        else if (roll < 85f) selectedRarity = PotionRarity.Rare;
        else if (roll < 95f) selectedRarity = PotionRarity.Epic;
        else selectedRarity = PotionRarity.Forbidden;

        int level = ExperienceManager.Instance.GetLevel();

        List<Potion> filteredPotions = potions.FindAll(p =>
        p.rarity == selectedRarity && p.unlockLevel <= level);

        if (filteredPotions.Count == 0)
        {
            Debug.LogWarning($"No unlocked potions of rarity {selectedRarity} for level {level}! Falling back to common.");
            filteredPotions = potions.FindAll(p => p.rarity == PotionRarity.Common);
        }

        Potion selectedPotion = filteredPotions[Random.Range(0, filteredPotions.Count)];

        foreach (var entry in selectedPotion.ingredients)
        {
            currentOrderData.Add(entry.Key, entry.Value);
        }

        string colorHex = selectedPotion.rarity switch
        {
            PotionRarity.Common => "#CCCCCC",
            PotionRarity.Rare => "#4AA6FF",
            PotionRarity.Epic => "#B86BFF",
            PotionRarity.Forbidden => "#FF3B3B",
            _ => "#FFFFFF"
        };

        string coloredName = $"<b><color={colorHex}>{selectedPotion.name}</color></b>";

        string orderText = $"{coloredName}\n";
        int i = 1;
        foreach (var entry in selectedPotion.ingredients)
        {
            string ingredientName = entry.Key;
            float amount = entry.Value;

            string displayName;

            // Gauk ID pagal pavadinimą (čia reiks ingredientų ID sąsajos – padarysim toliau)
            int id = IngredientIDResolver.GetIngredientID(ingredientName);

            if (AnalyzerManagerScript.Instance != null && AnalyzerManagerScript.Instance.isBought[id])
            {
                displayName = ingredientName;
            }
            else
            {
                displayName = $"X{i}";
            }

            orderText += $"- {displayName}: {amount:F0}ml\n";
            i++;
        }

        currentOrder = orderText;
        orderDisplayUI.text = currentOrder;

        currentPotion = selectedPotion;
    }

    public void CheckHeldBottle()
    {
        if (!orderActive || currentOrderData.Count == 0)
        {
            Debug.Log("Nėra aktyvaus užsakymo!");
            return;
        }

        Bottle bottle = GetHeldBottle();
        if (bottle == null)
        {
            Debug.Log("Objektas neturi 'Bottle' skripto!");
            return;
        }

        if (IsBottleCorrect(bottle))
        {
            Debug.Log($"Atitinka uzsakyma! Tirpalo pavadinimas: {currentPotion.name}");
            CompleteOrder();
        }
        else
        {
            Debug.Log("Neatitinka uzsakymo!");
        }
    }

    private void CompleteOrder()
    {
        currentOrderData.Clear();
        currentOrder = "";
        orderDisplayUI.text = "";
        orderActive = false; // Uzsakymas baigtas, leidziam nauja NPC

        npcMovement.ResetNPC(); // NPC grizta i pradzia

        int expAmount = currentPotion.rarity switch
        {
            PotionRarity.Common => 10,
            PotionRarity.Rare => 20,
            PotionRarity.Epic => 40,
            PotionRarity.Forbidden => 70,
            _ => 10
        };

        ExperienceManager.Instance.AddExperience(expAmount);
        Debug.Log($"Gained {expAmount} EXP for {currentPotion.name}!");
    }

    private bool IsBottleCorrect(Bottle bottle)
    {
        if (bottle.ingredients.Count != currentOrderData.Count)
            return false;

        foreach (var orderIngredient in currentOrderData)
        {
            var found = bottle.ingredients.Find(i => i.name == orderIngredient.Key);
            if (found == null)
                return false;

            float diff = Mathf.Abs(found.amount - orderIngredient.Value);
            if (diff > 5f)
                return false;
        }

        return true;
    }

    private Bottle GetHeldBottle()
    {
        if (PlayerHeldObject.currentHeldObject != null)
        {
            return PlayerHeldObject.currentHeldObject.GetComponent<Bottle>();
        }

        return null;
    }

}
