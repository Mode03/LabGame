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
            PriceTxt.text = "Price: " + manager.analyzerItems[2, ItemID];
            DescriptionTxt.text = ""; // tuscias, jei neisigytas
        }
    }

}
