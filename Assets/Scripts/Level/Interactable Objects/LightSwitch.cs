using UnityEngine;

public class LightSwitch : MonoBehaviour
{
    [Header("Gameobject References")]
    public GameObject switchButton;

    [Header("Audio Sources")]
    public AudioSource clickSound;

    // Logic
    private bool isOn;
    private bool isReadyToChangeState;

    void Start()
    {
        isReadyToChangeState = true;
    }

    void Update()
    {

    }

    public void Switch()
    {
        if (isReadyToChangeState)
        {
            isReadyToChangeState = false;

            clickSound.Play();

            if (isOn)
            {
                switchButton.GetComponent<Animator>().Play("Off");
            }
            else
            {
                switchButton.GetComponent<Animator>().Play("On");
            }

            isOn = !isOn;

            Invoke(nameof(ResetState), 0.2f);
        }
    }

    private void ResetState()
    {
        isReadyToChangeState = true;
    }
}
