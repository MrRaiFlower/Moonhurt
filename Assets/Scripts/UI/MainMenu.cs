using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Gameobject References")]
    public GameObject mainMenu;
    public GameObject settingsMenu;

    // Inputs
    private InputAction escapeAction;

    void Start()
    {
        settingsMenu.SetActive(false);

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
