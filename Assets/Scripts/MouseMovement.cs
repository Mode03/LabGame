using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    private float originalMouseSensitivity; // store the initial 

    float xRotation = 0f;
    float YRotation = 0f;

    void Start()
    {
        //locking the cursor to the middle of the screen and making it invisible
        Cursor.lockState = CursorLockMode.Locked;
        originalMouseSensitivity = mouseSensitivity; // save the initial sensitivity
    }

    void Update()
    {
        // if "R" is held, sensitivity is 0 to prevent camera movement
        if (Input.GetKey(KeyCode.R))
        {
            mouseSensitivity = 0f;
        }
        else
        {
            mouseSensitivity = originalMouseSensitivity;
        }

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        //control rotation around x axis (Look up and down)
        xRotation -= mouseY;

        //we clamp the rotation so we cant Over-rotate (like in real life)
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //control rotation around y axis (Look up and down)
        YRotation += mouseX;

        //applying both rotations
        transform.localRotation = Quaternion.Euler(xRotation, YRotation, 0f);

    }
}
