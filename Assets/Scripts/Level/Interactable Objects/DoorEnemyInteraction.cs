using UnityEngine;

public class DoorEnemyInteraction : MonoBehaviour
{
    [Header("Enemy Interaction")]
    public string enemyLayerName;
    
    void Start()
    {

    }

    void Update()
    {

    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.transform.gameObject.layer == LayerMask.NameToLayer(enemyLayerName))
        {
            if (!this.gameObject.GetComponentInParent<Door>().isOpen)
            {
                this.gameObject.GetComponentInParent<Door>().ChangeState();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform.gameObject.layer == LayerMask.NameToLayer(enemyLayerName))
        {
            if (this.gameObject.GetComponentInParent<Door>().isOpen)
            {
                this.gameObject.GetComponentInParent<Door>().ChangeState();
            }
        }
    }
}
