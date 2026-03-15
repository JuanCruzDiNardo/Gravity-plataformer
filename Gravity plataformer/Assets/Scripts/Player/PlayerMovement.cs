using System;
using System.Linq;
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
    [SerializeField] public static bool isOnPlatform;
    public bool IsOnPlatform => isOnPlatform;
    private int plataformCount;

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
    
    void Update()
    {
        moveInput = action.Player.Move.ReadValue<Vector2>();        
    }

    private void FixedUpdate()
    {
        //if (!isOnPlatform) return;

        MovePlayer();
    }

    private void MovePlayer()
    {
        if (action.Player.RotateCompass.IsPressed() && SettingsManager.UseDiscreteCompass) return;

        Vector3 gravityDir = gravity.CurrentGravityVector.normalized;
        Vector3 targetVelocity;

        bool gravityOnY = Mathf.Abs(Vector3.Dot(gravityDir, Vector3.up)) > 0.9f;

        if (gravityOnY) //Si la gravedad esta en Y, el movimiento es relativoa  la camara
        {
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;

            camForward = Vector3.ProjectOnPlane(camForward, gravityDir).normalized;
            camRight = Vector3.ProjectOnPlane(camRight, gravityDir).normalized;

            targetVelocity =
                (camRight * moveInput.x + camForward * moveInput.y) * moveSpeed;
        }
        else //Si la gravedad esa en X o Z, los movimientos de arriba y abajo son fijos, izquierda y derecha relativos a la camara 
        {
            // Movimiento vertical fijo en Y
            Vector3 vertical = Vector3.up * moveInput.y;

            // Horizontal relativo a cámara 
            Vector3 camRight = cameraTransform.right;
            camRight = Vector3.ProjectOnPlane(camRight, gravityDir).normalized;

            targetVelocity =
                (camRight * moveInput.x + vertical) * moveSpeed;
        }

        Vector3 currentVelocity = rb.linearVelocity;
        Vector3 lateralVelocity = Vector3.ProjectOnPlane(currentVelocity, gravityDir);

        //Movimiento
        Vector3 velocityChange = targetVelocity - lateralVelocity;

        //Aceleracion gradual
        velocityChange = Vector3.ClampMagnitude(
            velocityChange,
            acceleration * Time.fixedDeltaTime
        );

        rb.linearVelocity += velocityChange;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            isOnPlatform = true;
            plataformCount++;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            plataformCount--;

            if (plataformCount == 0)
                isOnPlatform = false;
        }
    }
}
