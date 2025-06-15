using UnityEngine;
using UnityEngine.SceneManagement;

public class Car : MonoBehaviour
{
    [Header("Logic")]
    public GameObject player;
    public GameObject darkTint;
    public GameObject tasks;
    public AudioSource crushSound;
    public float speed;

    // Logic
    private bool moves;

    void Start()
    {

    }

    void Update()
    {
        if (moves)
        {
            this.gameObject.transform.position += (player.transform.position - this.gameObject.transform.position).normalized * Time.deltaTime * speed;
        }

        if ((player.transform.position - this.gameObject.transform.position).sqrMagnitude <= 1f)
        {
            moves = false;
            darkTint.SetActive(true);

            tasks.GetComponent<TasksOverlay>().UpdateTask5();

            crushSound.Play();
            Time.timeScale = 0f;

            SceneManager.LoadScene("Outro");
        }
    }

    public void Kill()
    {
        moves = true;
    }
}
