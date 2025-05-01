using UnityEngine;
using TMPro;
public class AnalyzerInteraction : MonoBehaviour
{
    public GameObject analyzerUI;
    public Transform playerCamera;
    public MouseMovement cameraScript;
    public float interactionDistance = 3f;
    public LayerMask analyzerLayer;

    public TMP_Text interactionText;

    private bool isAnalyzerOpen = false;

    void Start()
    {
        analyzerUI.SetActive(false); // Neparodyk is pradziu
        if (interactionText != null)
        {
            interactionText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (isAnalyzerOpen)
        {
            // jei jau atidaryta, paspaudus E – uzdarom
            if (Input.GetKeyDown(KeyCode.E))
            {
                ExitAnalyzer();
            }

            if (interactionText != null)
                interactionText.gameObject.SetActive(false);

            return;
        }

        if (IsLookingAtAnalyzer())
        {
            if (interactionText != null)
            {
                interactionText.text = "Press [E]";
                interactionText.gameObject.SetActive(true);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                ToggleAnalyzer(true);
            }
        }
        else
        {
            if (interactionText != null)
                interactionText.gameObject.SetActive(false);
        }
    }

    private bool IsLookingAtAnalyzer()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, interactionDistance, analyzerLayer))
        {
            Debug.Log("Raycast hit: " + hit.collider.name);
            return true;
        }
        return false;
    }

    private void ToggleAnalyzer(bool open)
    {
        isAnalyzerOpen = open;
        analyzerUI.SetActive(open);

        Cursor.lockState = open ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = open;

        if (cameraScript != null)
        {
            cameraScript.enabled = !open;
        }
    }

    public void ExitAnalyzer()
    {
        if (isAnalyzerOpen)
        {
            ToggleAnalyzer(false);
        }
    }
}
