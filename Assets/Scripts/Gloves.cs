using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Gloves : MonoBehaviour
{
    public Transform playerCamera; // The player's camera (for raycasting)
    public MonoBehaviour cameraScript; // Reference to the player's camera movement script
    public float interactionDistance = 3f; // How close the player needs to be
    public TMP_Text interactText;
    public LayerMask shopLayer;
    public PlayerPickAndDrop player;
    private void Update()
    {
            if (IsLookingAtGloves())
            {
                interactText.gameObject.SetActive(true);
                interactText.text = "Press [E]";

                if (Input.GetKeyDown(KeyCode.E))
                {
                OnTriggerEnter();
                }
            }
            else
            {
                interactText.gameObject.SetActive(false);
            }
    }
    private bool IsLookingAtGloves()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, interactionDistance, shopLayer))
        {
            Debug.Log("Looking at shop object: " + hit.collider.gameObject.name);
            return true;
        }
        return false;
    }
    private void OnTriggerEnter()
    {
        if (player != null && !player.Gloves)
        {
            player.PickGloves();
            Destroy(gameObject);
            Debug.Log("Gloves picked up via 3D trigger!");
        }
    }
}
