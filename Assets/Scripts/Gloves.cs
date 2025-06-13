using UnityEngine;
using TMPro;

public class Gloves : MonoBehaviour
{
    public Transform playerCamera;                // Žaid?jo kamera
    public TMP_Text interactionText;              // UI tekstas
    public KeyCode pickupKey = KeyCode.E;         // Mygtukas pirštin?ms paimti
    public PlayerPickAndDrop player;              // Tavo žaid?jo skriptas, kuris turi PickGloves()

    private void Update()
    {
        if (IsLookingAtGloves())
        {
            if (interactionText != null)
            {
                interactionText.text = "Press [E] to take gloves";
                interactionText.gameObject.SetActive(true);
            }

            if (Input.GetKeyDown(pickupKey))
            {
                PickUpGloves();
            }
        }
        else
        {
            if (interactionText != null)
            {
                interactionText.gameObject.SetActive(false);
            }
        }
    }

    private bool IsLookingAtGloves()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, 3f))
        {
            return hit.collider.gameObject == gameObject;
        }
        return false;
    }

    private void PickUpGloves()
    {
        if (player != null && !player.Gloves)
        {
            player.PickGloves();
            interactionText.gameObject.SetActive(false);
            Destroy(gameObject);
            Debug.Log("Gloves picked up!");
        }
    }
}
