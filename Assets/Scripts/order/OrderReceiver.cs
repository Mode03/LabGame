using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NUnit.Framework;
using System.Collections.Generic;
using GLTFast.Schema;
using System.Linq;
using System.Collections;
using Unity.Collections;
using Unity.VisualScripting;

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

    public Potion currentPotion;

    public PlayerDataManager player;

    public FeedbackUI feedbackUI;
    private float lastAccuracy;


    private Dictionary<string, string> ingredientHints = new Dictionary<string, string>
    {
        { "Still water", "hydration 100" },
        { "Neuron dust", "brain go brrrr" },
        { "Sigma extract", "grindset fuel" },
        { "Ohio crystal", "strange things happen when you touch it" },
        { "Toilet core", "found it in public bathroom" },
        { "Bomboclat root", "makes you shout for no reason" },
        { "Gyaatium", "high mass-per-volume asset extract" },
        { "Crocodiline oil", "illegal in 3 states and one swamp" },
        { "Skibidite", "whispering 'yes yes yes'..." },
        { "Mew juice", "makes your jawline dangerously defined" },
        { "Pre-gta6 essence", "older than time itself" }
    };

    void Start()
    {
        orderTextUI.SetActive(false);

        // Common
        potions.Add(new Potion("Goofy Ahh Serum", new Dictionary<string, float> {
            { "Still water", 35f },
            { "Neuron dust", 12f },
            { "Skibidite", 37f }
        }, PotionRarity.Common, 0, 40));

        potions.Add(new Potion("Sigma Juice Deluxe", new Dictionary<string, float> {
            { "Still water", 24f },
            { "Sigma extract", 13f },
            { "Neuron dust", 36f }
        }, PotionRarity.Common, 2, 40));

        potions.Add(new Potion("Toilet Rage Serum", new Dictionary<string, float> {
            { "Still water", 22f },
            { "Toilet core", 36f },
            { "Bomboclat root", 11f }
        }, PotionRarity.Common, 6, 50));

        potions.Add(new Potion("Tralalero Tralala Water", new Dictionary<string, float> {
            { "Still water", 17f },
            { "Gyaatium", 34f },
            { "Ohio crystal", 39f }
        }, PotionRarity.Common, 14, 60));

        // Rare
        potions.Add(new Potion("Crocodilo Bombardilo Brew", new Dictionary<string, float> {
            { "Still water", 13f },
            { "Crocodiline oil", 36f },
            { "Bomboclat root", 21f }
        }, PotionRarity.Rare, 4, 70));

        potions.Add(new Potion("Low Taper Fade Elixir", new Dictionary<string, float> {
            { "Still water", 30f },
            { "Mew juice", 35f },
            { "Sigma extract", 30f }
        }, PotionRarity.Rare, 10, 80));

        potions.Add(new Potion("Cooked Neuron Smoothie", new Dictionary<string, float> {
            { "Still water", 35f },
            { "Neuron dust", 30f },
            { "Ohio crystal", 25f }
        }, PotionRarity.Rare, 18, 90));

        // Epic
        potions.Add(new Potion("GYATT-O-RATE Ultra Edition", new Dictionary<string, float> {
            { "Gyaatium", 35f },
            { "Mew juice", 30f },
            { "Sigma extract", 30f }
        }, PotionRarity.Epic, 8, 100));

        potions.Add(new Potion("Ohio Disappearo", new Dictionary<string, float> {
            { "Ohio crystal", 30f },
            { "Toilet core", 35f },
            { "Pre-gta6 essence", 30f }
        }, PotionRarity.Epic, 12, 110));

        potions.Add(new Potion("Gyatt Gravity Reducer", new Dictionary<string, float> {
            { "Gyaatium", 35f },
            { "Ohio crystal", 30f },
            { "Sigma extract", 30f }
        }, PotionRarity.Epic, 20, 150));

        // Forbidden
        potions.Add(new Potion("Shrek's Swamp Juice", new Dictionary<string, float> {
            { "Crocodiline oil", 25f },
            { "Toilet core", 25f },
            { "Bomboclat root", 25f },
            { "Skibidite", 25f }
        }, PotionRarity.Forbidden, 16, 200));

        potions.Add(new Potion("GTA 6 Pre-Release Elixir", new Dictionary<string, float> {
            { "Pre-gta6 essence", 25f },
            { "Ohio crystal", 25f },
            { "Sigma extract", 25f },
            { "Neuron dust", 25f }
        }, PotionRarity.Forbidden, 24, 250));

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
    private Potion selected;

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
            filteredPotions = potions.FindAll(p => p.rarity == PotionRarity.Common && p.unlockLevel <= level);

            if (filteredPotions.Count == 0)
            {
                Debug.LogError($"Even fallback to common potions failed — no potions available for level {level}!");
                return;
            }
        }

        //Potion selectedPotion = filteredPotions[Random.Range(0, filteredPotions.Count)];
        Potion selectedPotion = potions.Find(p => p.name == "Shrek's Swamp Juice");
        selected = selectedPotion;
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
        int totalIngredients = selectedPotion.ingredients.Count;
        int hintsToShow = 0;

        // Hintų skaičius priklausomai nuo unlock lygio
        if (selectedPotion.unlockLevel <= 0) hintsToShow = totalIngredients;
        else if (selectedPotion.unlockLevel <= 6) hintsToShow = 2;
        else if (selectedPotion.unlockLevel <= 12) hintsToShow = 1;
        else hintsToShow = 0;

        int hintsShown = 0;

        foreach (var entry in selectedPotion.ingredients)
        {
            string ingredientName = entry.Key;
            float amount = entry.Value;

            int id = IngredientIDResolver.GetIngredientID(ingredientName);
            string displayName;

            //if (AnalyzerManagerScript.Instance != null && AnalyzerManagerScript.Instance.isBought[id])
            //{
            //    displayName = ingredientName;
            //}
            if (hintsShown < hintsToShow && ingredientHints.ContainsKey(ingredientName))
            {
                displayName = $"X{i}: " + ingredientHints[ingredientName];
                hintsShown++;
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
            Debug.Log("No active order!");
            return;
        }

        Bottle bottle = GetHeldBottle();
        if (bottle == null)
        {
            Debug.Log("Object doesnt have 'Bottle' script!");
            return;
        }

        SubmitPotion(bottle);
    }

    public void SubmitPotion(Bottle bottle)
    {
        float accuracy = EvaluatePotion(bottle);
        lastAccuracy = accuracy;
        string feedback = GetFeedbackText(accuracy);

        if (accuracy <= 0.5f)
        {
            Debug.Log("The order does not meet the requirements (accuracy is too low)");
            ShowFeedbackUI(feedback, accuracy);
            return;
        }

        int reward = Mathf.RoundToInt(accuracy * selected.price);
        int exp = Mathf.RoundToInt(accuracy * GetXPByRarity(currentPotion.rarity));

        player.AddCurrency(reward);
        ExperienceManager.Instance.AddExperience(exp);

        ShowFeedbackUI(feedback, accuracy);

        Debug.Log($"Accuracy: {accuracy:P0} | Coins: {reward} | XP: {exp}");

        CompleteOrder();
    }

    private float EvaluatePotion(Bottle bottle)
    {
        if (bottle.ingredients == null || currentOrderData == null)
            return 0f;

        int requiredCount = currentOrderData.Count;
        float accuracyPerIngredient = 1f / requiredCount; // pvz. 0.33f jeigu 3 ingridientai
        float totalAccuracy = 0f;

        foreach (var orderIngredient in currentOrderData)
        {
            var found = bottle.ingredients.Find(i =>
                i.name.Trim().ToLower() == orderIngredient.Key.Trim().ToLower());

            if (found == null)
            {
                // Ingredientas nerastas – uz ji 0% (nepridedam)
                continue;
            }

            // Uz tai kad yra teisingas ingredientas – puse vertes
            float ingredientAccuracy = accuracyPerIngredient * 0.5f;

            // Kiekybine dalis – papildoma iki kitos puses vertes
            float diff = Mathf.Abs(found.amount - orderIngredient.Value);
            float amountAccuracy = 0f;

            if (diff <= 1f)
            {
                amountAccuracy = 1f; // 100% tikslumas
            }
            else if (diff <= orderIngredient.Value)
            {
                // Proporcinis tikslumas (mazejantis nuo 1 iki 0)
                amountAccuracy = Mathf.Clamp01(1f - (diff / orderIngredient.Value));
            }

            ingredientAccuracy += accuracyPerIngredient * 0.5f * amountAccuracy;

            totalAccuracy += ingredientAccuracy;
        }

        // Papildomu ingredientu bauda
        int extraIngredients = bottle.ingredients
            .Count(i => !currentOrderData.Keys
                .Select(k => k.Trim().ToLower())
                .Contains(i.name.Trim().ToLower()));

        float penalty = 0.2f * extraIngredients;
        float finalAccuracy = totalAccuracy - penalty;

        return Mathf.Clamp01(finalAccuracy);
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

    private string GetFeedbackText(float accuracy)
    {
        if (accuracy >= 1f) return "[+++] PERFECT BREW\nThis belongs in a museum!";
        if (accuracy >= 0.9f) return "[+++] LEGENDARY BREW\nYou're cracked!";
        if (accuracy >= 0.7f) return "[++] NICE MIX\nAlmost top-tier.";
        if (accuracy >= 0.5f) return "[+] DECENT\nKinda mid but works.";
        if (accuracy >= 0.3f) return "[~] HMMM\nTechnically a potion.";
        return "[X] DISASTER\nThis is liquid chaos.";
    }

    private void ShowFeedbackUI(string feedbackText, float accuracy)
    {
        int reward = 0;
        int xp = 0;

        if (accuracy > 0.5f)
        {
            reward = Mathf.RoundToInt(accuracy * selected.price);
            xp = Mathf.RoundToInt(accuracy * GetXPByRarity(currentPotion.rarity));
        }

        StartCoroutine(ShowFeedbackWithDelay(accuracy, reward, xp, feedbackText));

        Debug.Log($"[Feedback] {feedbackText} ({accuracy * 100f:F0}%)");
    }

    IEnumerator ShowFeedbackWithDelay(float accuracy, int reward, int xp, string feedback)
    {
        yield return new WaitForSeconds(0.2f); // leisti dummy animacijai startuoti
        feedbackUI.Show(accuracy, reward, xp, feedback);
    }

    private int GetXPByRarity(PotionRarity rarity)
    {
        return rarity switch
        {
            PotionRarity.Common => 10,
            PotionRarity.Rare => 20,
            PotionRarity.Epic => 40,
            PotionRarity.Forbidden => 70,
            _ => 10
        };
    }

    private Bottle GetHeldBottle()
    {
        if (PlayerHeldObject.currentHeldObject != null)
        {
            return PlayerHeldObject.currentHeldObject.GetComponent<Bottle>();
        }

        return null;
    }

    public List<Potion> GetUnlockedPotionsAtLevel(int level)
    {
        List<Potion> unlocked = new();

        foreach (var p in potions)
        {
            if (p.unlockLevel == level)
            {
                unlocked.Add(p);
            }
        }

        return unlocked;
    }
    public float Accuracy => lastAccuracy;

}
