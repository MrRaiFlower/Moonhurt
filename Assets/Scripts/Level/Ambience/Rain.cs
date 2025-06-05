using System;
using UnityEngine;

public class Rain : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource rainSound;

    [Header("Volume Control")]
    public float volume;
    public Vector2 houseCenter;
    public float houseRadius;
    public float rainHearingDistance;

    // Volume Control
    private float centerDistance;
    private float volumeFactor;

    void Start()
    {
        rainSound.volume = volume;
    }


    void Update()
    {
        centerDistance = (houseCenter - new Vector2(this.gameObject.GetComponentInParent<Transform>().position.x, this.gameObject.GetComponentInParent<Transform>().position.z)).magnitude;

        if (centerDistance > houseRadius)
        {
            volumeFactor = 1f;
        }
        else if (centerDistance > houseRadius - rainHearingDistance)
        {
            volumeFactor = Mathf.Pow(1f - ((houseRadius - centerDistance) / rainHearingDistance), 2f);
        }
        else
        {
            volumeFactor = 0f;
        }

        rainSound.volume = volume * volumeFactor;
    }
}
