using UnityEngine;
using System.Collections;

public class PourDetector : MonoBehaviour
{
    public int pourThreshold = -45;
    public Transform origin = null;
    public GameObject streamPrefab = null;

    private bool isPouring = false;
    private Stream currentStream = null;

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

    public bool IsPouring()
    {
        return isPouring;
    }

    private void StartPour()
    {
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
            Debug.LogWarning("EndPour was called, but currentStream is null!");
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