using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using System;

public class ExperienceManager : MonoBehaviour
{
    public static ExperienceManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    [Header("Experience")]
    [SerializeField] AnimationCurve experienceCurve;

    int currentLevel, totalExperience;
    int previousLevelsExperience, nextLevelsExperience;

    [Header("Interface")]
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI experienceText;
    [SerializeField] Image experienceFill;

    [SerializeField] private PotionUnlockUI unlockUI;
    [SerializeField] private OrderReceiver orderReceiver;

    void Start()
    {
        UpdateLevel();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            AddExperience(25);
        }
    }

    public void AddExperience(int amount)
    {
        totalExperience += amount;
        CheckForLevelUp();
        UpdateInterface();
    }

    void CheckForLevelUp()
    {
        if(totalExperience >= nextLevelsExperience)
        {
            currentLevel++;
            UpdateLevel();

            // Nauji potion'ai
            var unlockedPotions = orderReceiver.GetUnlockedPotionsAtLevel(currentLevel);
            foreach (var potion in unlockedPotions)
            {
                unlockUI.Show(potion.name, potion.rarity); // parodyk atrakinta potion
            }

            // Start level up sequence ... vfx and sound?
        }
    }

    void UpdateLevel()
    {
        previousLevelsExperience = (int)experienceCurve.Evaluate(currentLevel);
        nextLevelsExperience = (int)experienceCurve.Evaluate(currentLevel + 1);
        UpdateInterface();
    }

    private void UpdateInterface()
    {
        int start = totalExperience - previousLevelsExperience;
        int end = nextLevelsExperience - previousLevelsExperience;

        string rankName = GetRankName(currentLevel);
        int tier = (currentLevel % 4) + 1; // I–V

        levelText.text = $"{rankName} {ToRoman(tier)}";
        experienceText.text = start + " exp / " + end + " exp";
        experienceFill.fillAmount = (float)start / (float)end;
    }

    private string GetRankName(int level)
    {
        if (level < 4) return "Intern Sipper";
        if (level < 8) return "Lab Rat";
        if (level < 12) return "Mixer Monkey";
        if (level < 16) return "Potion Rookie";
        if (level < 20) return "Certified Brewer";
        if (level < 24) return "Recipe Manipulator";
        if (level < 28) return "Alchemical Dealer";
        if (level < 32) return "Meme Distiller";
        if (level < 36) return "High Rizz Alchemist";
        if (level < 40) return "Forbidden Mixer";
        return "Drinklord";
    }

    private string ToRoman(int number)
    {
        switch (number)
        {
            case 1: return "I";
            case 2: return "II";
            case 3: return "III";
            case 4: return "IV";
            default: return "";
        }
    }

    public int GetLevel()
    {
        return currentLevel;
    }

}
