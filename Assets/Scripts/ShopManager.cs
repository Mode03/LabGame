using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
   public TMP_Text CoinsTxt; 
   public Spicies[] ShopItems;
   public GameObject[] ShopPanelsSO;
   public ShopTemplate[] ShopPanels;
   public Button[] BuyButton;
   public ShopTrigger shopTrigger; // Reference to ShopTrigger
   public Transform[] itemSpawnPoints;
    public Camera playerCamera;
    public GameObject infoPanel;
    public TextMeshProUGUI infoText;
    public PlayerDataManager player;
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
        CoinsTxt.text = "Coins: " + player.GetCurrency().ToString();
        LoadPanels();
        CheckPurchase();
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
   public void AddCoins()
   {
    player.AddCurrency(50);
    CoinsTxt.text = "Coins: " + player.GetCurrency().ToString();
    CheckPurchase();
   }
   public void CheckPurchase()
   {
    for(int i = 0; i < ShopItems.Length;i++)
    {
        if(player.GetCurrency() >= ShopItems[i].price)
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
    if (player.GetCurrency() >= ShopItems[btnNo].price)
    {
        player.SubtractCurrency(ShopItems[btnNo].price);
        CoinsTxt.text = "Coins: " + player.GetCurrency().ToString();
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
                bottle.AddLiquid(item.amount, new List <MixtureIngredient> {item});
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
