using UnityEngine;

public class CarPlayerDetector : MonoBehaviour
{
    [Header("Logic")]
    public GameObject car;
    public string playerLayerName;

    void Start()
    {
        car.SetActive(false);
    }

    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.gameObject.layer == LayerMask.NameToLayer(playerLayerName))
        {
            car.SetActive(true);
            car.GetComponent<Car>().Kill();

            this.gameObject.SetActive(false);
        }
    }
}
