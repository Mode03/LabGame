using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
   public int coins;
   public TMP_Text coinUI; 
   public Spicies[] ShopItems;
   public GameObject[] ShopPanelsSO;
   public ShopTemplate[] ShopPanels;
   public Button[] BuyButton;
   public ShopTrigger shopTrigger; // Reference to ShopTrigger
   public Transform[] itemSpawnPoints;
    public Camera playerCamera;
    public GameObject infoPanel;
    public TextMeshProUGUI infoText;
     public void Exit()
    {
        if (shopTrigger != null)
        {
            shopTrigger.ExitShop(); // Call ExitShop from the instance
        }
        else
        {
            Debug.LogError("ShopTrigger reference is missing in ShopManager!");
        }
    }
   void Start()
    {
        for(int i = 0; i < ShopItems.Length;i++)
        {
             ShopPanelsSO[i].SetActive(true);
        }
        coinUI.text = "Coins: " + coins.ToString();
        LoadPanels();
        CheckPurchase();
    }
   public void AddCoins()
   {
    coins += 50;
    coinUI.text = "Coins: " + coins.ToString();
    CheckPurchase();
   }
   public void CheckPurchase()
   {
    for(int i = 0; i < ShopItems.Length;i++)
    {
        if(coins >= ShopItems[i].price)
        {
            BuyButton[i].interactable = true;
        }
        else
        {
            BuyButton[i].interactable = false;
        }
    }
   }
   public void purchaseItem(int btnNo)
{
    if (coins >= ShopItems[btnNo].price)
    {
        coins -= ShopItems[btnNo].price;
        coinUI.text = "Coins: " + coins.ToString();
        CheckPurchase();

        GameObject prefabToSpawn = ShopItems[btnNo].bottle.gameObject;

        if (prefabToSpawn != null && itemSpawnPoints != null && itemSpawnPoints.Length > 0)
        {
            Transform randomSpawn = itemSpawnPoints[Random.Range(0, itemSpawnPoints.Length)];
            GameObject bottleInstance = Instantiate(prefabToSpawn, randomSpawn.position, randomSpawn.rotation);

            // Try to find and configure the BottleInfoDisplay
            BottleInfoDisplay display = bottleInstance.GetComponentInChildren<BottleInfoDisplay>();
            if (display != null)
            {
                display.playerCamera = playerCamera;
                display.infoPanel = infoPanel;
                display.infoText = infoText;
            }

            // Optionally initialize the bottle contents
            Bottle bottle = bottleInstance.GetComponent<Bottle>();
            if (bottle != null)
            {
                MixtureIngredient item = ShopItems[btnNo].item;
                bottle.AddLiquid(item.amount, new List <MixtureIngredient> { item });
            }

            Debug.Log($"Spawned item: {ShopItems[btnNo].bottle.name}");
        }
        else
        {
            Debug.LogWarning("Missing prefab or spawn points.");
        }
    }
}
   public void LoadPanels()
   {
        for(int i = 0; i < ShopItems.Length;i++)
        {
            ShopPanels[i].titleTXT.text = ShopItems[i].item.name;
            ShopPanels[i].image.texture = ShopItems[i].image;
            ShopPanels[i].costTXT.text = "Coins: " + ShopItems[i].price.ToString();
        }
   }
}
