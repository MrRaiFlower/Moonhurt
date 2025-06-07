using System;
using UnityEngine;

public class Rain : MonoBehaviour
{
    [Header("Gameobject References")]
    public GameObject rainParticleSystem;

    [Header("Audio Sources")]
    public AudioSource rainSound;

    [Header("Distance Calculation")]
    public GameObject[] windows;
    public GameObject houseBounds;

    [Header("Volume Control")]
    public float outDoorRainVolume;
    public float inDoorRainVolume;
    public float volumeChangeSpeed;
    public float rainHearingDistance;

    [Header("Misc")]
    public float valueCuttingTreshold;

    // Volume Control
    private float currentWindowDistance;
    private float closestWindowDistance;
    private float volume;
    private float volumeFactor;

    void Start()
    {
        
    }


    void Update()
    {
        if (!houseBounds.GetComponent<HouseBounds>().isInHouse && volume != outDoorRainVolume)
        {
            volume = Mathf.Lerp(volume, volumeChangeSpeed, Time.deltaTime * volumeChangeSpeed);

            if (volume > outDoorRainVolume - valueCuttingTreshold)
            {
                volume = outDoorRainVolume;
            }
        }

        if (houseBounds.GetComponent<HouseBounds>().isInHouse && volume != inDoorRainVolume)
        {
            volume = Mathf.Lerp(volume, inDoorRainVolume, Time.deltaTime * volumeChangeSpeed);

            if (volume < inDoorRainVolume + valueCuttingTreshold)
            {
                volume = inDoorRainVolume;
            }
        }

        if (!houseBounds.GetComponent<HouseBounds>().isInHouse)
        {
            volumeFactor = 1f;
        }

        if (houseBounds.GetComponent<HouseBounds>().isInHouse)
        {
            closestWindowDistance = 1000f;

            foreach (GameObject window in windows)
            {
                currentWindowDistance = (new Vector2(window.transform.position.x, window.transform.position.z) - new Vector2(this.gameObject.GetComponentInParent<Transform>().position.x, this.gameObject.GetComponentInParent<Transform>().position.z)).magnitude;

                if (currentWindowDistance < closestWindowDistance)
                {
                    closestWindowDistance = currentWindowDistance;
                }
            }

            closestWindowDistance += 0.5f;

            volumeFactor = Mathf.Pow(1f - (closestWindowDistance / rainHearingDistance), 2f);

            if (volumeFactor < 0f + valueCuttingTreshold)
            {
                volumeFactor = 0f;
            }
        }

        if (closestWindowDistance > rainHearingDistance)
        {
            volumeFactor = 0f;
        }

        if (volumeFactor != 0f && !rainParticleSystem.activeSelf)
            {
                rainParticleSystem.SetActive(true);
            }

        if (volumeFactor == 0f && rainParticleSystem.activeSelf)
        {
            rainParticleSystem.SetActive(false);
        }

        rainSound.volume = volume * volumeFactor;
    }
}
