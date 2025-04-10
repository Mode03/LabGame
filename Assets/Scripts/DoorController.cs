using UnityEngine;

public class DoorController : MonoBehaviour
{
    public Transform leftDoor;
    public Transform rightDoor;
    public Vector3 leftOpenPosition;
    public Vector3 rightOpenPosition;
    public Vector3 leftClosedPosition;
    public Vector3 rightClosedPosition;
    public float doorSpeed = 2f;

    private bool isOpen = false;

    void Update()
    {
        // duru judejimas
        if (isOpen)
        {
            leftDoor.localPosition = Vector3.Lerp(leftDoor.localPosition, leftOpenPosition, Time.deltaTime * doorSpeed);
            rightDoor.localPosition = Vector3.Lerp(rightDoor.localPosition, rightOpenPosition, Time.deltaTime * doorSpeed);
        }
        else
        {
            leftDoor.localPosition = Vector3.Lerp(leftDoor.localPosition, leftClosedPosition, Time.deltaTime * doorSpeed);
            rightDoor.localPosition = Vector3.Lerp(rightDoor.localPosition, rightClosedPosition, Time.deltaTime * doorSpeed);
        }
    }

    public void ToggleDoor()
    {
        isOpen = !isOpen;
    }
}
