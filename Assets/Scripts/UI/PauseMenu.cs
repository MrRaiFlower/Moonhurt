using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Gameobject References")]
    public GameObject pauseMenu;
    public GameObject settingsMenu;
    public GameObject controlsMenu;
    public GameObject loadingOverlay;
    public GameObject tint;
    public GameObject enemy;

    [Header("Audio Sources")]
    public AudioSource pressSound;

    [Header("Settings")]
    public float pressSoundVolume;
    public float pauseMenuTintAlpha;
    public float pauseMenuTintAlphaChangeSpeed;
    public float pauseMenuMasterVolumeChangeFactor;
    public float pauseMenuMasterVolumeChangeFactorChangeSpeed;

    [Header("Misc")]
    public float valueCuttingTreshold;

    // Inputs
    private InputAction escapeAction;

    void Start()
    {
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(false);
        controlsMenu.SetActive(false);
        loadingOverlay.SetActive(false);

        pressSound.volume = pressSoundVolume;

        tint.GetComponent<UnityEngine.UI.Image>().color = new Color(0f, 0f, 0f, 0f);

        escapeAction = InputSystem.actions.FindAction("Escape");
    }

    void Update()
    {
        if (escapeAction.WasPressedThisFrame() && !enemy.GetComponent<Enemy>().coughtPlayer)
        {
            if (!pauseMenu.activeSelf && !settingsMenu.activeSelf && !controlsMenu.activeSelf && !loadingOverlay.activeSelf)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                Time.timeScale = 0f;

                pauseMenu.SetActive(true);
                tint.SetActive(true);
            }
            else if (!pauseMenu.activeSelf && settingsMenu.activeSelf)
            {
                pauseMenu.SetActive(true);
                settingsMenu.SetActive(false);
            }
            else if (!pauseMenu.activeSelf && controlsMenu.activeSelf)
            {
                pauseMenu.SetActive(true);
                controlsMenu.SetActive(false);
            }
            else if (pauseMenu.activeSelf && !settingsMenu.activeSelf && !controlsMenu.activeSelf && !loadingOverlay.activeSelf)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                Time.timeScale = 1f;

                pauseMenu.SetActive(false);
            }
        }

        pressSound.volume = pressSoundVolume * Mathf.Pow(AudioListener.volume, -1f);

        if ((pauseMenu.activeSelf || settingsMenu.activeSelf || controlsMenu.activeSelf || loadingOverlay.activeSelf) && AudioListener.volume != PlayerPrefs.GetInt("MasterVolume") / 100f * pauseMenuMasterVolumeChangeFactor)
            {
                AudioListener.volume = Mathf.Lerp(AudioListener.volume, PlayerPrefs.GetInt("MasterVolume") / 100f * pauseMenuMasterVolumeChangeFactor, Time.unscaledDeltaTime * pauseMenuMasterVolumeChangeFactorChangeSpeed);

                if (AudioListener.volume < PlayerPrefs.GetInt("MasterVolume") / 100f * pauseMenuMasterVolumeChangeFactor + valueCuttingTreshold)
                {
                    AudioListener.volume = PlayerPrefs.GetInt("MasterVolume") / 100f * pauseMenuMasterVolumeChangeFactor;
                }
            }

        if (!pauseMenu.activeSelf && !settingsMenu.activeSelf && !controlsMenu.activeSelf && !loadingOverlay.activeSelf && AudioListener.volume != PlayerPrefs.GetInt("MasterVolume") / 100f)
        {
            AudioListener.volume = Mathf.Lerp(AudioListener.volume, PlayerPrefs.GetInt("MasterVolume") / 100f, Time.unscaledDeltaTime * pauseMenuMasterVolumeChangeFactorChangeSpeed);

            if (AudioListener.volume > PlayerPrefs.GetInt("MasterVolume") / 100f - valueCuttingTreshold)
            {
                AudioListener.volume = PlayerPrefs.GetInt("MasterVolume") / 100f;
            }
        }

        if ((pauseMenu.activeSelf || settingsMenu.activeSelf || controlsMenu.activeSelf || loadingOverlay.activeSelf) && tint.GetComponent<UnityEngine.UI.Image>().color.a != pauseMenuTintAlpha)
        {
            tint.GetComponent<UnityEngine.UI.Image>().color = new Color(0f, 0f, 0f, Mathf.Lerp(tint.GetComponent<UnityEngine.UI.Image>().color.a, pauseMenuTintAlpha, Time.unscaledDeltaTime * pauseMenuTintAlphaChangeSpeed));

            if (tint.GetComponent<UnityEngine.UI.Image>().color.a > pauseMenuTintAlpha - valueCuttingTreshold)
            {
                tint.GetComponent<UnityEngine.UI.Image>().color = new Color(0f, 0f, 0f, pauseMenuTintAlpha);
            }
        }

        if (!pauseMenu.activeSelf && !settingsMenu.activeSelf && !controlsMenu.activeSelf && !loadingOverlay.activeSelf && tint.GetComponent<UnityEngine.UI.Image>().color.a != 0f)
        {
            tint.GetComponent<UnityEngine.UI.Image>().color = new Color(0f, 0f, 0f, Mathf.Lerp(tint.GetComponent<UnityEngine.UI.Image>().color.a, 0f, Time.unscaledDeltaTime * pauseMenuTintAlphaChangeSpeed));

            if (tint.GetComponent<UnityEngine.UI.Image>().color.a < 0f + valueCuttingTreshold)
            {
                tint.GetComponent<UnityEngine.UI.Image>().color = new Color(0f, 0f, 0f, 0f);
            }
        }
    }

    public void Resume()
    {
        ButtonPress();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Time.timeScale = 1f;

        pauseMenu.SetActive(false);
    }

    public void Settings()
    {
        ButtonPress();

        settingsMenu.SetActive(true);

        pauseMenu.SetActive(false);
    }

    public void Controls()
    {
        ButtonPress();

        controlsMenu.SetActive(true);

        pauseMenu.SetActive(false);
    }

    public void Exit()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        ButtonPress();

        loadingOverlay.SetActive(true);

        pauseMenu.SetActive(false);

        SceneManager.LoadScene("Main Menu");
    }

    public void ButtonPress()
    {
        pressSound.Play();
    }
}
