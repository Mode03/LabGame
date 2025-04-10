using UnityEngine;
using System.Collections;
using UnityEngine.Animations.Rigging;

public class DummyCollision : MonoBehaviour
{
    public GameObject target;
    public Rig headRig;


    public MultiAimConstraint headAimConstraint;
    public MultiAimConstraint moveHeadConstraint;

    public bool playerIsNearby = false;

    private Vector3 extendedArmPosition = new Vector3(53.98188f, 1.9755056f, 21.9888439f);
    private Quaternion extendedArmRotation = new Quaternion(-0.7041214f, 0.02805415f, -0.02546502f, 0.7090681f);

    private Vector3 liftedHandPosition = new Vector3(53.88044f, 3.0984f, 22.05754f);
    private Quaternion liftedHandRotation = new Quaternion(-0.32408f, 0.55321f, 0.10986f, 0.75951f);

    private Vector3 holdPosition = new Vector3(54.47599f, 2.917f, 22.373f);
    private Quaternion holdRotation = new Quaternion(0.61538327f, 0.03000121f, -0.04507899f, 0.7863659f);

    private Vector3 inactivePosition = new Vector3(54.00042f, 1.371988f, 22.66025f);
    private Quaternion inactiveRotation = Quaternion.Euler(-24.899f, -195.387f, 194.784f);

    public float moveDuration = 1.0f;
    public float weightBlendDuration = 1.0f;

    private Coroutine moveCoroutine;
    private Coroutine weightCoroutine;
    private Coroutine resetCoroutine;

    private bool hasGivenItem = false;

    void Start()
    {
        if (target != null)
        {
            target.SetActive(false);
            target.transform.position = inactivePosition;
            target.transform.rotation = inactiveRotation;
        }

        if (headAimConstraint != null) headAimConstraint.weight = 0.5f;
        if (moveHeadConstraint != null) moveHeadConstraint.weight = 0f;
    }

    void Update()
    {
        if (playerIsNearby && !hasGivenItem && Input.GetKeyDown(KeyCode.Q))
        {
            hasGivenItem = true;
            Debug.Log("Q pressed - Giving item (lifting arm)");

            if (moveCoroutine != null) StopCoroutine(moveCoroutine);
            moveCoroutine = StartCoroutine(GiveItemSequence());
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && target != null)
        {
            playerIsNearby = true;
            hasGivenItem = false;

            Debug.Log("ENTER - Extending arm");
            target.SetActive(true);

            if (moveCoroutine != null) StopCoroutine(moveCoroutine);
            moveCoroutine = StartCoroutine(MoveTarget(target, extendedArmPosition, extendedArmRotation));
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && target != null)
        {
            playerIsNearby = false;

            Debug.Log("EXIT - Resetting arm");
            if (moveCoroutine != null) StopCoroutine(moveCoroutine);
            moveCoroutine = StartCoroutine(MoveTarget(target, inactivePosition, inactiveRotation));

            if (weightCoroutine != null) StopCoroutine(weightCoroutine);
            weightCoroutine = StartCoroutine(BlendWeights(0.5f, 0f));
        }
    }

    IEnumerator GiveItemSequence()
    {
        yield return StartCoroutine(MoveTarget(target, liftedHandPosition, liftedHandRotation));
        yield return StartCoroutine(BlendWeights(0.5f, 1f));
        yield return new WaitForSeconds(0.0f);
        yield return StartCoroutine(MoveTarget(target, holdPosition, holdRotation));

        // Automatically reset after 5 seconds
        if (resetCoroutine != null) StopCoroutine(resetCoroutine);
        resetCoroutine = StartCoroutine(ResetAfterDelay(3.5f));
    }

    IEnumerator ResetAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        Debug.Log("Auto-reset after 5 seconds");

        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MoveTarget(target, inactivePosition, inactiveRotation));

        if (weightCoroutine != null) StopCoroutine(weightCoroutine);
        weightCoroutine = StartCoroutine(BlendWeights(0.5f, 0f));

        hasGivenItem = false;
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

        obj.transform.position = endPos;
        obj.transform.rotation = endRot;
    }

    IEnumerator BlendWeights(float headAimTarget, float moveHeadTarget)
    {
        float elapsed = 0f;
        float startHeadAim = headAimConstraint.weight;
        float startMoveHead = moveHeadConstraint.weight;

        while (elapsed < weightBlendDuration)
        {
            float t = elapsed / weightBlendDuration;
            headAimConstraint.weight = Mathf.Lerp(startHeadAim, headAimTarget, t);
            moveHeadConstraint.weight = Mathf.Lerp(startMoveHead, moveHeadTarget, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        headAimConstraint.weight = headAimTarget;
        moveHeadConstraint.weight = moveHeadTarget;
    }
}
