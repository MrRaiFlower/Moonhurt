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
        volumeFactor = Mathf.Pow(Mathf.Clamp(centerDistance / houseRadius, 0f, 1f) / 1f, 7f);

        rainSound.volume = volume * volumeFactor;
    }
}
