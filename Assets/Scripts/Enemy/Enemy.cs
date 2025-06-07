using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Analytics;

public class Enemy : MonoBehaviour
{
    [Header("Gameobject References")]
    public GameObject player;
    public GameObject[] wanderingPoints;

    [Header("Component References")]
    public Transform myTransform;
    public NavMeshAgent myNavMeshAgent;

    [Header("Audio Sources")]
    public AudioSource giantHollowTubeSound;
    public AudioSource roarSound;

    [Header("Giant Hollow Tube")]
    public float giantHollowTubeSoundChance;

    [Header("Sound")]
    public float roarSoundVolume;
    public float roarSoundDifferentFloorVolumeFactor;
    public float roarSoundVolumeChangeSpeed;
    public float roarSoundVolumeChangeSpeedOnDeath;

    [Header("Raycast")]
    public string playerLayerName;
    public string houseMiddlePieceLayerName;

    [Header("Nav Mesh Agent Parameters")]
    public float startSpeed;
    public float angularSpeed;
    public float acceleration;

    [Header("Movement Speed")]
    public float wanderingSpeed;
    public float walkingSpeed;
    public float runningSpeed;
    public float stateChangeAcceleration;

    [Header("Vision")]
    public float fOV;
    public float visionRange;
    public int verticalRays;
    public int horizontalRays;

    [Header("Player Hearing")]
    public float differentFloorHearingFactor;
    public float walkSoundRange;
    public float sprintSoundRange;
    public float crouchSoundRange;
    public float jumpSoundRange;
    public float groundingSoundRange;
    public float doorInteractionSoundRange;

    [Header("Logic")]
    public float runDistance;
    public float catchDistance;
    public float runningMemoryTime;

    [Header("Misc")]
    public float valueCuttingTreshold;

    // State
    private string state;
    private string previousFrameState;
    private float distanceToPlayer;
    private bool isOnDifferentFloor;
    [HideInInspector] public bool coughtPlayer;

    // Vision
    private Vector3 playerDirection;
    private RaycastHit raycastHitInfo;
    private bool seesPlayer;
    private float playerDirectionAngle;

    // Velocity
    private float speed;

    // Player Hearing
    private float actualDifferentFloorHearingFactor;


    void Start()
    {
        // Setup

        myNavMeshAgent.speed = startSpeed;
        myNavMeshAgent.angularSpeed = angularSpeed;
        myNavMeshAgent.acceleration = acceleration;

        state = "Wandering";
        speed = wanderingSpeed;

        myNavMeshAgent.SetDestination(wanderingPoints[Random.Range(0, wanderingPoints.Length)].transform.position);
    }

