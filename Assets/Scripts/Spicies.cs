using UnityEngine;
using UnityEngine.UI;
[CreateAssetMenu(fileName = "Spicies", menuName = "Spicies/newspice")]
public class Spicies : ScriptableObject
{
   public Texture2D image;
   public int price;
   public MixtureIngredient item;
}
