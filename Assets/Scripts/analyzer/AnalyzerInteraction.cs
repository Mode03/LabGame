using UnityEngine;

public class AnalyzerInteraction : MonoBehaviour
{
    public GameObject analyzerUI;
    public Transform playerCamera;
    public MouseMovement cameraScript;
    public float interactionDistance = 3f;
    public LayerMask analyzerLayer;

    private bool isAnalyzerOpen = false;

    void Start()
    {
        analyzerUI.SetActive(false); // Neparodyk is pradziu
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isAnalyzerOpen)
            {
                ExitAnalyzer(); // jei atidarytas, tada uzdaryk
            }
            else if (IsLookingAtAnalyzer())
            {
                ToggleAnalyzer(true); // jei ziuri i analyzer ir paspaudi E, tada atidaryk
                Debug.Log("Analyzer detected and E pressed");
            }
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
