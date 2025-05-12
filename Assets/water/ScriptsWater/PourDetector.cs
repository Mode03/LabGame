using UnityEngine;
using System.Collections;

public class PourDetector : MonoBehaviour
{
    public int pourThreshold = -45;
    public Transform origin = null;
    public GameObject streamPrefab = null;

    private bool isPouring = false;
    private Stream currentStream = null;

    public Bottle bottle;

    private void Start()
    {
        if (bottle == null)
        {
            bottle = GetComponentInParent<Bottle>();
        }
    }

    private void Update()
    {
        bool pourCheck = CalculatePourAngle() < pourThreshold;

        if (isPouring != pourCheck)
        {
            isPouring = pourCheck;

            if (isPouring)
            {
                StartPour();
            }
            else
            {
                EndPour();
            }
        }
    }

    private void StartPour()
    {
        if (bottle == null || bottle.currentVolume <= 0f)
        {
            Debug.Log("No liquid to pour.");
            return;
        }

        Debug.Log("Start");
        currentStream = CreateStream();
        currentStream.Begin();
    }

    private void EndPour()
    {
        Debug.Log("End");
        if (currentStream != null)
        {
            currentStream.End();
            currentStream = null;
        }
        else
        {
            //Debug.LogWarning("EndPour was called, but currentStream is null!");
        }
    }

    private float CalculatePourAngle()
    {
        float angle = transform.eulerAngles.z;
        return (angle > 180) ? angle - 360 : angle;
    }

    private Stream CreateStream()
    {
        GameObject streamObject = Instantiate(streamPrefab, origin.position, Quaternion.identity, transform);
        return streamObject.GetComponent<Stream>();
    }
}