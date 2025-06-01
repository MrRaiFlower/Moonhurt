using UnityEngine;

public class AmbiencePlayer : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource randomAmbienceSound;

    [Header("Random Ambience Sounds Parameters")]
    public float randomAmbienceSoundInterval;
    public float randomAmbienceSoundChance;

    // Random Ambience Sounds
    private float timeCounter;

    void Start()
    {
        
    }

    void Update()
    {
        timeCounter += Time.deltaTime;

        if (timeCounter >= randomAmbienceSoundInterval)
        {
            timeCounter = 0f;

            if (randomAmbienceSoundChance >= Random.Range(0f, 1f) && !randomAmbienceSound.isPlaying)
            {
                randomAmbienceSound.Play();
            }
        }
    }
}
