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


    public OrderNPC npcMovement;

    void Start()
    {
        orderTextUI.SetActive(false);
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

        if (Input.GetKeyDown(KeyCode.Q))
        {
            CheckHeldBottle();
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

        string[] ingredients = { "H2O", "skibidi", "toilet", "sigma", "boy", "chad" };
        int ingredientCount = Random.Range(2, 6); // 2 iki 5 ingridientu

        List<string> chosenIngredients = new List<string>();
        List<float> amounts = new List<float>();
        //float totalAmount = 0f;
        float maxTotal = 100f;

        // Parenkame unikalius ingridientus
        List<string> availableIngredients = new List<string>(ingredients);
        for (int i = 0; i < ingredientCount && availableIngredients.Count > 0; i++)
        {
            int index = Random.Range(0, availableIngredients.Count);
            string selected = availableIngredients[index];
            availableIngredients.RemoveAt(index);
            chosenIngredients.Add(selected);
        }

        // Parenkam kiekius
        float remainingAmount = maxTotal;

        for (int i = 0; i < chosenIngredients.Count; i++)
        {
            int remainingIngredients = chosenIngredients.Count - i;
            float maxForThis = remainingAmount - (remainingIngredients - 1) * 10f; // min 10ml kiekvienam likusiam
            float amount = Random.Range(10f, Mathf.Min(40f, maxForThis)); // maxForThis ribojam iki 40ml
            amount = Mathf.Clamp(amount, 10f, maxForThis);
            amounts.Add(amount);
            remainingAmount -= amount;
        }

        // Surenkam teksta
        string orderText = "Order:\n";
        for (int i = 0; i < chosenIngredients.Count; i++)
        {
            orderText += $"- {chosenIngredients[i]}: {amounts[i]:F0}ml\n";
            currentOrderData.Add(chosenIngredients[i], amounts[i]);
        }

        currentOrder = orderText;
        orderDisplayUI.text = currentOrder;
    }

    public void CheckHeldBottle()
    {
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
