using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("Gameobject References")]
    public GameObject settingsMenu;
    public GameObject mainMenu;
    public GameObject pauseMenu;
    public GameObject player;

    [Header("Settings Elements")]
    public Slider graphicsQualitySlider;
    public TMP_Text graphicsQualitySliderText;
    public Slider masterVolumeSlider;
    public TMP_Text masterVolumeSliderText;
    public Slider mouseSensitivitySlider;
    public TMP_Text mouseSensitivitySliderText;

    [Header("Other")]
    public string type;

    void Start()
    {
        graphicsQualitySlider.SetValueWithoutNotify(PlayerPrefs.GetInt("GraphicsQuality"));
        masterVolumeSlider.SetValueWithoutNotify(PlayerPrefs.GetInt("MasterVolume"));
        mouseSensitivitySlider.SetValueWithoutNotify(PlayerPrefs.GetInt("MouseSensitivity"));

        switch (PlayerPrefs.GetInt("GraphicsQuality"))
        {
            case 0:

                graphicsQualitySliderText.text = "Низкое";
                break;

            case 1:

                graphicsQualitySliderText.text = "Среднее";
                break;
            
            case 2:

                graphicsQualitySliderText.text = "Высокое";
                break;
        }
        masterVolumeSliderText.text = ((int)masterVolumeSlider.value).ToString();
        mouseSensitivitySliderText.text = ((int)mouseSensitivitySlider.value).ToString();
    }

    void Update()
    {

    }

    public void Return()
    {
        settingsMenu.SetActive(false);

        if (type == "PauseSettingsMenu")
        {
            pauseMenu.SetActive(true);
        }

        if (type == "MainSettingsMenu")
        { 
            mainMenu.SetActive(true);
        }
    }

    public void ChangeGraphicsQuality()
    {
        PlayerPrefs.SetInt("GraphicsQuality", (int)graphicsQualitySlider.value);
        PlayerPrefs.Save();

        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("GraphicsQuality"));

        switch (PlayerPrefs.GetInt("GraphicsQuality"))
        {
            case 0:

                graphicsQualitySliderText.text = "Низкое";
                break;

            case 1:

                graphicsQualitySliderText.text = "Среднее";
                break;
            
            case 2:

                graphicsQualitySliderText.text = "Высокое";
                break;
        }
    }

    public void ChangeMasterVolume()
    {
        PlayerPrefs.SetInt("MasterVolume", (int)masterVolumeSlider.value);
        PlayerPrefs.Save();

        AudioListener.volume = PlayerPrefs.GetInt("MasterVolume") / 100f;

        masterVolumeSliderText.text = PlayerPrefs.GetInt("MasterVolume").ToString();
    }

    public void ChangeMouseSensitivity()
    {
        PlayerPrefs.SetInt("MouseSensitivity", (int)mouseSensitivitySlider.value);
        PlayerPrefs.Save();

        if (type == "PauseSettingsMenu")
        {
            player.GetComponent<Player>().mouseSensitivity = PlayerPrefs.GetInt("MouseSensitivity") / 100f;
        }

        mouseSensitivitySliderText.text = PlayerPrefs.GetInt("MouseSensitivity").ToString();
    }
}
