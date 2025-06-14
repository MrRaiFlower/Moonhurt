using UnityEngine;

public class Outro : MonoBehaviour
{
    void Start()
    {
        Time.timeScale = 1f;
        Invoke(nameof(LeaveGame), 5f);
    }

    void Update()
    {

    }

    private void LeaveGame()
    {
        Application.Quit();
    }
}
