using UnityEngine;
using TMPro;
public class AnalyzerInteraction : MonoBehaviour
{
    public GameObject analyzerUI;
    public Transform playerCamera;
    public MouseMovement cameraScript;
    public float interactionDistance = 3f;
    public LayerMask analyzerLayer;

    public AnalyzerManagerScript analyzerManager;

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
                string message = "Press [E]";

                // Patikrinam ar analize vyksta
                float maxRemaining = 0f;

                for (int i = 0; i < analyzerManager.revealTimers.Length; i++)
                {
                    if (analyzerManager.isBought[i] && Time.time < analyzerManager.revealTimers[i])
                    {
                        float remaining = analyzerManager.revealTimers[i] - Time.time;
                        if (remaining > maxRemaining) maxRemaining = remaining;
                    }
                }

                if (maxRemaining > 0)
                {
                    int minutes = Mathf.FloorToInt(maxRemaining / 60);
                    int seconds = Mathf.FloorToInt(maxRemaining % 60);
                    message += $"\n({minutes:D2}:{seconds:D2} remaining)";
                }

                interactionText.text = message;
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
