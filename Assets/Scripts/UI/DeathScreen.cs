using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreen : MonoBehaviour
{
    [Header("Gameobject References")]
    public GameObject deathScreenImage;
    public GameObject text;
    public GameObject loading;
    public GameObject enemy;

    [Header("Audio Sources")]
    public AudioSource hitSound;
    public AudioSource flatLineSound;
    public AudioSource buttonPressSound;

    void Start()
    {
        text.SetActive(false);
    }

    void Update()
    {
        
    }

    public void Die()
    {
        deathScreenImage.GetComponent<Animator>().Play("Die");

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        hitSound.Play();
        Invoke(nameof(Delayed), 0.1f);
    }

    public void Restart()
    {
        loading.SetActive(true);
        text.SetActive(false);

        buttonPressSound.Play();
        SceneManager.LoadScene("Level");
    }

    public void Menu()
    {
        loading.SetActive(true);
        text.SetActive(false);

        buttonPressSound.Play();
        SceneManager.LoadScene("Main Menu");
    }

    private void Delayed()
    {
        text.SetActive(true);

        flatLineSound.Play();

        Time.timeScale = 0f;
        
        enemy.GetComponent<Enemy>().coughtPlayer = true;
    }
}
