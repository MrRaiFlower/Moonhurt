using UnityEngine;
using UnityEngine.InputSystem;

public class Intro : MonoBehaviour
{
    // Input
    private InputAction skipAction;

    void Start()
    {
        skipAction = InputSystem.actions.FindAction("Skip");

        Time.timeScale = 0f;
    }

    void Update()
    {
        if (skipAction.WasPressedThisFrame())
        {
            StartGame();
        }
    }

    private void StartGame()
    {
        Time.timeScale = 1f;
        this.gameObject.SetActive(false);
    }
}