    void Update()
    {
        // If Cought

        if (coughtPlayer && roarSound.volume != 0f)
        {
            roarSound.volume = Mathf.Lerp(roarSound.volume, 0f, Time.unscaledDeltaTime * roarSoundVolumeChangeSpeedOnDeath);

            if (roarSound.volume < 0f + valueCuttingTreshold)
            {
                roarSound.volume = 0f;
            }
        }

        // Sees Player Check

        playerDirection = player.transform.position - this.gameObject.transform.position;

        playerDirectionAngle = Vector3.Angle(playerDirection, this.gameObject.transform.TransformDirection(Vector3.forward));

        seesPlayer = false;

        if (playerDirectionAngle <= (fOV / 2))
        {
            for (float dv = player.GetComponent<Player>().normalHeight * 0.25f * (player.GetComponent<Player>().myCharacterController.height / player.GetComponent<Player>().normalHeight); dv >= player.GetComponent<Player>().normalHeight * -0.75f * (player.GetComponent<Player>().myCharacterController.height / player.GetComponent<Player>().normalHeight); dv -= player.GetComponent<Player>().normalHeight / (float)(verticalRays - 1))
            {
                for (float dh = -0.5f; dh <= 0.5f; dh += 1f / (float)(horizontalRays - 1))
                {
                    if (Physics.Raycast(this.gameObject.transform.position + (Vector3.up * 1.5f), playerDirection + new Vector3(dh, dv - ((player.GetComponent<Player>().normalHeight - (player.GetComponent<Player>().myCharacterController.height / player.GetComponent<Player>().normalHeight * 2f)) / 2f), dh), out raycastHitInfo, visionRange))
                    {
                        seesPlayer = raycastHitInfo.transform.gameObject.layer == LayerMask.NameToLayer(playerLayerName);

                        // Debug.DrawRay(this.gameObject.transform.position + (Vector3.up * 1.5f), (playerDirection + new Vector3(dh, dv - ((player.GetComponent<Player>().normalHeight - (player.GetComponent<Player>().myCharacterController.height / player.GetComponent<Player>().normalHeight * 2f)) / 2f), dh)).normalized * raycastHitInfo.distance, Color.green);

                        if (seesPlayer)
                        {
                            // Debug.DrawRay(this.gameObject.transform.position + (Vector3.up * 1.5f), (playerDirection + new Vector3(dh, dv - ((player.GetComponent<Player>().normalHeight - (layer.GetComponent<Player>().myCharacterController.height / player.GetComponent<Player>().normalHeight * 2f)) / 2f), dh)).normalized * raycastHitInfo.distance, Color.green);

                            goto skipLaterRaycast;
                        }
                    }
                }
            }
        }

    skipLaterRaycast:

        // Floor Check

        if (!seesPlayer)
        {
            isOnDifferentFloor = Physics.Raycast(this.gameObject.transform.position + Vector3.up, playerDirection, 50f, LayerMask.NameToLayer(houseMiddlePieceLayerName));
        }
        else
        {
            isOnDifferentFloor = false;
        }

        // First State Control

        previousFrameState = state;

        distanceToPlayer = (myTransform.position - player.transform.position).magnitude;

        if (distanceToPlayer <= runDistance && state == "Walking")
        {
            state = "Running";
        }

        if (seesPlayer)
        {
            state = "Running";
            myNavMeshAgent.SetDestination(player.transform.position);
        }

        if (state == "Running" && !seesPlayer)
        {
            Invoke(nameof(ResetRunning), runningMemoryTime);
        }

        // Wandering Destination Control

        if (state == "Wandering" && myNavMeshAgent.remainingDistance <= catchDistance)
        {
            myNavMeshAgent.SetDestination(wanderingPoints[Random.Range(0, wanderingPoints.Length)].transform.position);
        }

        if ((state == "Walking" || state == "Running") && myNavMeshAgent.remainingDistance <= catchDistance)
        {
            state = "Wandering";

            myNavMeshAgent.SetDestination(wanderingPoints[Random.Range(0, wanderingPoints.Length)].transform.position);
        }

        // Speed Control

        if (state == "Wandering" && speed != wanderingSpeed)
        {
            speed = Mathf.Lerp(speed, wanderingSpeed, Time.deltaTime * stateChangeAcceleration);

            if (speed < wanderingSpeed + valueCuttingTreshold)
            {
                speed = wanderingSpeed;
            }
        }
        else if (state == "Walking" && speed != walkingSpeed)
        {
            speed = Mathf.Lerp(speed, walkingSpeed, Time.deltaTime * stateChangeAcceleration);

            if (speed < walkingSpeed + valueCuttingTreshold && speed > walkingSpeed - valueCuttingTreshold)
            {
                speed = walkingSpeed;
            }
        }
        else if (state == "Running" && speed != runningSpeed)
        {
            speed = Mathf.Lerp(speed, runningSpeed, Time.deltaTime * stateChangeAcceleration);

            if (speed > runningSpeed - valueCuttingTreshold)
            {
                speed = runningSpeed;
            }
        }

        myNavMeshAgent.speed = speed;

        // Player Hearing

        if (isOnDifferentFloor)
        {
            actualDifferentFloorHearingFactor = differentFloorHearingFactor;
        }
        else
        {
            actualDifferentFloorHearingFactor = 1f;
        }

        switch (player.GetComponent<Player>().state)
        {
            case "Crouching":

                if (distanceToPlayer <= crouchSoundRange * actualDifferentFloorHearingFactor)
                {
                    myNavMeshAgent.SetDestination(player.transform.position);

                    if (state != "Running")
                    {
                        state = "Walking";
                    }
                }
                break;

            case "Sprinting":

                if (distanceToPlayer <= sprintSoundRange * actualDifferentFloorHearingFactor)
                {
                    myNavMeshAgent.SetDestination(player.transform.position);

                    if (state != "Running")
                    {
                        state = "Walking";
                    }
                }
                break;

            case "Walking":

                if (distanceToPlayer <= walkSoundRange * actualDifferentFloorHearingFactor)
                {
                    myNavMeshAgent.SetDestination(player.transform.position);

                    if (state != "Running")
                    {
                        state = "Walking";
                    }
                }
                break;
        }

        if (player.GetComponent<Player>().hasJumped && distanceToPlayer <= jumpSoundRange * actualDifferentFloorHearingFactor)
        {
            myNavMeshAgent.SetDestination(player.transform.position);

            if (state != "Running")
            {
                state = "Walking";
            }
        }

        if (player.GetComponent<Player>().hasGrounded && distanceToPlayer <= groundingSoundRange * actualDifferentFloorHearingFactor && player.GetComponent<Player>().fallingTime > 0.12f)
        {
            myNavMeshAgent.SetDestination(player.transform.position);

            if (state != "Running")
            {
                state = "Walking";
            }
        }

        if (player.GetComponent<Player>().hasInteractedWithDoor && distanceToPlayer <= doorInteractionSoundRange * actualDifferentFloorHearingFactor)
        {
            myNavMeshAgent.SetDestination(player.transform.position);

            if (state != "Running")
            {
                state = "Walking";
            }
        }

        // Giant Hollow Tube

        if (previousFrameState == "Wandering" && state == "Walking")
        {
            if (giantHollowTubeSoundChance >= Random.Range(0f, 1f))
            {
                giantHollowTubeSound.Play();
            }
        }

        // Roar Sound

        if (isOnDifferentFloor && roarSound.volume == roarSoundVolume * roarSoundDifferentFloorVolumeFactor)
        {
            roarSound.volume = Mathf.Lerp(roarSound.volume, roarSoundVolume * roarSoundDifferentFloorVolumeFactor, Time.deltaTime * roarSoundVolumeChangeSpeed);

            if (roarSound.volume < (roarSoundVolume * roarSoundDifferentFloorVolumeFactor) + valueCuttingTreshold)
            {
                roarSound.volume = roarSoundVolume * roarSoundDifferentFloorVolumeFactor;
            }
        }

        if (!isOnDifferentFloor && roarSound.volume == roarSoundVolume)
        {
            roarSound.volume = Mathf.Lerp(roarSound.volume, roarSoundVolume, Time.deltaTime * roarSoundVolumeChangeSpeed);

            if (roarSound.volume > roarSoundVolume - valueCuttingTreshold)
            {
                roarSound.volume = roarSoundVolume;
            }
        }

        if (!roarSound.isPlaying)
        {
            roarSound.Play();
        }
    }

    private void ResetRunning()
    {
        if (state == "Running" && !seesPlayer)
        {
            state = "Walking";
        }
    }
}
