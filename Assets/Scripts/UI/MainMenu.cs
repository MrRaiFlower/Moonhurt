using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Gameobject References")]
    public GameObject mainMenu;
    public GameObject settingsMenu;
    public GameObject controlsMenu;
    public GameObject loadingOverlay;
    public TMP_Text beginText;
    public GameObject mike;
    public GameObject dirtOverlay;

    [Header("Audio Sources")]
    public AudioSource pressSound;

    [Header("Settings")]
    public float pressSoundVolume;

    // Inputs
    private InputAction escapeAction;

    void Start()
    {
        settingsMenu.SetActive(false);
        controlsMenu.SetActive(false);
        loadingOverlay.SetActive(false);

        pressSound.volume = pressSoundVolume;

        if (Time.timeScale == 0f)
        {
            Time.timeScale = 1f;
        }
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (!PlayerPrefs.HasKey("GraphicsQuality"))
        {
            PlayerPrefs.SetInt("GraphicsQuality", 2);
            PlayerPrefs.Save();
        }

        if (!PlayerPrefs.HasKey("MasterVolume"))
        {
            PlayerPrefs.SetInt("MasterVolume", 100);
            PlayerPrefs.Save();
        }

        if (!PlayerPrefs.HasKey("MouseSensitivity"))
        {
            PlayerPrefs.SetInt("MouseSensitivity", 30);
            PlayerPrefs.Save();
        }

        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("GraphicsQuality"));
        AudioListener.volume = PlayerPrefs.GetInt("MasterVolume") / 100f;

        escapeAction = InputSystem.actions.FindAction("Escape");
    }

    void Update()
    {
        if (escapeAction.WasPressedThisFrame())
        {
            if (settingsMenu.activeSelf)
            {
                mainMenu.SetActive(true);
                settingsMenu.SetActive(false);
            }

            if (controlsMenu.activeSelf)
            {
                mainMenu.SetActive(true);
                controlsMenu.SetActive(false);
            }
        }

        if (mike.activeSelf != mainMenu.activeSelf)
        {
            mike.SetActive(mainMenu.activeSelf);
        }
    }

    public void Begin()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        ButtonPress();

        loadingOverlay.SetActive(true);

        mainMenu.SetActive(false);

        SceneManager.LoadScene("Level");
    }

    public void Settings()
    {
        ButtonPress();

        settingsMenu.SetActive(true);

        mainMenu.SetActive(false);
    }

    public void Controls()
    {
        ButtonPress();

        controlsMenu.SetActive(true);

        mainMenu.SetActive(false);
    }

    public void Exit()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        ButtonPress();

        Application.Quit();
    }

    public void ButtonPress()
    {
        pressSound.Play();
    }
}
