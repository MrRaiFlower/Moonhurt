using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControl : MonoBehaviour
{
    public float mouseSensitivity;

    private float cursorMovementX;
    private float cursorMovementY;
    private float cameraRotationX;
    private float cameraRotationY;

    private InputAction lookAction;

    void Start()
    {
        // Setup

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        this.gameObject.transform.localPosition = Vector3.up * (0.75f * this.gameObject.transform.parent.GetComponent<CapsuleCollider>().height);

        lookAction = InputSystem.actions.FindAction("Look");
    }

    void Update()
    {
        // Rotation Inputs

        cursorMovementX = lookAction.ReadValue<Vector2>().x;
        cursorMovementY = lookAction.ReadValue<Vector2>().y;

        cameraRotationY += cursorMovementX * Time.deltaTime * mouseSensitivity;

        cameraRotationX -= cursorMovementY * Time.deltaTime * mouseSensitivity;
        cameraRotationX = Mathf.Clamp(cameraRotationX, -90f, 90f);

        // Rotation

        this.gameObject.transform.localRotation = Quaternion.Euler(cameraRotationX, 0f, 0f);
        this.gameObject.transform.parent.transform.rotation = Quaternion.Euler(0f,cameraRotationY, 0f);

        // Vertical Position

        this.gameObject.transform.localPosition = Vector3.up * (0.75f * this.gameObject.transform.parent.GetComponent<CapsuleCollider>().height);
    }
}
