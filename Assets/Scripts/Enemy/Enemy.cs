using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public GameObject player;
    public GameObject[] wanderingPoints;

    public float startSpeed;
    public float wanderingSpeed;
    public float walkingSpeed;
    public float runningSpeed;
    public float stateChangeAcceleration;
    public float angularSpeed;
    public float acceleration;
    public string playerMaskName;
    public float fOV;
    public float visionRange;
    public int verticalRays;
    public int horizontalRays;
    public float catchDistance;
    public float runningCooldown;
    public float valueCuttingTreshhold;

    [HideInInspector] public string state;

    private Vector3 playerDirection;
    private RaycastHit raycastHitInfo;

    private float speed;
    private bool seesPlayer;
    private float playerDirectionAngle;

    void Start()
    {
        // Setup
        this.gameObject.GetComponent<NavMeshAgent>().speed = startSpeed;
        this.gameObject.GetComponent<NavMeshAgent>().angularSpeed = angularSpeed;
        this.gameObject.GetComponent<NavMeshAgent>().acceleration = acceleration;

        state = "Wandering";
        speed = wanderingSpeed;

        this.gameObject.GetComponent<NavMeshAgent>().SetDestination(wanderingPoints[Random.Range(0, wanderingPoints.Length)].transform.position);
    }

    void Update()
    {
        // Sees Player Check

        playerDirection = player.transform.position - this.gameObject.transform.position;

        playerDirectionAngle = Vector3.Angle(playerDirection, this.gameObject.transform.TransformDirection(Vector3.forward));

        seesPlayer = false;

        if (playerDirectionAngle <= (fOV / 2))
        {
            for (float dv = 0.5f * (player.GetComponent<CapsuleCollider>().height / player.GetComponent<PlayerControl>().normalHeight); dv >= -1.5f * (player.GetComponent<CapsuleCollider>().height / player.GetComponent<PlayerControl>().normalHeight); dv -= 2f / (float)(verticalRays - 1))
            {
                for (float dh = -0.5f; dh <= 0.5f; dh +=1f / (float)(horizontalRays - 1))
                {
                    if (Physics.Raycast(this.gameObject.transform.position + (Vector3.up * 1.5f), playerDirection + new Vector3(dh, dv - ((2f - (player.GetComponent<CapsuleCollider>().height / player.GetComponent<PlayerControl>().normalHeight * 2f)) / 2f), dh), out raycastHitInfo, visionRange))
                    {
                        seesPlayer = raycastHitInfo.transform.gameObject.layer == LayerMask.NameToLayer(playerMaskName);

                        // Debug.DrawRay(this.gameObject.transform.position + (Vector3.up * 1.5f), (playerDirection + new Vector3(dh, dv - ((2f - (player.GetComponent<CapsuleCollider>().height / player.GetComponent<PlayerControl>().normalHeight * 2f)) / 2f), dh)).normalized * raycastHitInfo.distance, Color.green);

                        if (seesPlayer)
                        {
                            // Debug.DrawRay(this.gameObject.transform.position + (Vector3.up * 1.5f), (playerDirection + new Vector3(dh, dv - ((2f - (player.GetComponent<CapsuleCollider>().height / player.GetComponent<PlayerControl>().normalHeight * 2f)) / 2f), dh)).normalized * raycastHitInfo.distance, Color.green);
                            
                            // goto skipLaterRaycast;
                        }
                    }
                }
            }
        }
        
        // skipLaterRaycast:

        // State Control

        if (seesPlayer)
        {
            state = "Running";
            this.gameObject.GetComponent<NavMeshAgent>().SetDestination(player.transform.position);
        }

        if (state == "Running" && !seesPlayer)
        {
            Invoke(nameof(ResetRunning), runningCooldown);
        }

        // Destination Control

        if (state == "Wandering" && this.gameObject.GetComponent<NavMeshAgent>().remainingDistance <= catchDistance)
        {
            this.gameObject.GetComponent<NavMeshAgent>().SetDestination(wanderingPoints[Random.Range(0, wanderingPoints.Length)].transform.position);
        }

        if ((state == "Walking" || state == "Running") && this.gameObject.GetComponent<NavMeshAgent>().remainingDistance <= catchDistance)
        {
            state = "Wandering";
            this.gameObject.GetComponent<NavMeshAgent>().SetDestination(wanderingPoints[Random.Range(0, wanderingPoints.Length)].transform.position);
        }

        // Speed Control

        if (state == "Wandering" && speed != wanderingSpeed)
        {
            speed = Mathf.Lerp(speed, wanderingSpeed, Time.deltaTime * stateChangeAcceleration);

            if (speed < wanderingSpeed + valueCuttingTreshhold)
            {
                speed = wanderingSpeed;
            }
        }
        else if (state == "Walking" && speed != walkingSpeed)
        {
            speed = Mathf.Lerp(speed, walkingSpeed, Time.deltaTime * stateChangeAcceleration);

            if (speed < walkingSpeed + valueCuttingTreshhold && speed > walkingSpeed - valueCuttingTreshhold)
            {
                speed = walkingSpeed;
            }
        }
        else if (state == "Running" && speed != runningSpeed)
        {
            speed = Mathf.Lerp(speed, runningSpeed, Time.deltaTime * stateChangeAcceleration);

            if (speed > runningSpeed - valueCuttingTreshhold)
            {
                speed = runningSpeed;
            }
        }

        this.gameObject.GetComponent<NavMeshAgent>().speed = speed;
    }

    private void ResetRunning()
    {
        if (state == "Running" && !seesPlayer)
        {
            state = "Walking";
        }
    }
}
