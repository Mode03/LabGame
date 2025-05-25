using UnityEngine;
using TMPro;

public class BottleInfoDisplay : MonoBehaviour
{
    public Camera playerCamera; // �aidejo kamera
    public float maxDistance = 5f; // Maksimalus atstumas iki megintuvelio
    public GameObject infoPanel; // UI panel
    public TextMeshProUGUI infoText; // UI tekstas

    private void Update()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Tikrinam, ar ziuri i objekta su "Bottle" scriptu
        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            Bottle bottle = hit.collider.GetComponent<Bottle>();

            if (bottle != null)
            {
                // Jei pataikom i buteli � rodom info
                ShowBottleInfo(bottle);
                return;
            }
        }

        // Jei nieko nerandam � paslepiam lentele
        infoPanel.SetActive(false);
    }

    //private void ShowBottleInfo(Bottle bottle)
    //{
    //    infoPanel.SetActive(true);

    //    string info = $"**Megintuvelis:**\n";
    //    info += $"Kiekis: {bottle.currentVolume}/{bottle.maxVolume} ml\n";

    //    foreach (var ingredient in bottle.ingredients)
    //    {
    //        // Kiekvienas ingredientas su spalva ir kiekiu
    //        info += $"<color=#{ColorUtility.ToHtmlStringRGB(ingredient.color)}>{ingredient.name}</color>: {ingredient.amount:F1} ml\n";
    //    }

    //    infoText.text = info;
    //}

    private void ShowBottleInfo(Bottle bottle)
    {
        infoPanel.SetActive(true);

        string info = $"**Test-Tube**\n";
        info += $"Amount: {bottle.currentVolume:F2}/{bottle.maxVolume} ml\n";

        foreach (var ingredient in bottle.ingredients)
        {
            info += $"<color=#{ColorUtility.ToHtmlStringRGB(ingredient.color)}>{ingredient.name}</color>: {ingredient.amount:F1} ml\n";
        }

        infoText.text = info;

        // Pakeiciam pozicija salia ziurimo objekto
        Vector3 screenPos = playerCamera.WorldToScreenPoint(bottle.transform.position);
        infoPanel.transform.position = new Vector3(screenPos.x + 200, screenPos.y, screenPos.z);
    }
}
