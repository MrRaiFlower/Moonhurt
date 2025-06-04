using UnityEngine;

public class DoubleDoorsTrigger : MonoBehaviour
{
    [Header("Gameobject References")]
    public GameObject TasksOverlay;

    [Header("Player Detector")]
    public string playerLayerName;

    // Logic
    [HideInInspector] public string state;

    void Start()
    {
        state = "WaitsToBeEntered";
    }

    void Update()
    {

    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform.gameObject.layer == LayerMask.NameToLayer(playerLayerName) && this.gameObject.GetComponentInParent<DoubleDoorsSpecial>().isInteractable)
        {
            if (state == "WaitsToBeEntered")
            {
                if (other.transform.position.z < this.gameObject.transform.position.z)
                {
                    if (this.gameObject.GetComponentInParent<DoubleDoorsSpecial>().isOpen)
                    {
                        this.gameObject.GetComponentInParent<DoubleDoorsSpecial>().ChangeState();
                    }

                    this.gameObject.GetComponentInParent<DoubleDoorsSpecial>().isInteractable = false;

                    state = "WaitsToBeExited";

                    if (this.gameObject.GetComponentInParent<DoubleDoorsSpecial>().doorType == "House")
                    {
                        TasksOverlay.GetComponent<TasksOverlay>().UpdateTask1();
                    }
                }
            }
            else if (state == "WaitsToBeExited")
            {
                if (other.transform.position.z > this.gameObject.transform.position.z && this.gameObject.GetComponentInParent<DoubleDoorsSpecial>().isInteractable)
                {
                    if (this.gameObject.GetComponentInParent<DoubleDoorsSpecial>().isOpen)
                    {
                        this.gameObject.GetComponentInParent<DoubleDoorsSpecial>().ChangeState();
                    }

                    this.gameObject.GetComponentInParent<DoubleDoorsSpecial>().isInteractable = false;

                    state = "WaitsToBeEntered";

                    if (this.gameObject.GetComponentInParent<DoubleDoorsSpecial>().doorType == "Gate")
                    {
                        TasksOverlay.GetComponent<TasksOverlay>().UpdateTask3();
                    }
                }
            }
        }
    }
}
