using System.Collections;
using UnityEngine;

public class PlayerPickAndDrop : MonoBehaviour
{
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private Transform objectGrabPointTransform;
    [SerializeField] private LayerMask pickUpLayerMask;
    [SerializeField] private DummyCollision dummyCollision;
    [SerializeField] private OrderReceiver orderReceiver;
    [SerializeField] private Transform dummyHandTransform; // Assign this in Inspector

    private bool beakerAttachedToDummyHand = false;
    private Transform beakerTransform; // Track reference to move in LateUpdate
    private ObjectGrabbable objectGrabbable;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (objectGrabbable == null)
            {
                // Try to pick up
                float pickupDistance = 2f;
                if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit raycastHit, pickupDistance, pickUpLayerMask))
                {
                    if (raycastHit.transform.TryGetComponent(out objectGrabbable))
                    {
                        objectGrabbable.Grab(objectGrabPointTransform);
                    }
                }
            }
            else
            {
                // Drop the object
                objectGrabbable.Drop();
                objectGrabbable = null;
            }
        }

        // Give to dummy
        if (Input.GetKeyDown(KeyCode.Q) && dummyCollision != null && dummyCollision.playerIsNearby)
        {
            if (objectGrabbable != null && dummyHandTransform != null)
            {
                orderReceiver.CheckHeldBottle();
                objectGrabbable.Drop();

                StartCoroutine(GiveToDummyAfterDrop(objectGrabbable));
                objectGrabbable = null;
            }
        }
    }

    private IEnumerator GiveToDummyAfterDrop(ObjectGrabbable droppedObject)
    {
        yield return new WaitForEndOfFrame();

        Rigidbody rb = droppedObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        droppedObject.transform.SetParent(dummyHandTransform, worldPositionStays: false);

        //  Reset scale to 1 so it doesn't inherit weird scaling
        droppedObject.transform.localScale = Vector3.one;

        beakerTransform = droppedObject.transform;
        beakerAttachedToDummyHand = true;
    }

    private void LateUpdate()
    {
        // Lock the beaker to the dummy hand *after* animations have moved it
        if (beakerAttachedToDummyHand && beakerTransform != null)
        {
            // Adjust these values until the beaker sits nicely in the palm
            beakerTransform.localPosition = new Vector3(0.03f, 0.08f, -0.12f);
            beakerTransform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        }
    }
}
