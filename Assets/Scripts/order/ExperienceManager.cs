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

    void Start()
    {
        UpdateLevel();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            AddExperience(10);
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
        int tier = (currentLevel % 5) + 1; // I–V

        levelText.text = $"{rankName} {ToRoman(tier)}";
        experienceText.text = start + " exp / " + end + " exp";
        experienceFill.fillAmount = (float)start / (float)end;
    }

    private string GetRankName(int level)
    {
        if (level < 5) return "Intern Sipper";
        if (level < 10) return "Lab Rat";
        if (level < 15) return "Mixer Monkey";
        if (level < 20) return "Potion Rookie";
        if (level < 25) return "Certified Brewer";
        if (level < 30) return "Recipe Manipulator";
        if (level < 35) return "Alchemical Dealer";
        if (level < 40) return "Meme Distiller";
        if (level < 45) return "High Rizz Alchemist";
        if (level < 50) return "Forbidden Mixer";
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
            case 5: return "V";
            default: return "";
        }
    }

    public int GetLevel()
    {
        return currentLevel;
    }

}
