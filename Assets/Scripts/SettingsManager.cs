using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject playerUI;
    [SerializeField] private MouseMovement cameraScript;
    [SerializeField] private PlayerMovement playerMovementScript;

    private bool isOpen = false;

    private void Start()
    {
        settingsPanel.SetActive(false);
        isOpen = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (PlayerPrefs.HasKey("musicVolume"))
        {
            LoadVolume();
        }
        else
        {
            SetMusicVolume();
            SetSFXVolume();
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Debug.Log("Escape pressed");


            if (isOpen)
                CloseSettings();
            else
                OpenSettings();
        }
    }

    public void OpenSettings()
    {
        Debug.Log("Opening settings menu");

        isOpen = true;
        settingsPanel.SetActive(true);
        if(playerUI != null) playerUI.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (cameraScript != null)
        {
            cameraScript.enabled = false; // sustabdo mouse movement
        }

        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = false;
        }
    }

    public void CloseSettings()
    {
        isOpen = false;
        settingsPanel.SetActive(false);
        if (playerUI != null) playerUI.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (cameraScript != null)
        {
            cameraScript.enabled = true; // ijungia vel kamera
        }

        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = true;
        }
    }

    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        audioMixer.SetFloat("music", Mathf.Log10(volume)*20);
        PlayerPrefs.SetFloat("musicVolume", volume);
    }

    public void SetSFXVolume()
    {
        float volume = sfxSlider.value;
        audioMixer.SetFloat("sfx", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("sfxVolume", volume);
    }

    private void LoadVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume");

        SetMusicVolume();
        SetSFXVolume();
    }
}
