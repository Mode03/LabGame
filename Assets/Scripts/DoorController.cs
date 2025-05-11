using UnityEngine;
using System.Collections;

public class DoorController : MonoBehaviour
{
    public Transform leftDoor;
    public Transform rightDoor;
    public Vector3 leftOpenPosition;
    public Vector3 rightOpenPosition;
    public Vector3 leftClosedPosition;
    public Vector3 rightClosedPosition;

    private bool isOpen = false;

    public void ToggleDoor()
    {
        isOpen = !isOpen;
        StopAllCoroutines();
        StartCoroutine(MoveDoor(isOpen));

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.doorClip);
        }
    }

    private IEnumerator MoveDoor(bool open)
    {
        float duration = 1.5f; // sekundziu kiek truks judejimas
        float elapsed = 0f;

        Vector3 leftStart = leftDoor.localPosition;
        Vector3 leftTarget = open ? leftOpenPosition : leftClosedPosition;

        Vector3 rightStart = rightDoor.localPosition;
        Vector3 rightTarget = open ? rightOpenPosition : rightClosedPosition;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            leftDoor.localPosition = Vector3.Lerp(leftStart, leftTarget, t);
            rightDoor.localPosition = Vector3.Lerp(rightStart, rightTarget, t);
            yield return null;
        }

        // uzfiksuoti tikslia pozicija pabaigoje
        leftDoor.localPosition = leftTarget;
        rightDoor.localPosition = rightTarget;
    }

}
