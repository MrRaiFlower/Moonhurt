using UnityEngine;

public class CoinsPile : MonoBehaviour
{
    [Header("Gameobject References")]
    public GameObject[] models;

    [Header("Component References")]
    public Animator myAnimator;

    [Header("Audio Sources")]
    public AudioSource coinsPileCollectionSound;

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
        this.gameObject.GetComponentInParent<CoinsManager>().Collect();

        myAnimator.Play("Collect");
        Invoke(nameof(PlaySound), 0.008f);
    }

    private void PlaySound()
    {
        coinsPileCollectionSound.pitch = Random.Range(0.9f, 1.1f);
        coinsPileCollectionSound.Play();
        Invoke(nameof(Disappear), 0.2f);
    }

    private void Disappear()
    {
        this.gameObject.SetActive(false);
    }
}
