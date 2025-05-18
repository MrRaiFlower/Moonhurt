using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Gameobject References")]
    public GameObject myCameraGameObject;
    public Camera myCameraCamera;
    public GameObject flashlight;
    public GameObject enemy;

    [Header("Component References")]
    public CharacterController myCharacterController;
    public CapsuleCollider myCapsuleCollider;

    [Header("Audio Sources")]

    public AudioSource normalFootstepsSound;
    public AudioSource lightFootstepsSound;
    public AudioSource heavyFootstepsSound;
    public AudioSource jumpSound;
    public AudioSource landingSound;
    public AudioSource heavyBreathingSound;
    public AudioSource flashlightTurnOnSound;
    public AudioSource flashlightTurnOffSound;

    [Header("Raycast")]
    public string groundLayerName;

    [Header("Camera")]
    public float mouseSensitivity;
    public float cameraPositionY;

    [Header("Crouching Height")]
    public float normalHeight;
    public float crouchHeight;
    public float heightChangeSpeed;

    [Header("Movement Speed")]
    public float normalSpeed;
    public float crouchSpeed;
    public float sprintSpeed;
    public float normalAcceleration;
    public float backwardsAcceleration;
    public float drag;

    [Header("Stamina")]
    public float maxStamina;
    public float restingCooldown;
    public float staminaJumpConsumption;
    public float staminaSprintingConsumption;
    public float staminaWalkingRegeneration;
    public float staminaCrouchingRegeneration;
    public float staminaIdleRegeneration;

    [Header("Jump")]
    public float jumpHeight;

    [Header("Flashlight")]
    public float flashlightCooldown;

    [Header("Sounds")]
    public float normalFootstepsCooldown;
    public float lightFootstepsCooldown;
    public float heavyFootstepsCooldown;
    public float heavyBreathingCooldown;
    public float heavyBreathingVolumeChangeSpeed;

    [Header("Misc")]
    public float valueCuttingTreshold;

    // Movement Input
    private Vector2 inputMoveDirection;
    private Vector3 moveDirection;
    [HideInInspector] public bool isMoving;

    // Ground Check
    private RaycastHit raycastHitInfo;
    [HideInInspector] public bool isGrounded;
    private bool wasGrounded;

    // State
    [HideInInspector] public string state;

    // Stamina
    private float stamina;
    private bool isTired;

    // Jump
    private bool canJump;
    [HideInInspector] public bool hasJumped;
    [HideInInspector] public bool hasLanded;

    // Flashlight
    private bool flashlightIsOn;
    private bool canUseFlashlight;

    // Crouching
    private float lerpedHeightScale;

    // Camera Rotation
    private float cursorMovementX;
    private float cursorMovementY;
    private float cameraRotationX;
    private float cameraRotationY;

    // Sound
    private bool normalFootstepsReadyToPlay;
    private bool lightFootstepsReadyToPlay;
    private bool heavyFootstepsReadyToPlay;

    private bool heavyBreathingReadyToPlay;

    // Velocity
    private float speed;
    private float acceleration;
    private float verticalVelocity;

    // Inputs
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction crouchAction;
    private InputAction lookAction;
    private InputAction flashlightAction;

    void Start()
    {
        // Setup

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        myCapsuleCollider.height = normalHeight;
        myCapsuleCollider.center = Vector3.up * normalHeight / 2f;

        myCharacterController.height = crouchHeight;
        myCharacterController.center = Vector3.up * crouchHeight / 2f;

        myCameraGameObject.transform.localPosition = Vector3.up * (cameraPositionY * myCapsuleCollider.height);

        normalFootstepsReadyToPlay = true;
        lightFootstepsReadyToPlay = true;
        heavyFootstepsReadyToPlay = true;

        heavyBreathingReadyToPlay = true;

        canUseFlashlight = true;

        heavyBreathingSound.volume = 0f;

        stamina = maxStamina;

        canJump = true;

        moveAction = InputSystem.actions.FindAction("Move");
        crouchAction = InputSystem.actions.FindAction("Crouch");
        sprintAction = InputSystem.actions.FindAction("Sprint");
        jumpAction = InputSystem.actions.FindAction("Jump");
        lookAction = InputSystem.actions.FindAction("Look");
        flashlightAction = InputSystem.actions.FindAction("Flashlight");
    }

    void Update()
    {
        // Camera Rotation Inputs

        cursorMovementX = lookAction.ReadValue<Vector2>().x;
        cursorMovementY = lookAction.ReadValue<Vector2>().y;

        cameraRotationY += cursorMovementX * Time.deltaTime * mouseSensitivity;

        cameraRotationX -= cursorMovementY * Time.deltaTime * mouseSensitivity;
        cameraRotationX = Mathf.Clamp(cameraRotationX, -90f, 90f);

        // Camera Rotation

        myCameraGameObject.transform.localRotation = Quaternion.Euler(cameraRotationX, 0f, 0f);
        this.gameObject.transform.rotation = Quaternion.Euler(0f, cameraRotationY, 0f);

        // Flashlight

        if (flashlightAction.WasPressedThisFrame() && canUseFlashlight)
        {
            canUseFlashlight = false;

            if (!flashlightIsOn)
            {
                flashlightIsOn = true;
                flashlightTurnOnSound.Play();
                flashlight.GetComponent<Animator>().Play("TurnOn");
            }
            else
            {
                flashlightIsOn = false;
                flashlightTurnOffSound.Play();
                flashlight.GetComponent<Animator>().Play("TurnOff");
            }

            Invoke(nameof(ResetFlashlight), flashlightCooldown);
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

        // Ground Check

        wasGrounded = isGrounded;

        isGrounded = false;

        if (Physics.Raycast(this.gameObject.transform.position, Vector3.down, out raycastHitInfo, 0.1f))
        {
            isGrounded = raycastHitInfo.transform.gameObject.layer == LayerMask.NameToLayer(groundLayerName);
        }

        hasLanded = false;

        if (!wasGrounded && isGrounded)
        {
            hasLanded = true;
        }

        // State Control

        if (isGrounded)
        {
            if (isMoving)
            {
                if (crouchAction.IsPressed())
                {
                    state = "Crouching";
                }
                else if (sprintAction.IsPressed() && inputMoveDirection.y > 0f && stamina > 0f && !isTired)
                {
                    state = "Sprinting";
                }
                else
                {
                    state = "Walking";
                }
            }
            else
            {
                state = "Idle";
            }
        }
        else
        {
            state = "Falling";
        }

        // Stamina Control

        if (hasJumped)
        {
            stamina -= staminaJumpConsumption;
        }

        switch (state)
        {
            case "Crouching":

                stamina += staminaCrouchingRegeneration * Time.deltaTime;
                break;

            case "Sprinting":

                stamina -= staminaSprintingConsumption * Time.deltaTime;
                break;

            case "Walking":

                stamina += staminaWalkingRegeneration * Time.deltaTime;
                break;

            case "Idle":

                stamina += staminaIdleRegeneration * Time.deltaTime;
                break;
        }

        if (stamina > maxStamina - valueCuttingTreshold)
        {
            stamina = maxStamina;
        }

        if (stamina < 0f + valueCuttingTreshold)
        {
            stamina = 0f;
        }

        if (stamina == 0f)
        {
            isTired = true;

            Invoke(nameof(ResetTierd), restingCooldown);
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

        switch (state)
        {
            case "Crouching":

                if (speed != crouchSpeed)
                {
                    speed = Mathf.Lerp(speed, crouchSpeed, Time.deltaTime * acceleration);

                    if (speed < crouchSpeed + valueCuttingTreshold)
                    {
                        speed = crouchSpeed;
                    }
                }
                break;

            case "Sprinting":

                if (speed != sprintSpeed)
                {
                    speed = Mathf.Lerp(speed, sprintSpeed, Time.deltaTime * acceleration);

                    if (speed > sprintSpeed - valueCuttingTreshold)
                    {
                        speed = sprintSpeed;
                    }
                }
                break;

            case "Walking":

                if (speed != normalSpeed)
                {
                    speed = Mathf.Lerp(speed, normalSpeed, Time.deltaTime * acceleration);

                    if (speed < normalSpeed + valueCuttingTreshold && speed > normalSpeed - valueCuttingTreshold)
                    {
                        speed = normalSpeed;
                    }
                }
                break;

            case "Idle":

                if (speed != 0f)
                {
                    speed = Mathf.Lerp(speed, 0f, Time.deltaTime * drag);

                    if (speed < 0f + valueCuttingTreshold)
                    {
                        speed = 0f;
                    }
                }
                break;
        }

        // Gravity

        if (isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -1.69f;
        }
        else
        {
            verticalVelocity += -9.81f * Time.deltaTime;
        }

        // Jump

        hasJumped = false;

        if (jumpAction.IsPressed() && isGrounded && canJump && stamina > 0f && !isTired)
        {
            hasJumped = true;
            canJump = false;

            verticalVelocity = Mathf.Sqrt(jumpHeight * 9.81f);

            Invoke(nameof(ResetJump), 0.13f);
        }

        // Height Control

        if (state == "Crouching" && myCapsuleCollider.height != crouchHeight)
        {
            lerpedHeightScale = Mathf.Lerp(myCapsuleCollider.height, crouchHeight, Time.deltaTime * heightChangeSpeed);

            myCapsuleCollider.height = lerpedHeightScale;
            myCapsuleCollider.center = Vector3.up * lerpedHeightScale;

            if (myCapsuleCollider.height < crouchHeight + valueCuttingTreshold)
            {
                myCapsuleCollider.center = Vector3.up * crouchHeight / 2f;
                myCapsuleCollider.height = crouchHeight;
            }

            myCameraGameObject.transform.localPosition = Vector3.up * (cameraPositionY * myCapsuleCollider.height);
        }

        if (state != "Crouching" && myCapsuleCollider.height != normalHeight)
        {
            lerpedHeightScale = Mathf.Lerp(myCapsuleCollider.height, normalHeight, Time.deltaTime * heightChangeSpeed);

            myCapsuleCollider.height = lerpedHeightScale;
            myCapsuleCollider.center = Vector3.up * lerpedHeightScale;

            if (myCapsuleCollider.height > normalHeight - valueCuttingTreshold)
            {
                myCapsuleCollider.center = Vector3.up * normalHeight / 2f;
                myCapsuleCollider.height = normalHeight;
            }

            myCameraGameObject.transform.localPosition = Vector3.up * (cameraPositionY * myCapsuleCollider.height);
        }

        // Move

        myCharacterController.Move(moveDirection * speed * Time.deltaTime + Vector3.up * verticalVelocity * Time.deltaTime);

        // Sound and Animation

        if (hasLanded)
        {
            normalFootstepsReadyToPlay = false;
            lightFootstepsReadyToPlay = false;
            heavyFootstepsReadyToPlay = false;

            Invoke(nameof(NormalFootstepCooldown), normalFootstepsCooldown);
            Invoke(nameof(LightFootstepCooldown), lightFootstepsCooldown);
            Invoke(nameof(HeavyFootstepCooldown), heavyFootstepsCooldown);
        }

        if (isTired && heavyBreathingSound.volume != 1f)
        {
            heavyBreathingSound.volume = Mathf.Lerp(heavyBreathingSound.volume, 1f, Time.deltaTime * heavyBreathingVolumeChangeSpeed);

            if (heavyBreathingSound.volume > 1f - valueCuttingTreshold)
            {
                heavyBreathingSound.volume = 1f;
            }
        }
        else if (heavyBreathingSound.volume != 0f)
        {
            heavyBreathingSound.volume = Mathf.Lerp(heavyBreathingSound.volume, 0f, Time.deltaTime * heavyBreathingVolumeChangeSpeed);

            if (heavyBreathingSound.volume < 0f + valueCuttingTreshold)
            {
                heavyBreathingSound.volume = 0f;
            }
        }

        if (isTired)
        {
            if (heavyBreathingReadyToPlay)
            {
                heavyBreathingReadyToPlay = false;

                heavyBreathingSound.Play();

                myCameraCamera.GetComponent<Animator>().Play("HeavyBreath");

                Invoke(nameof(HeavyBreathingCooldown), heavyBreathingCooldown);
            }
        }

        else switch (state)
        {
            case "Crouching":

                if (lightFootstepsReadyToPlay)
                {
                    lightFootstepsReadyToPlay = false;

                    lightFootstepsSound.Play();
                    myCameraCamera.GetComponent<Animator>().Play("LightStep");

                    Invoke(nameof(LightFootstepCooldown), lightFootstepsCooldown);
                }
                break;

            case "Sprinting":

                if (heavyFootstepsReadyToPlay)
                {
                    heavyFootstepsReadyToPlay = false;

                    heavyFootstepsSound.Play();
                    myCameraCamera.GetComponent<Animator>().Play("HeavyStep");

                    Invoke(nameof(HeavyFootstepCooldown), heavyFootstepsCooldown);
                }
                break;

            case "Walking":

                if (normalFootstepsReadyToPlay)
                {
                    normalFootstepsReadyToPlay = false;

                    normalFootstepsSound.Play();
                    myCameraCamera.GetComponent<Animator>().Play("NormalStep");

                    Invoke(nameof(NormalFootstepCooldown), normalFootstepsCooldown);
                }
                break;
        }

        if (hasJumped && !jumpSound.isPlaying)
        {
            jumpSound.Play();
            myCameraCamera.GetComponent<Animator>().Play("Jump");
        }

        if (hasLanded && !landingSound.isPlaying && !jumpSound.isPlaying)
        {
            landingSound.Play();
            myCameraCamera.GetComponent<Animator>().Play("Landing");
        }
    }

    private void ResetJump()
    {
        canJump = true;
    }

    private void ResetFlashlight()
    {
        canUseFlashlight = true;
    }

    private void ResetTierd()
    {
        isTired = false;
    }

    private void NormalFootstepCooldown()
    {
        normalFootstepsReadyToPlay = true;
    }

    private void LightFootstepCooldown()
    {
        lightFootstepsReadyToPlay = true;
    }

    private void HeavyFootstepCooldown()
    {
        heavyFootstepsReadyToPlay = true;
    }
    
    private void HeavyBreathingCooldown()
    {
        heavyBreathingReadyToPlay = true;
    }
}
