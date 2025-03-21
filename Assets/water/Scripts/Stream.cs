using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stream : MonoBehaviour
{
    private LineRenderer lineRenderer = null;
    private ParticleSystem splashParticle = null;

    private Coroutine pourRoutine = null;
    private Vector3 targetPosition = Vector3.zero;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        splashParticle = GetComponentInChildren<ParticleSystem>();
    }

    private void Start()
    {
        MoveToPosition(0, transform.position);
        MoveToPosition(1, transform.position);
    }

    public void Begin()
    {
        StartCoroutine(UpdateParticle());
        pourRoutine = StartCoroutine(BeginPour());
    }

    private IEnumerator BeginPour()
    {
        while (gameObject.activeSelf)
        {
            targetPosition = FindEndPoint();

            MoveToPosition(0, transform.position);
            AnimateToPosition(1, targetPosition);

            yield return null;
        }
    }

    private void Update()
    {
        // Tikriname, ar tevinis objektas (butelis) nebeturi skyscio
        Bottle sourceBottle = GetComponentInParent<Bottle>();
        if (sourceBottle != null && sourceBottle.currentVolume <= 0)
        {
            End();  // Kai tik istusteja, nutraukiam srauta
        }
    }

    public void End()
    {
        StopCoroutine(pourRoutine);
        pourRoutine = StartCoroutine(EndPour());
    }

    private IEnumerator EndPour()
    {
        while (!HasReachedPosition(0, targetPosition))
        {
            AnimateToPosition(0, targetPosition);
            AnimateToPosition(1, targetPosition);

            yield return null;
        }

        Destroy(gameObject);
    }

    private Vector3 FindEndPoint()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, Vector3.down);

        Bottle sourceBottle = GetComponentInParent<Bottle>();

        if (sourceBottle != null)
        {
            // Visada nuleidziam skysti — net jei nieko nepasiekia
            float pourAmount = Time.deltaTime * 10f;
            List<MixtureIngredient> drainedIngredients = sourceBottle.DrainLiquid(pourAmount);

            // Jei pataikom i buteli — perpilam skysti su ingredientais
            if (Physics.Raycast(ray, out hit, 2.0f))
            {
                Bottle targetBottle = hit.collider.GetComponent<Bottle>();

                if (targetBottle != null)
                {
                    targetBottle.AddLiquid(pourAmount, drainedIngredients);
                }

                return hit.point;
            }
        }

        // Jeigu nieko nepasiekia, srove vis tiek bega
        return ray.GetPoint(2.0f);
    }


    private void MoveToPosition(int index, Vector3 targetPosition)
    {
        lineRenderer.SetPosition(index, targetPosition);
    }

    private void AnimateToPosition(int index, Vector3 targetPosition)
    {
        Vector3 currentPoint = lineRenderer.GetPosition(index);
        Vector3 newPosition = Vector3.MoveTowards(currentPoint, targetPosition, Time.deltaTime * 1.75f);
        lineRenderer.SetPosition(index, newPosition);
    }

    private bool HasReachedPosition(int index, Vector3 targetPosition)
    {
        Vector3 currentPosition = lineRenderer.GetPosition(index);
        return currentPosition == targetPosition;
    }

    private IEnumerator UpdateParticle()
    {
        while (gameObject.activeSelf)
        {
            splashParticle.gameObject.transform.position = targetPosition;

            bool isHitting = HasReachedPosition(1, targetPosition);
            splashParticle.gameObject.SetActive(isHitting);

            yield return null;
        }
    }
}
