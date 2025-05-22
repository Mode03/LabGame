using UnityEngine;
using TMPro;
using System.Collections;

public class FeedbackUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private TextMeshProUGUI accuracyText;
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI xpText;
    [SerializeField] private float showDuration = 3f;
    [SerializeField] private float fadeDuration = 0.5f;

    void Awake()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void Show(float accuracy, int coins, int xp, string feedback)
    {
        feedbackText.text = feedback;
        feedbackText.color = GetColorByAccuracy(accuracy);

        accuracyText.text = $"Accuracy: {(accuracy * 100f):F0}%";
        coinsText.text = $"Coins: {coins}";
        xpText.text = $"XP: {xp}";

        StopAllCoroutines();
        StartCoroutine(FadeSequence());
    }

    private Color GetColorByAccuracy(float accuracy)
    {
        if (accuracy <= 0.5f)
        {
            // Raudona - Geltona (0.0 – 0.5)
            return Color.Lerp(Color.red, Color.yellow, accuracy / 0.5f);
        }
        else
        {
            // Geltona - zalia (0.5 – 1.0)
            return Color.Lerp(Color.yellow, Color.green, (accuracy - 0.5f) / 0.5f);
        }
    }


    private IEnumerator FadeSequence()
    {
        yield return StartCoroutine(FadeCanvasGroup(0f, 1f, fadeDuration));
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        yield return new WaitForSeconds(showDuration);

        yield return StartCoroutine(FadeCanvasGroup(1f, 0f, fadeDuration));
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    private IEnumerator FadeCanvasGroup(float from, float to, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(from, to, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = to;
    }
}
