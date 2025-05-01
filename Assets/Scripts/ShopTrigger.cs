using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ShopTrigger : MonoBehaviour
{
    public GameObject shopUI; // The shop canvas
    public Transform playerCamera; // The player's camera (for raycasting)
    public MonoBehaviour cameraScript; // Reference to the player's camera movement script
    public float interactionDistance = 3f; // How close the player needs to be
    public LayerMask shopLayer; // Layer for the PC object
    public Button exitButton; // Reference to the exit button
    public TMP_Text interactText;
    public bool isShopOpen = false;

    private void Start()
    {
        shopUI.SetActive(false); // Ensure shop is hidden at the start
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(ExitShop); // Assign exit button function
        }
        Debug.Log("Shop system initialized. Shop is closed.");
    }

    private void Update()
{
    if (IsLookingAtShop())
    {
        interactText.gameObject.SetActive(true);
        interactText.text = "Press [E]";

        if (Input.GetKeyDown(KeyCode.E))
        {
            isShopOpen = true;
            ToggleShop(true);
        }
    }
    else
    {
        interactText.gameObject.SetActive(false);
    }
}

    private bool IsLookingAtShop()
{
    RaycastHit hit;
    if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, interactionDistance, shopLayer))
    {
        Debug.Log("Looking at shop object: " + hit.collider.gameObject.name);
        return true;
    }
    return false;
}

    private void ToggleShop(bool open)
    {
        isShopOpen = open;
        shopUI.SetActive(open);

        Cursor.lockState = open ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = open;

        // Freeze/unfreeze camera movement
        if (cameraScript != null)
        {
            cameraScript.enabled = !open;
        }

        Debug.Log(open ? "Shop opened." : "Shop closed.");
    }

    // Exit shop when button is clicked
    public void ExitShop()
    {
        if(isShopOpen)
        {
            ToggleShop(false);
        }
    }
}