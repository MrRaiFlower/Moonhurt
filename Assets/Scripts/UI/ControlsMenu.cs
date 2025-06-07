using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControlsMenu : MonoBehaviour
{
    [Header("Gameobject References")]
    public GameObject controlsMenu;
    public GameObject mainMenu;
    public GameObject pauseMenu;

    [Header("Other")]
    public string type;

    void Start()
    {

    }

    void Update()
    {

    }

    public void Return()
    {
        controlsMenu.SetActive(false);

        if (type == "PauseControlsMenu")
        {
            pauseMenu.SetActive(true);
        }

        if (type == "MainControlsMenu")
        { 
            mainMenu.SetActive(true);
        }
    }
}
