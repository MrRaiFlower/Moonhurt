using UnityEngine;

public class HouseBounds : MonoBehaviour
{
    [Header("Logic")]
    public string playerLayerName;

    // Logic
    [HideInInspector] public bool isInHouse;

    void Start()
    {

    }

    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.gameObject.layer == LayerMask.NameToLayer(playerLayerName))
        {
            isInHouse = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform.gameObject.layer == LayerMask.NameToLayer(playerLayerName))
        {
            isInHouse = false;
        }
    }
}
