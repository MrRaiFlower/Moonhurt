using UnityEngine;

public class CoinsPile : MonoBehaviour
{
    [Header("Gameobject References")]
    public GameObject[] models;

    void Start()
    {
        
    }

    void Update()
    {

    }

    public void SetModel()
    {
        models[Random.Range(0, models.Length)].SetActive(true);
    }

    public void Collect()
    {
        this.gameObject.SetActive(false);
        this.gameObject.GetComponentInParent<CoinsManager>().Collect();
    }
}
