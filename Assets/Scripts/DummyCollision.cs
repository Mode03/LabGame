using UnityEngine;
using System.Collections;

public class DummyCollision : MonoBehaviour
{
    public GameObject target; // Assign the "Target" GameObject in Inspector

    private Vector3 activePosition = new Vector3(53.716f, 1.6654f, 20.83f);
    private Quaternion activeRotation = Quaternion.Euler(-43.714f, -90.58f, 89.969f);

    private Vector3 inactivePosition = new Vector3(54.00042f, 1.371988f, 22.66025f);
    private Quaternion inactiveRotation = Quaternion.Euler(-24.899f, -195.387f, 194.784f);

    public float moveDuration = 1.0f; // Time to complete movement

    private Coroutine moveCoroutine;

    void Start()
    {
        if (target != null)
        {
            target.SetActive(false);
            target.transform.position = inactivePosition;
            target.transform.rotation = inactiveRotation;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && target != null)
        {
            Debug.Log("ENTER - Moving Target");
            target.SetActive(true);

            if (moveCoroutine != null) StopCoroutine(moveCoroutine);
            moveCoroutine = StartCoroutine(MoveTarget(target, activePosition, activeRotation));
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && target != null)
        {
            Debug.Log("EXIT - Resetting Target");

            if (moveCoroutine != null) StopCoroutine(moveCoroutine);
            moveCoroutine = StartCoroutine(MoveTarget(target, inactivePosition, inactiveRotation));
        }
    }

    IEnumerator MoveTarget(GameObject obj, Vector3 endPos, Quaternion endRot)
    {
        float elapsedTime = 0;
        Vector3 startPos = obj.transform.position;
        Quaternion startRot = obj.transform.rotation;

        while (elapsedTime < moveDuration)
        {
            obj.transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / moveDuration);
            obj.transform.rotation = Quaternion.Slerp(startRot, endRot, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        //dsaaa

        obj.transform.position = endPos;
        obj.transform.rotation = endRot;
    }
}
