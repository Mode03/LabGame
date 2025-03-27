using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinkWaterPouring : MonoBehaviour
{
    public GameObject streamPrefab; // Vandens srauto prefab'as
    public Transform streamOrigin; // Vieta, is kur bega vanduo
    public KeyCode rotateKey = KeyCode.E; // Mygtukas, kuriuo pasuksi rutuliuka

    private bool isPouring = false;
    private GameObject currentStream;

    private void Update()
    {
        // Jei ziuri i rutuliuka ir paspaudi mygtuka, pasukame ji
        if (IsLookingAtKnob() && Input.GetKeyDown(rotateKey))
        {
            ToggleWaterFlow();
        }
    }

    private bool IsLookingAtKnob()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit))
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
