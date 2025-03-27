using UnityEngine;
using TMPro;

public class BottleInfoDisplay : MonoBehaviour
{
    public Camera playerCamera; // Žaid?jo kamera
    public float maxDistance = 5f; // Maksimalus atstumas iki m?gintuv?lio
    public GameObject infoPanel; // UI panel?
    public TextMeshProUGUI infoText; // UI tekstas

    private void Update()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Tikrinam, ar ži?ri ? objekt? su "Bottle" scriptu
        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            Bottle bottle = hit.collider.GetComponent<Bottle>();

            if (bottle != null)
            {
                // Jei pataikom ? butel? — rodom informacij?
                ShowBottleInfo(bottle);
                return;
            }
        }

        // Jei nieko nerandam — paslepiam lentel?
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

        string info = $"**Megintuvelis:**\n";
        info += $"Kiekis: {bottle.currentVolume}/{bottle.maxVolume} ml\n";

        foreach (var ingredient in bottle.ingredients)
        {
            info += $"<color=#{ColorUtility.ToHtmlStringRGB(ingredient.color)}>{ingredient.name}</color>: {ingredient.amount:F1} ml\n";
        }

        infoText.text = info;

        // Pakei?iam pozicij? šalia ži?rimo objekto
        Vector3 screenPos = playerCamera.WorldToScreenPoint(bottle.transform.position);
        infoPanel.transform.position = new Vector3(screenPos.x + 200, screenPos.y, screenPos.z);
    }
}
