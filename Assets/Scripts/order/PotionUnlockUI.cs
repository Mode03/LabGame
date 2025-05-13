using UnityEngine;
using TMPro;
using System.Collections;

public class PotionUnlockUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup; // vietoj panel
    [SerializeField] private TextMeshProUGUI potionText;
    [SerializeField] private float showDuration = 3f;
    [SerializeField] private float fadeDuration = 0.5f;

    void Awake()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void Show(string potionName, PotionRarity rarity)
    {
        string colorHex = rarity switch
        {
            PotionRarity.Common => "#CCCCCC",
            PotionRarity.Rare => "#4AA6FF",
            PotionRarity.Epic => "#B86BFF",
            PotionRarity.Forbidden => "#FF3B3B",
            _ => "#FFFFFF"
        };

        potionText.text = $"New potion unlocked:\n<color={colorHex}><b>{potionName}</b></color>";
        StopAllCoroutines();
        StartCoroutine(FadeSequence());
    }

    IEnumerator FadeSequence()
    {
        // Fade In
        yield return StartCoroutine(FadeCanvasGroup(0, 1, fadeDuration));
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        yield return new WaitForSeconds(showDuration);

        // Fade Out
        yield return StartCoroutine(FadeCanvasGroup(1, 0, fadeDuration));
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    IEnumerator FadeCanvasGroup(float from, float to, float duration)
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
