using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Gameobject References")]
    public GameObject myDoor;
    public GameObject myDoorHandle;

    [Header("Component References")]
    public AudioSource doorcreakSound;

    // Logic
    private bool isOpen;
    [HideInInspector] public bool readyToPlayAnimation;

    void Start()
    {
        readyToPlayAnimation = true;
    }

    void Update()
    {

    }

    public void Interact()
    {
        readyToPlayAnimation = false;

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
        
        Invoke(nameof(ResetAnimationCooldown), 0.5f);
    
    }

    private void ResetAnimationCooldown()
    {
        readyToPlayAnimation = true;
    }
}
