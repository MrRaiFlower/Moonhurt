using UnityEngine;
using UnityEngine.Rendering;

public class Door : MonoBehaviour
{
    [Header("Gameobject References")]
    public GameObject myDoor;
    public GameObject myDoorHandle;

    [Header("Audio Sources")]
    public AudioSource doorcreakSound;

    // Logic
    [HideInInspector] public bool isOpen;
    [HideInInspector] public bool readyToChangeState;

    void Start()
    {
        readyToChangeState = true;
    }

    void Update()
    {
        
    }

    public void ChangeState()
    {
        readyToChangeState = false;

        doorcreakSound.Play();

        if (isOpen)
        {
            myDoor.GetComponent<Animator>().Play("DoorClose");
        }
        else
        {
            myDoorHandle.GetComponent<Animator>().Play("PressHandle");
            myDoor.GetComponent<Animator>().Play("DoorOpen");
        }

        isOpen = !isOpen;

        Invoke(nameof(ResetAnimationCooldown), 0.42f);
    }

    private void ResetAnimationCooldown()
    {
        readyToChangeState = true;
    }
}
