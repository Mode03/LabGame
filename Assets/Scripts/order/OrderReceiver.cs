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

    void Start()
    {
        orderTextUI.SetActive(false);

        potions.Add(new Potion("GigaGoofy Mix", new Dictionary<string, float> {
            { "skibidi", 30f },
            { "boy", 25f },
            { "toilet", 30f },
            { "H2O", 10f }
        }, PotionRarity.Common));

        potions.Add(new Potion("Toilet Rage Serum", new Dictionary<string, float> {
            { "toilet", 40f },
            { "skibidi", 30f },
            { "rot", 25f }
        }, PotionRarity.Common));

        potions.Add(new Potion("NPC Whisper Brew", new Dictionary<string, float> {
            { "rot", 30f },
            { "toilet", 30f },
            { "H2O", 30f }
        }, PotionRarity.Common));

        potions.Add(new Potion("Skibidi Brain Melter", new Dictionary<string, float> {
            { "skibidi", 33f },
            { "boy", 28f },
            { "chad", 35f }
        }, PotionRarity.Rare));

        potions.Add(new Potion("Chad Flex Elixir", new Dictionary<string, float> {
            { "chad", 38f },
            { "sigma", 32f },
            { "H2O", 25f }
        }, PotionRarity.Rare));

        potions.Add(new Potion("Sigma Juice Deluxe", new Dictionary<string, float> {
            { "sigma", 35f },
            { "H2O", 30f },
            { "boy", 25f }
        }, PotionRarity.Rare));

        potions.Add(new Potion("Crocodilo Bombardilo Brew", new Dictionary<string, float> {
            { "giga essence", 35f },
            { "skibidi", 30f },
            { "toilet", 28f }
        }, PotionRarity.Rare));

        potions.Add(new Potion("Ohio Disappearo", new Dictionary<string, float> {
            { "skibidi", 40f },
            { "H2O", 35f },
            { "sigma", 20f }
        }, PotionRarity.Epic));

        potions.Add(new Potion("Gyatt Gravity Reducer", new Dictionary<string, float> {
            { "chad", 40f },
            { "giga essence", 35f },
            { "H2O", 20f }
        }, PotionRarity.Epic));

        potions.Add(new Potion("Shrekified Gas", new Dictionary<string, float> {
            { "rot", 35f },
            { "toilet", 35f },
            { "npc dust", 25f }
        }, PotionRarity.Epic));

        potions.Add(new Potion("GTA 6 Pre-Release Elixir", new Dictionary<string, float> {
            { "giga essence", 40f },
            { "rizz powder", 35f },
            { "npc dust", 20f }
        }, PotionRarity.Forbidden));
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

        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    CheckHeldBottle();
        //}
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

        List<Potion> filteredPotions = potions.FindAll(p => p.rarity == selectedRarity);

        if (filteredPotions.Count == 0)
        {
            Debug.LogWarning($"No potions of rarity {selectedRarity} found! Falling back to common.");
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
        foreach (var amount in selectedPotion.ingredients.Values)
        {
            orderText += $"- X{i}: {amount:F0}ml\n";
            i++;
        }

        currentOrder = orderText;
        orderDisplayUI.text = currentOrder;
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
            Debug.Log("Atitinka uzsakyma!");
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
