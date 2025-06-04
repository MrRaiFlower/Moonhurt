using UnityEngine;

public class DoubleDoorsSpecial : MonoBehaviour
{
    [Header("Gameobject References")]
    public GameObject myDoor1;
    public GameObject myDoorHandle1;
    public GameObject myDoor2;
    public GameObject myDoorHandle2;
    public GameObject PlayerDetector;

    [Header("Audio Sources")]
    public AudioSource doorcreakSound;

    [Header("Logic")]
    public string doorType;

    // Logic
    [HideInInspector] public bool isOpen;
    [HideInInspector] public bool readyToChangeState;
    [HideInInspector] public bool isInteractable;

    void Start()
    {
        readyToChangeState = true;
        isInteractable = true;
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
            myDoor1.GetComponent<Animator>().Play("DoorClose");
            myDoor2.GetComponent<Animator>().Play("DoorClose");
        }
        else
        {
            if (doorType == "House")
            {
                myDoorHandle1.GetComponent<Animator>().Play("PressHandle");
                myDoorHandle2.GetComponent<Animator>().Play("PressHandle");
            }

            myDoor1.GetComponent<Animator>().Play("DoorOpen");
            myDoor2.GetComponent<Animator>().Play("DoorOpen");
        }

        isOpen = !isOpen;

        Invoke(nameof(ResetAnimationCooldown), 0.42f);
    }

    private void ResetAnimationCooldown()
    {
        readyToChangeState = true;
    }
}
