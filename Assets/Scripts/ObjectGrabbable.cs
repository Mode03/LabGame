using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGrabbable : MonoBehaviour
{
    private Rigidbody objectRigidbody;
    private Transform objectGrabPointTransform;

    public float rotationSensitivity = 2f; // rotation speed

    private float horizontalRotationAngle = 0f; // Rotation for Mouse X
    private float scrollRotationAngle = 0f; // Rotation for ScrollWheel

    private float minRotationAngleZ = -110f; // Maximum left rotation
    private float maxRotationAngleZ= 0f; // Neutral position

    private float minRotationAngleY = -180f; // Maximum left rotation
    private float maxRotationAngleY = 180f; // Neutral position

    private void Awake()
    {
        objectRigidbody = GetComponent<Rigidbody>();
        objectRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    public void Grab(Transform objectGrabPointTransform)
    {
        if (objectGrabPointTransform != null)
        {
            this.objectGrabPointTransform = objectGrabPointTransform;
            objectRigidbody.useGravity = false;
            //objectRigidbody.isKinematic = true;

            // Gauti objekto dabartinę rotaciją Euler kampais
            Vector3 currentRotation = transform.eulerAngles;

            // Išspausdinti kampus į konsolę
            Debug.Log("Object Rotation - X: " + currentRotation.x + ", Y: " + currentRotation.y + ", Z: " + currentRotation.z);
        }
    }

    public void Drop()
    {
        this.objectGrabPointTransform = null;
        objectRigidbody.useGravity = true;
        //objectRigidbody.isKinematic = false;
    }

    private void RotateObject()
    {
        if (Input.GetKey(KeyCode.R))
        {
            // Gauti objekto dabartinę rotaciją Euler kampais 
            Vector3 currentRotation = transform.eulerAngles;

            // Get horizontal mouse movement
            float YaxisRotation = Input.GetAxis("Mouse X") * rotationSensitivity;

            // Update rotation angle for horizontal movement
            horizontalRotationAngle += YaxisRotation;
            horizontalRotationAngle = Mathf.Clamp(horizontalRotationAngle, minRotationAngleY, maxRotationAngleY);

            // Get ScrollWheel input
            float scrollInput = Input.GetAxis("Mouse ScrollWheel") * rotationSensitivity * 10f;

            if (Mathf.Abs(scrollInput) > 0.01f) // Prevent tiny movements
            {
                // Update rotation angle for scroll movement
                scrollRotationAngle += scrollInput;
                scrollRotationAngle = Mathf.Clamp(scrollRotationAngle, minRotationAngleZ, maxRotationAngleZ);
            }

            // for smooth rotation
            float smoothedY = Mathf.LerpAngle(currentRotation.y, horizontalRotationAngle, Time.deltaTime * 5f);
            float smoothedZ = Mathf.LerpAngle(currentRotation.z, scrollRotationAngle, Time.deltaTime * 5f);

            // Combine both rotations
            Quaternion finalRotation = Quaternion.Euler(0, smoothedY, smoothedZ);
            objectRigidbody.MoveRotation(finalRotation);
        }
    }

    private void FixedUpdate()
    {
        if (objectGrabPointTransform != null)
        {
            float lerpSpeed = 10f;
            Vector3 newPosition = Vector3.Lerp(transform.position, objectGrabPointTransform.position, Time.deltaTime * lerpSpeed);
            objectRigidbody.MovePosition(newPosition);

            RotateObject();
        }
    }
}
