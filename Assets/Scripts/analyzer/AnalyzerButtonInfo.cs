using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AnalyzerButtonInfo : MonoBehaviour
{
    public int ItemID;
    public TMP_Text PriceTxt;
    public TMP_Text DescriptionTxt;
    public GameObject AnalyzerManager;

    void Update()
    {
        var manager = AnalyzerManager.GetComponent<AnalyzerManagerScript>();

        if (manager.isBought[ItemID])
        {
            PriceTxt.text = "";

            if (Time.time >= manager.revealTimers[ItemID])
            {
                // parodyti tikra aprasyma
                if (manager.ingredientDescriptions.ContainsKey(ItemID))
                {
                    DescriptionTxt.text = manager.ingredientDescriptions[ItemID];
                }
                else
                {
                    DescriptionTxt.text = "No description available.";
                }
            }
            else
            {
                // Laikmatis iki analizes pabaigos
                float remainingTime = manager.revealTimers[ItemID] - Time.time;
                remainingTime = Mathf.Max(0, remainingTime);

                int minutes = Mathf.FloorToInt(remainingTime / 60);
                int seconds = Mathf.FloorToInt(remainingTime % 60);

                DescriptionTxt.text = string.Format("Analyzing...\n{0:D2}:{1:D2}", minutes, seconds);
            }
        }
        else
        {
            PriceTxt.text = "Price: " + manager.analyzerItems[2, ItemID];
            DescriptionTxt.text = ""; // tuscias, jei neisigytas
        }
    }

}
