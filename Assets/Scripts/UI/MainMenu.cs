using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Gameobject References")]
    public GameObject mainMenu;
    public GameObject settingsMenu;
    public TMP_Text beginText;

    // Inputs
    private InputAction escapeAction;

    void Start()
    {
        settingsMenu.SetActive(false);

        if (!PlayerPrefs.HasKey("GraphicsQuality"))
        {
            PlayerPrefs.SetInt("GraphicsQuality", 2);
        }

        if (!PlayerPrefs.HasKey("MasterVolume"))
        {
            PlayerPrefs.SetInt("MasterVolume", 100);
        }

        if (!PlayerPrefs.HasKey("MouseSensitivity"))
        {
            PlayerPrefs.SetInt("MouseSensitivity", 30);
        }

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
        }
    }
    
    public void Begin()
    {
        beginText.text = "Загрузка...";

        SceneManager.LoadScene("Level");
    }

    public void Settings()
    {
        settingsMenu.SetActive(true);

        mainMenu.SetActive(false);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
