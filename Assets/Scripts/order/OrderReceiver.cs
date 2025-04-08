using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NUnit.Framework;
using System.Collections.Generic;

public class OrderReceiver : MonoBehaviour
{
    public GameObject orderTextUI;  // „Take Order [E]“
    public TMP_Text orderDisplayUI; // order info
    public Transform playerCamera;  // zaidejo kamera

    private bool isLookingAtOrderPoint = false;
    private string currentOrder;

    void Start()
    {
        orderTextUI.SetActive(false);
    }

    void Update()
    {
        CheckForOrderPoint();

        if (isLookingAtOrderPoint && Input.GetKeyDown(KeyCode.E))
        {
            GenerateOrder();
        }
    }

    private void CheckForOrderPoint()
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

    private void GenerateOrder()
    {
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
        for (int i = 0; i < chosenIngredients.Count;  i++)
        {
            orderText += $"- {chosenIngredients[i]}: {amounts[i]:F0}ml\n";
        }

        currentOrder = orderText;
        orderDisplayUI.text = currentOrder;
    }
}
