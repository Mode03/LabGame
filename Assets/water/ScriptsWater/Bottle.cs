using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class MixtureIngredient
{
    public string name;
    public Color color;
    public float amount;

    public MixtureIngredient(string name, Color color, float amount)
    {
        this.name = name;
        this.color = color;
        this.amount = amount;
    }
}

public class Bottle : MonoBehaviour
{
    public float maxVolume = 100f;
    public float currentVolume = 30f;

    public List<MixtureIngredient> ingredients = new List<MixtureIngredient>();

    private Material liquidMaterial;

    private void Start()
    {
        // Ieskome child'o, kuris turi Renderer component
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in renderers)
        {
            if (rend.material.HasProperty("_Fill"))
            {
                liquidMaterial = rend.material;
                break;
            }
        }

        if (liquidMaterial == null)
        {
            Debug.LogWarning("Liquid material with _Fill property not found in child objects!");
        }

        // pradiniai ingredientai
        ingredients.Add(new MixtureIngredient("H2O", Color.blue, currentVolume));
        UpdateLiquidAppearance();
    }

    private void UpdateLiquidAppearance()
    {
        // Atnaujina skyscio lygi
        float fillAmount = Mathf.Lerp(-0.03f, 0.25f, currentVolume / maxVolume);
        liquidMaterial.SetFloat("_Fill", fillAmount);

        // Apskaiciuoja misinio spalva pagal visus ingredientus
        Color finalColor = CalculateMixtureColor();
        liquidMaterial.SetColor("_LiquidColor", finalColor);
        liquidMaterial.SetColor("_Surface_color", finalColor * 1.2f);
        liquidMaterial.SetColor("_FresnelColor", finalColor * 0.8f);
    }

    private Color CalculateMixtureColor()
    {
        if (ingredients.Count == 0) return Color.clear;

        Color finalColor = Color.black;
        float totalAmount = 0f;

        foreach (var ingredient in ingredients)
        {
            finalColor += ingredient.color * ingredient.amount;
            totalAmount += ingredient.amount;
        }

        if (totalAmount > 0)
        {
            finalColor /= totalAmount;
        }

        return finalColor;
    }

    public void AddLiquid(float amount, List<MixtureIngredient> newIngredients)
    {
        // Patikrinam, ar jau pasiektas maksimalus turis
        if (currentVolume >= maxVolume)
        {
            Debug.Log("Bottle is full! Cannot add more liquid.");
            return; // Iseinam is metodo ir nieko nepridedam
        }

        // Apskaiciuojam, kiek is tiesu galim dar prideti
        float spaceLeft = maxVolume - currentVolume;
        float actualAmount = Mathf.Min(amount, spaceLeft);

        // Atnaujinam dabartini turio kieki
        currentVolume += actualAmount;

        // Pridedam ingredientus tik tiek, kiek leidzia likes turis
        foreach (var newIng in newIngredients)
        {
            var existingIng = ingredients.Find(i => i.name == newIng.name);
            if (existingIng != null)
            {
                existingIng.amount += (newIng.amount * (actualAmount / amount));
            }
            else
            {
                ingredients.Add(new MixtureIngredient(newIng.name, newIng.color, newIng.amount * (actualAmount / amount)));
            }
        }

        // Atnaujinam skyscio vaizda
        UpdateLiquidAppearance();
    }

    public List<MixtureIngredient> DrainLiquid(float amount)
    {
        Debug.Log("Removing " + amount);

        // Uztikrinam, kad nenuimtu daugiau, nei yra
        amount = Mathf.Clamp(amount, 0, currentVolume);
        currentVolume -= amount;
        UpdateLiquidAppearance();

        // Jei skyscio nebera — grazinam tuscia saras?
        if (currentVolume <= 0)
        {
            currentVolume = 0;
            ingredients.Clear();
            return new List<MixtureIngredient>();
        }

        // Apskaiciuojam, kiek ingredientu ispilam proporcingai
        List<MixtureIngredient> drainedIngredients = new List<MixtureIngredient>();
        float totalAmount = 0f;

        foreach (var ing in ingredients)
        {
            float drainedAmount = (ing.amount / (currentVolume + amount)) * amount;
            ing.amount -= drainedAmount;
            totalAmount += ing.amount;
            drainedIngredients.Add(new MixtureIngredient(ing.name, ing.color, drainedAmount));
        }

        // Jei kazkur liko neigiamu ar per mazu likuciu — pataisom
        ingredients.RemoveAll(i => i.amount <= 0);

        // Saugiklis nuo juodos spalvos — jei nera ingredientu, padarom skaidru
        if (ingredients.Count == 0 || totalAmount <= 0)
        {
            ingredients.Clear();
            liquidMaterial.SetColor("_LiquidColor", Color.clear);
        }

        return drainedIngredients;
    }

}