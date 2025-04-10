using UnityEngine;

public class SpiceBehavior : MonoBehaviour
{
    public MixtureIngredient ingredient;

    public void Initialize(MixtureIngredient newIngredient)
    {
        ingredient = newIngredient;
        // You can do more setup here, like updating visuals or logic based on the ingredient
    }
}