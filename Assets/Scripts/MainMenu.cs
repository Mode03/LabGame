using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(2);
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void NewGame()
    {
        SaveSystem.ResetSave();

        if (PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.ResetGame();
        }

        SceneManager.LoadSceneAsync(2);
    }
}
