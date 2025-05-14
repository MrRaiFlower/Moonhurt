using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerControl : MonoBehaviour
{
    public GameObject enemy;

    public string groundLayerName;
    public float normalHeight;
    public float crouchHeight;
    public float heightChangeSpeed;
    public float normalSpeed;
    public float crouchSpeed;
    public float sprintSpeed;
    public float normalAcceleration;
    public float backwardsAcceleration;
    public float drag;
    public float jumpHeight;
    public float jumpCooldown;
    public float walkSoundRange;
    public float sprintSoundRange;
    public float crouchSoundRange;
    public float runDistance;
    public float jumpSoundRange;
    public float valueCuttingTreshhold;

    private Vector2 inputMoveDirection;
    private Vector3 moveDirection;
    private RaycastHit raycastHitInfo;

    private bool isGrounded;
    private bool canJump;
    private bool isJumping;
    private bool isMoving;
    private float speed;
    private float acceleration;
    private float verticalVelocity;
    private float distanceToEnemy;
    private float lerpedHeightScale;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction crouchAction;

    void Start()
    {
        // Setup

        canJump = true;

        this.gameObject.GetComponent<CapsuleCollider>().height = normalHeight;
        this.gameObject.GetComponent<CapsuleCollider>().center = Vector3.up * normalHeight / 2f;

        this.gameObject.GetComponent<CharacterController>().height = crouchHeight;
        this.gameObject.GetComponent<CharacterController>().center = Vector3.up * crouchHeight / 2f;

        moveAction = InputSystem.actions.FindAction("Move");
        crouchAction = InputSystem.actions.FindAction("Crouch");
        sprintAction = InputSystem.actions.FindAction("Sprint");
        jumpAction = InputSystem.actions.FindAction("Jump");
    }

    void Update()
    {
        // Ground Check

        isGrounded = false;

        if (Physics.Raycast(this.gameObject.transform.position, Vector3.down, out raycastHitInfo, 0.1f))
        {
            isGrounded = raycastHitInfo.transform.gameObject.layer == LayerMask.NameToLayer(groundLayerName);
        }

        // Move Direction Inputs

        inputMoveDirection = moveAction.ReadValue<Vector2>();

        isMoving = inputMoveDirection.sqrMagnitude != 0;

        if (isMoving)
        {
            moveDirection = this.gameObject.transform.right * inputMoveDirection.x + this.gameObject.transform.forward * inputMoveDirection.y;
        }

        if (moveDirection.sqrMagnitude > 1)
        {
            moveDirection.Normalize();
        }

        // Height Control

        if (crouchAction.IsPressed() && this.gameObject.GetComponent<CapsuleCollider>().height != crouchHeight)
        {
            lerpedHeightScale = Mathf.Lerp(this.gameObject.GetComponent<CapsuleCollider>().height, crouchHeight, Time.deltaTime * heightChangeSpeed);

            this.gameObject.GetComponent<CapsuleCollider>().height = lerpedHeightScale;
            this.gameObject.GetComponent<CapsuleCollider>().center = Vector3.up * lerpedHeightScale;

            if (this.gameObject.GetComponent<CapsuleCollider>().height < crouchHeight + valueCuttingTreshhold)
            {
                this.gameObject.GetComponent<CapsuleCollider>().center = Vector3.up * crouchHeight / 2f;
                this.gameObject.GetComponent<CapsuleCollider>().height = crouchHeight;
            }
        }

        if (!crouchAction.IsPressed() && this.gameObject.GetComponent<CapsuleCollider>().height != normalHeight)
        {
            lerpedHeightScale = Mathf.Lerp(this.gameObject.GetComponent<CapsuleCollider>().height, normalHeight, Time.deltaTime * heightChangeSpeed);

            this.gameObject.GetComponent<CapsuleCollider>().height = lerpedHeightScale;
            this.gameObject.GetComponent<CapsuleCollider>().center = Vector3.up * lerpedHeightScale;

            if (this.gameObject.GetComponent<CapsuleCollider>().height > normalHeight - valueCuttingTreshhold)
            {
                this.gameObject.GetComponent<CapsuleCollider>().center = Vector3.up * normalHeight / 2f;
                this.gameObject.GetComponent<CapsuleCollider>().height = normalHeight;
            }
        }

        // Speed Control

        if (moveDirection.y > 0)
        {
            acceleration = normalAcceleration;
        }
        else
        {
            acceleration = backwardsAcceleration;
        }

        if (isGrounded)
        {
            if (isMoving)
            {
                if (crouchAction.IsPressed() && speed != crouchSpeed)
                {
                    speed = Mathf.Lerp(speed, crouchSpeed, Time.deltaTime * acceleration);

                    if (speed < crouchSpeed + valueCuttingTreshhold)
                    {
                        speed = crouchSpeed;
                    }
                }
                else if (sprintAction.IsPressed() && inputMoveDirection.y > 0f && speed != sprintSpeed)
                {
                    speed = Mathf.Lerp(speed, sprintSpeed, Time.deltaTime * acceleration);

                    if (speed > sprintSpeed - valueCuttingTreshhold)
                    {
                        speed = sprintSpeed;
                    }
                }
                else if (speed != normalSpeed)
                {
                    speed = Mathf.Lerp(speed, normalSpeed, Time.deltaTime * acceleration);

                    if (speed < normalSpeed + valueCuttingTreshhold && speed > normalSpeed - valueCuttingTreshhold)
                    {
                        speed = normalSpeed;
                    }
                }
            }
            else
            {
                speed = Mathf.Lerp(speed, 0f, Time.deltaTime * drag);

                if (speed < valueCuttingTreshhold)
                {
                    speed = 0f;
                }
            }
        }

        // Jump

        if(jumpAction.IsPressed() && isGrounded && canJump && this.GetComponent<CapsuleCollider>().height == normalHeight)
        {
            canJump = false;
            isJumping = true;

            verticalVelocity = Mathf.Sqrt(jumpHeight * 9.81f);

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // Gravity

        if(isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -1.8f;
        }
        else
        {
            verticalVelocity += -9.81f * Time.deltaTime;
        }

        // Sound

        distanceToEnemy = (enemy.gameObject.transform.position - this.gameObject.gameObject.transform.transform.position).magnitude;

        if (isMoving)
        {
            if (crouchAction.IsPressed() && distanceToEnemy <= crouchSoundRange)
            {
                enemy.gameObject.GetComponent<NavMeshAgent>().SetDestination(this.gameObject.gameObject.transform.position);

                if (enemy.gameObject.GetComponent<Enemy>().state != "Running")
                {
                    enemy.gameObject.GetComponent<Enemy>().state = "Walking";
                }
            }
            else if (sprintAction.IsPressed() && distanceToEnemy <= sprintSoundRange)
            {
                enemy.gameObject.GetComponent<NavMeshAgent>().SetDestination(this.gameObject.gameObject.transform.position);

                if (enemy.gameObject.GetComponent<Enemy>().state != "Running")
                {
                    enemy.gameObject.GetComponent<Enemy>().state = "Walking";
                }
            }
            else if (distanceToEnemy <= walkSoundRange)
            {
                enemy.gameObject.GetComponent<NavMeshAgent>().SetDestination(this.gameObject.gameObject.transform.position);

                if (enemy.gameObject.GetComponent<Enemy>().state != "Running")
                {
                    enemy.gameObject.GetComponent<Enemy>().state = "Walking";
                }
            }
        }

        if (isJumping && isGrounded && distanceToEnemy <= jumpSoundRange)
        {
            isJumping = false;

            enemy.gameObject.GetComponent<NavMeshAgent>().SetDestination(this.gameObject.gameObject.transform.position);

            if (enemy.gameObject.GetComponent<Enemy>().state != "Running")
            {
                enemy.gameObject.GetComponent<Enemy>().state = "Walking";
            }
        }
        
        if (distanceToEnemy <= runDistance && enemy.gameObject.GetComponent<Enemy>().state == "Walking")
        {
            enemy.gameObject.GetComponent<Enemy>().state = "Running";
        }

        // Move

        this.gameObject.GetComponent<CharacterController>().Move(moveDirection * speed * Time.deltaTime + Vector3.up * verticalVelocity * Time.deltaTime);
    }

    private void ResetJump()
    {
        // Reset Jump

        canJump = true;
    }
}
