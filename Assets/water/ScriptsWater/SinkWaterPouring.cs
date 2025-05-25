using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SinkWaterPouring : MonoBehaviour
{
    public GameObject streamPrefab; // Vandens srauto prefab'as
    public Transform streamOrigin; // Vieta, is kur bega vanduo
    public KeyCode rotateKey = KeyCode.E; // Mygtukas, kuriuo pasuksi rutuliuka

    public Transform playerCamera;
    public TMP_Text interactionText;

    private bool isPouring = false;
    private GameObject currentStream;

    private void Update()
    {
        if (IsLookingAtKnob())
        {
            if (interactionText != null)
            {
                interactionText.text = "Press [E]";
                interactionText.gameObject.SetActive(true);
            }

            if (Input.GetKeyDown(rotateKey))
            {
                ToggleWaterFlow();
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

    private bool IsLookingAtKnob()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, 3f))
        {
            return hit.collider.gameObject == gameObject;
        }
        return false;
    }

    private void ToggleWaterFlow()
    {
        if (isPouring)
        {
            StopWaterFlow();
        }
        else
        {
            StartWaterFlow();
        }
    }

    private void StartWaterFlow()
    {
        isPouring = true;
        transform.Rotate(Vector3.right, 45f); // pasukam krano rankena

        // sukuriam vandens srauta
        currentStream = Instantiate(streamPrefab, streamOrigin.position, Quaternion.identity, transform);
        currentStream.GetComponent<Stream>().Begin();
    }

    private void StopWaterFlow()
    {
        isPouring = false;
        transform.Rotate(Vector3.right, -45f); // pasukam krano rankena atgal

        if (currentStream != null)
        {
            currentStream.GetComponent<Stream>().End();
            Destroy(currentStream);
        }
    }
}
