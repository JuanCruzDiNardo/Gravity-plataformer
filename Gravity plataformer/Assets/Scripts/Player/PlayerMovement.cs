using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

[RequireComponent(typeof(Rigidbody))]  
public class PlayerMovement : MonoBehaviour
{
    private PlayerInputAction action;
    private Vector2 moveInput;
    private Rigidbody rb;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 15f;
    [SerializeField] private Transform cameraTransform;

    private GravityController gravity;
    private void Awake()
    {
        action = new PlayerInputAction();
        rb = GetComponent<Rigidbody>();
        gravity = GetComponent<GravityController>();

        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void OnEnable()
    {
        action.Enable();
    }

    private void OnDisable()
    {        
        action.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = action.Player.Move.ReadValue<Vector2>();
        CheckResetInput();
    }

    private void CheckResetInput()
    {
        if (action.Player.Reset.WasPressedThisFrame())
        {
           GameManager.ResetLevel();
        }
    }


    private void FixedUpdate()
    {
        Vector3 gravityDir = gravity.CurrentGravityVector.normalized;
        Vector3 targetVelocity;

        bool gravityOnY = Mathf.Abs(Vector3.Dot(gravityDir, Vector3.up)) > 0.9f;

        if (gravityOnY)
        {
            // Movimiento normal relativo a cámara
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;

            camForward = Vector3.ProjectOnPlane(camForward, gravityDir).normalized;
            camRight = Vector3.ProjectOnPlane(camRight, gravityDir).normalized;

            targetVelocity =
                (camRight * moveInput.x + camForward * moveInput.y) * moveSpeed;
        }
        else
        {
            // Movimiento vertical fijo en Y
            Vector3 vertical = Vector3.up * moveInput.y;

            // Horizontal relativo a cámara pero proyectado en plano correcto
            Vector3 camRight = cameraTransform.right;
            camRight = Vector3.ProjectOnPlane(camRight, gravityDir).normalized;

            targetVelocity =
                (camRight * moveInput.x + vertical) * moveSpeed;
        }

        Vector3 currentVelocity = rb.linearVelocity;
        Vector3 lateralVelocity = Vector3.ProjectOnPlane(currentVelocity, gravityDir);

        Vector3 velocityChange = targetVelocity - lateralVelocity;

        velocityChange = Vector3.ClampMagnitude(
            velocityChange,
            acceleration * Time.fixedDeltaTime
        );

        rb.linearVelocity += velocityChange;
    }
}
