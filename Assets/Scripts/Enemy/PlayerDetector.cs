using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    [Header("Gameobject References")]
    public GameObject deathScreen;

    [Header("Logic")]
    public string playerLayerName;

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
            deathScreen.GetComponent<DeathScreen>().Die();
        }
    }
}
