using TMPro;
using UnityEngine;

public class DoorInteractor : MonoBehaviour
{
    public float interactDistance = 3f;
    public LayerMask interactLayer;
    public TMP_Text interactionText;

    private Camera playerCamera;
    private DoorController currentDoor;

    void Start()
    {
        playerCamera = Camera.main;
        interactionText.gameObject.SetActive(false); // isjungia pradzioje
    }

    void Update()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * interactDistance, Color.red);


        if (Physics.Raycast(ray, out hit, interactDistance, interactLayer))
        {
            DoorController door = hit.collider.GetComponent<DoorController>();

            if (door != null)
            {
                currentDoor = door;
                interactionText.text = "Press [Q]";
                interactionText.gameObject.SetActive(true);

                if (Input.GetKeyDown(KeyCode.Q))
                {
                    door.ToggleDoor();
                }

                return; // kad zinute neissijungtu kol ziuri
            }
        }

        // Jei nieko nematome arba duru nere, teskto nerodome
        currentDoor = null;
        interactionText.gameObject.SetActive(false);
        
    }
}
