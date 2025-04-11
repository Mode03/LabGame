using System.Collections;
using UnityEngine;

public class PlayerPickAndDrop : MonoBehaviour
{
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private Transform objectGrabPointTransform;
    [SerializeField] private LayerMask pickUpLayerMask;
    [SerializeField] private DummyCollision dummyCollision;
    [SerializeField] private OrderReceiver orderReceiver;
    [SerializeField] private Transform dummyHandTransform;

    private bool beakerAttachedToDummyHand = false;
    private Transform beakerTransform;
    private ObjectGrabbable objectGrabbable;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (objectGrabbable == null)
            {
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
                objectGrabbable.Drop();
                objectGrabbable = null;
            }
        }

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
        droppedObject.transform.localScale = Vector3.one;

        beakerTransform = droppedObject.transform;
        beakerAttachedToDummyHand = true;

        StartCoroutine(DetachFromDummyHandAfterDelay(beakerTransform, 7f));
    }

    private IEnumerator DetachFromDummyHandAfterDelay(Transform objectToDetach, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (objectToDetach != null && objectToDetach.parent == dummyHandTransform)
        {
            objectToDetach.SetParent(null);

            Rigidbody rb = objectToDetach.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }

            beakerAttachedToDummyHand = false;
            beakerTransform = null;
        }
    }

    private void LateUpdate()
    {
        if (beakerAttachedToDummyHand && beakerTransform != null)
        {
            beakerTransform.localPosition = new Vector3(0.03f, 0.08f, -0.12f);
            beakerTransform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        }
    }
}
