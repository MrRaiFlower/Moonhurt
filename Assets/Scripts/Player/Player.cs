using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Gameobject References")]
    public GameObject myCameraGameObject;
    public Camera myCameraCamera;
    public GameObject flashlight;

    [Header("Component References")]
    public CharacterController myCharacterController;
    public CapsuleCollider myCapsuleCollider;

    [Header("Audio Sources")]
    public AudioSource normalFootstepsSound;
    public AudioSource lightFootstepsSound;
    public AudioSource heavyFootstepsSound;
    public AudioSource jumpSound;
    public AudioSource groundingSound;
    public AudioSource heavyBreathingSound;
    public AudioSource flashlightTurnOnSound;
    public AudioSource flashlightTurnOffSound;

    [Header("Raycast")]
    public float groundCheckRadius;
    public int groundCheckRays;
    public float ceilingCheckRadius;
    public int ceilingCheckRays;

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

    [Header("Objects Interaction")]
    public float objectInteractionDistance;
    public LayerMask interactableObjectsLayerMask;
    public string doorLayerName;
    public string coinsPileLayerName;
    public string doubleDoorsSpecialLayerName;

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

    // Raycast
    private RaycastHit raycastHitInfo;

    // Ground and Ceiling Check
    [HideInInspector] public bool isGrounded;
    private bool wasGrounded;
    [HideInInspector] public bool hasGrounded;
    [HideInInspector] public bool touchesCeiling;
    private bool touchedCeiling;
    [HideInInspector] public bool hasTouchedCeiling;

    // State
    [HideInInspector] public string state;
    [HideInInspector] public float fallingTime;

    // Stamina
    private float stamina;
    private bool isTired;

    // Jump
    private bool canJump;
    [HideInInspector] public bool hasJumped;

    // Flashlight
    private bool flashlightIsOn;
    private bool canUseFlashlight;

    // Crouching
    private float lerpedHeightValue;

    // Objects Interaction
    [HideInInspector] public bool hasInteractedWithDoor;

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
    private InputAction interactAction;
    private InputAction flashlightAction;

    void Start()
    {
        // Setup

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        cameraRotationY = 180f;

        myCharacterController.height = normalHeight;
        myCharacterController.center = Vector3.up * (normalHeight / 2f);

        myCapsuleCollider.height = normalHeight;
        myCapsuleCollider.center = Vector3.up * (normalHeight / 2f);

        myCameraGameObject.transform.localPosition = Vector3.up * (cameraPositionY * myCharacterController.height) + new Vector3(myCameraGameObject.transform.localPosition.x, 0f, myCameraGameObject.transform.localPosition.z);

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
        interactAction = InputSystem.actions.FindAction("Interact");
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

        isMoving = inputMoveDirection.magnitude != 0;

        if (isMoving)
        {
            moveDirection = this.gameObject.transform.right * inputMoveDirection.x + this.gameObject.transform.forward * inputMoveDirection.y;
        }

        if (moveDirection.magnitude > 1)
        {
            moveDirection.Normalize();
        }

        // Ground Check

        wasGrounded = isGrounded;

        isGrounded = false;

        for (float dz = -1f * groundCheckRadius; dz <= groundCheckRadius; dz += (2f * groundCheckRadius) / (float)(groundCheckRays - 1))
        {
            for (float dx = -1f * groundCheckRadius; dx <= groundCheckRadius; dx += (2f * groundCheckRadius) / (float)(groundCheckRays - 1))
            {
                if (new Vector2(dx, dz).magnitude <= groundCheckRadius)
                {
                    // Debug.DrawRay(this.gameObject.transform.position + new Vector3(dx, -1f * Mathf.Sqrt(0.25f - Mathf.Pow(new Vector2(dx, dz).magnitude, 2)) + 0.5f, dz), Vector3.down * 0.1f, Color.green);

                    if (Physics.Raycast(this.gameObject.transform.position + new Vector3(dx, -1f * Mathf.Sqrt(0.25f - Mathf.Pow(new Vector2(dx, dz).magnitude, 2)) + 0.5f, dz), Vector3.down, out raycastHitInfo, 0.1f))
                    {
                        isGrounded = true;
                        goto skipLaterGroundCheck;
                    }
                }
            }
        }

        skipLaterGroundCheck:

        hasGrounded = false;

        if (!wasGrounded && isGrounded)
        {
            hasGrounded = true;
        }

        // Ceiling Check

        touchedCeiling = touchesCeiling;

        touchesCeiling = false;

        for (float dz = -1 * ceilingCheckRadius; dz <= ceilingCheckRadius; dz += (2f * ceilingCheckRadius) / (float)(groundCheckRays - 1))
        {
            for (float dx = -1 * ceilingCheckRadius; dx <= ceilingCheckRadius; dx += (2f * ceilingCheckRadius) / (float)(groundCheckRays - 1))
            {
                if (new Vector2(dx, dz).magnitude <= ceilingCheckRadius)
                {
                    // Debug.DrawRay(this.gameObject.transform.position + new Vector3(dx, Mathf.Sqrt(0.25f - Mathf.Pow(new Vector2(dx, dz).magnitude, 2)) - 0.5f + myCharacterController.height, dz), Vector3.up * 0.1f, Color.green);

                    if (Physics.Raycast(this.gameObject.transform.position + new Vector3(dx, Mathf.Sqrt(0.25f - Mathf.Pow(new Vector2(dx, dz).magnitude, 2)) - 0.5f + myCharacterController.height, dz), Vector3.up, out raycastHitInfo, 0.1f))
                    {
                        touchesCeiling = true;
                        goto skipLaterCeilingCheck;
                    }
                }
            }
        }

    skipLaterCeilingCheck:

        hasTouchedCeiling = false;

        if (!touchedCeiling && touchesCeiling)
        {
            hasTouchedCeiling = true;
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

            Invoke(nameof(ResetTired), restingCooldown);
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
        else if (hasTouchedCeiling)
        {
            verticalVelocity = -9.81f * Time.deltaTime;
        }
        else
        {
            verticalVelocity += -9.81f * Time.deltaTime;
        }

        // Jump

        hasJumped = false;

        if (jumpAction.IsPressed() && isGrounded && canJump && stamina > 0f && !isTired && !crouchAction.IsPressed())
        {
            hasJumped = true;
            canJump = false;

            verticalVelocity = Mathf.Sqrt(jumpHeight * 9.81f);

            Invoke(nameof(ResetJump), 0.13f);
        }

        // Height Control

        if (crouchAction.IsPressed() && myCharacterController.height != crouchHeight && isGrounded)
        {
            lerpedHeightValue = Mathf.Lerp(myCharacterController.height, crouchHeight, Time.deltaTime * heightChangeSpeed);

            myCharacterController.height = lerpedHeightValue;
            myCharacterController.center = Vector3.up * (myCharacterController.height / 2f);

            myCapsuleCollider.height = lerpedHeightValue;
            myCapsuleCollider.center = Vector3.up * (myCapsuleCollider.height / 2f);

            if (myCharacterController.height < crouchHeight + valueCuttingTreshold)
            {
                myCharacterController.height = crouchHeight;
                myCharacterController.center = Vector3.up * (myCharacterController.height / 2f);

                myCapsuleCollider.height = crouchHeight;
                myCapsuleCollider.center = Vector3.up * (myCapsuleCollider.height / 2f);
            }

            myCameraGameObject.transform.localPosition = Vector3.up * (cameraPositionY * myCharacterController.height) + new Vector3(myCameraGameObject.transform.localPosition.x, 0f, myCameraGameObject.transform.localPosition.z);
        }

        if (!crouchAction.IsPressed() && myCharacterController.height != normalHeight && !touchesCeiling)
        {
            lerpedHeightValue = Mathf.Lerp(myCharacterController.height, normalHeight, Time.deltaTime * heightChangeSpeed);

            myCharacterController.height = lerpedHeightValue;
            myCharacterController.center = Vector3.up * (myCharacterController.height / 2f);

            myCapsuleCollider.height = lerpedHeightValue;
            myCapsuleCollider.center = Vector3.up * (myCapsuleCollider.height / 2f);

            if (myCharacterController.height > normalHeight - valueCuttingTreshold)
            {
                myCharacterController.height = normalHeight;
                myCharacterController.center = Vector3.up * (myCharacterController.height / 2f);

                myCapsuleCollider.height = normalHeight;
                myCapsuleCollider.center = Vector3.up * (myCapsuleCollider.height / 2f);
            }

            myCameraGameObject.transform.localPosition = Vector3.up * (cameraPositionY * myCharacterController.height) + new Vector3(myCameraGameObject.transform.localPosition.x, 0f, myCameraGameObject.transform.localPosition.z);
        }

        // Move

        myCharacterController.Move(moveDirection * speed * Time.deltaTime + Vector3.up * verticalVelocity * Time.deltaTime);

        // Objects Interaction

        hasInteractedWithDoor = false;

        if (interactAction.WasPressedThisFrame())
        {
            if (Physics.Raycast(myCameraGameObject.transform.position, myCameraGameObject.transform.TransformDirection(Vector3.forward), out raycastHitInfo, objectInteractionDistance, interactableObjectsLayerMask, QueryTriggerInteraction.Ignore))
            {
                if (raycastHitInfo.transform.gameObject.layer == LayerMask.NameToLayer(doorLayerName))
                {
                    if (raycastHitInfo.transform.gameObject.GetComponentInParent<Door>().readyToChangeState)
                    {
                        raycastHitInfo.transform.gameObject.GetComponentInParent<Door>().ChangeState();
                        hasInteractedWithDoor = true;
                    }
                }

                if (raycastHitInfo.transform.gameObject.layer == LayerMask.NameToLayer(coinsPileLayerName))
                {
                    raycastHitInfo.transform.gameObject.GetComponent<CoinsPile>().Collect();
                }
                
                if (raycastHitInfo.transform.gameObject.layer == LayerMask.NameToLayer(doubleDoorsSpecialLayerName))
                {
                    if (raycastHitInfo.transform.gameObject.GetComponentInParent<DoubleDoorsSpecial>().readyToChangeState && raycastHitInfo.transform.gameObject.GetComponentInParent<DoubleDoorsSpecial>().isInteractable)
                    {
                        raycastHitInfo.transform.gameObject.GetComponentInParent<DoubleDoorsSpecial>().ChangeState();
                        hasInteractedWithDoor = true;
                    }
                }
            }
        }

        // Sound and Animation

            if (hasGrounded && fallingTime > 0.12f)
            {
                normalFootstepsReadyToPlay = false;
                Invoke(nameof(NormalFootstepCooldown), normalFootstepsCooldown);

                lightFootstepsReadyToPlay = false;
                Invoke(nameof(LightFootstepCooldown), lightFootstepsCooldown);

                heavyFootstepsReadyToPlay = false;
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

        if (hasGrounded && !groundingSound.isPlaying && !jumpSound.isPlaying && fallingTime > 0.12f)
        {
            groundingSound.Play();
            myCameraCamera.GetComponent<Animator>().Play("Grounding");
        }
        
        if (state == "Falling")
        {
            fallingTime += Time.deltaTime;
        }
        else
        {
            fallingTime = 0f;
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

    private void ResetTired()
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
