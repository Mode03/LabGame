using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
   public int coins;
   public TMP_Text coinUI; 
   public Spicies[] ShopItems;
   public GameObject[] ShopPanelsSO;
   public ShopTemplate[] ShopPanels;
   public Button[] BuyButton;
   public ShopTrigger shopTrigger; // Reference to ShopTrigger

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
        if(coins >= ShopItems[btnNo].price)
        {
            coins = coins - ShopItems[btnNo].price;
             coinUI.text = "Coins: " + coins.ToString();
             CheckPurchase();
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
