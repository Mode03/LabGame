using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class DummyCollision : MonoBehaviour
{
    public OrderReceiver orderReceiver; // Assign via Inspector or Find it in Start()
    public TwoBoneIKConstraint armIKConstraint; // Assign via Inspector
    public MultiAimConstraint HeadConstraint;



    public GameObject explosionFire;
    public GameObject explosionFire1;
    public GameObject sparkles;
    public GameObject flameThrower;
    public GameObject bombPrefab;


    public SphereCollider sphereCollider; // Assign this in the Inspector or via GetComponent

    public GameObject target;
    public Rig headRig;
    public Animator dummyAnimator; // Assign this via Inspector

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

            if (moveCoroutine != null) StopCoroutine(moveCoroutine);
            moveCoroutine = StartCoroutine(GiveItemSequence());

            // Start fall trigger coroutine
            StartCoroutine(TriggerFallAfterDelay(7f));
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && target != null)
        {
            playerIsNearby = true;
            hasGivenItem = false;

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

        // Automatically reset arm pose after 3.5 seconds
        if (resetCoroutine != null) StopCoroutine(resetCoroutine);
        resetCoroutine = StartCoroutine(ResetAfterDelay(3.5f));
    }

    IEnumerator ResetAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);


        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MoveTarget(target, inactivePosition, inactiveRotation));

        if (weightCoroutine != null) StopCoroutine(weightCoroutine);
        weightCoroutine = StartCoroutine(BlendWeights(0.5f, 0f));

        hasGivenItem = false;
    }

    IEnumerator TriggerFallAfterDelay(float delay)
    {
        StartCoroutine(ExpandColliderAfterDelay(0f));
        yield return new WaitForSeconds(delay);

        float acc = orderReceiver.Accuracy;


        if (dummyAnimator != null)
        {
            if (acc < 0.5f)
            {
                dummyAnimator.applyRootMotion = true;

                int randomDeath = 4;//ndom.Range(0, 3);
                dummyAnimator.SetInteger("DeathType", randomDeath);
                dummyAnimator.SetTrigger("FallTrigger");

                if (randomDeath == 1)
                {
                    StartCoroutine(ResetToStandAfterDelay(10f));
                    explosionFire.GetComponent<ParticleSystem>().Play();
                    yield return new WaitForSeconds(1.5f);
                    Transform headTransform = transform.Find("metarig_male/hips/spine/chest/upper_chest/neck/head");
                    if (headTransform != null)
                    {
                        Vector3 originalScale = headTransform.localScale;
                        headTransform.localScale = Vector3.zero; // hide head
                        yield return new WaitForSeconds(8.5f); // wait 3 seconds
                        headTransform.localScale = originalScale; // show head again

                    }

                }
                else if (randomDeath == 2)
                {
                    // Trigger death1 after 2 seconds, regardless of whether death2 finished
                    StartCoroutine(SwitchToDeath1AfterDelay(2f));
                    StartCoroutine(ResetToStandAfterDelay(10f));
                }
                else if (randomDeath == 0)
                {
                    // Trigger death1 after 2 seconds, regardless of whether death2 finished

                    StartCoroutine(ResetToStandAfterDelay(8f));
                }
                else if (randomDeath == 3)
                {
                    if (armIKConstraint != null)
                    {
                        armIKConstraint.weight = 0f; // Disable IK influence
                        headAimConstraint.weight = 0f;
                    }


                    StartCoroutine(ResetToStandAfterDelay(14f));
                    Invoke(nameof(PlaySparkles), 1.7f);
                }
                else if (randomDeath == 4)
                {
                    StartCoroutine(SwitchToStrong(2f));
                    if (armIKConstraint != null)
                    {
                        armIKConstraint.weight = 0f; // Disable IK influence
                        headAimConstraint.weight = 0f;
                    }
                    Invoke(nameof(PlayFlameThrower), 4.7f);
                    StartCoroutine(ResetToStandAfterDelay(11f));
                    Invoke(nameof(StopFlameThrower), 6f);
                }
                else if (randomDeath == 6)
                {
                    StartCoroutine(SwitchToDeathFront(2f));

                    if (armIKConstraint != null)
                    {
                        armIKConstraint.weight = 0f; // Disable IK influence
                        headAimConstraint.weight = 0f;
                    }
                    if (bombPrefab != null)
                    {
                        Vector3 spawnPosition = new Vector3(54.5340004f, 0.477999985f, 20.9039993f);
                        Quaternion spawnRotation = new Quaternion(-0.6792373f, -0.2061180f, -0.2045378f, 0.6740299f);
                        Vector3 spawnScale = new Vector3(0.0068674237f, 0.0068674237f, 0.0068674237f);

                        GameObject spawnedBomb = Instantiate(bombPrefab, spawnPosition, spawnRotation);
                        spawnedBomb.transform.localScale = spawnScale;
                        StartCoroutine(PlayExplosionAfterDelay(4f));
                        Destroy(spawnedBomb, 6f); // Despawns bomb after 5 seconds
                        
                    }
                    StartCoroutine(ResetToStandAfterDelay(14f));
                }


            }
            else
            {
                string potionName = orderReceiver != null && orderReceiver.currentPotion != null
                    ? orderReceiver.currentPotion.name
                    : "Unknown Potion";

                Debug.Log($"Positive reaction triggered for potion: {potionName} (accuracy >= 50%)");

                // Example: Different reactions based on potion name
                if (potionName == "Goofy Ahh Serum")
                {
                    Debug.Log("LOLOLOLOL");

                }
                else if (potionName == "sds")
                {
                    // Trigger fire burst effect
                }
            }
        }
    }
    IEnumerator SwitchToDeath1AfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        dummyAnimator.SetInteger("DeathType", 1);
        dummyAnimator.SetTrigger("FallTrigger");
    }
    IEnumerator SwitchToStrong(float delay)
    {
        yield return new WaitForSeconds(delay);
        dummyAnimator.SetInteger("DeathType", 5);
        dummyAnimator.SetTrigger("FallTrigger");
    }
    IEnumerator SwitchToDeathFront(float delay)
    {
        yield return new WaitForSeconds(delay);
        dummyAnimator.SetInteger("DeathType", 7);
        dummyAnimator.SetTrigger("FallTrigger");
    }
    IEnumerator PlayExplosionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        explosionFire.GetComponent<ParticleSystem>().Play();
    }

    IEnumerator ResetToStandAfterDelay(float delay)
    {

        yield return new WaitForSeconds(delay);

        sparkles.GetComponent<ParticleSystem>().Stop();

        if (dummyAnimator != null)
        {

            // Prevent animations from overriding position
            dummyAnimator.applyRootMotion = false;

            // Reset to "Stand" state
            dummyAnimator.CrossFade("Stand", 0.2f);
            if (armIKConstraint != null)
                armIKConstraint.weight = 1f;
            headAimConstraint.weight = 0.513f;

        }

        // Reset the dummy's transform manually

        transform.position = new Vector3(54.515f, 0.234f, 22.621f);
        transform.rotation = new Quaternion(0f, -0.9996081f, 0f, 0.02799418f);
        transform.localScale = new Vector3(1.7867f, 1.7867f, 1.7867f);
    }

    IEnumerator ExpandColliderAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (sphereCollider != null)
        {
            sphereCollider.radius = 15f;
            StartCoroutine(ShrinkColliderAfterDelay(22f, 1.5f));

        }
    }
    IEnumerator ShrinkColliderAfterDelay(float delay, float originalRadius)
    {

        yield return new WaitForSeconds(delay);
        Debug.Log("SHRINKKKKKKKKKKKKKKKKKKKKKKKKKKKK");

        if (sphereCollider != null)
        {
            sphereCollider.radius = originalRadius;
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
    private void PlaySparkles()
    {
        sparkles.GetComponent<ParticleSystem>().Play();
    }
    private void PlayFlameThrower()
    {
        flameThrower.GetComponent<ParticleSystem>().Play();
    }
    private void StopFlameThrower()
    {
        flameThrower.GetComponent<ParticleSystem>().Stop();
    }
}