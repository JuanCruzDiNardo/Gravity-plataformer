using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Settings")]
    [SerializeField] private float distance = 6f;
    [SerializeField] private float height = 2f;

    [Header("Rotation")]
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float gamepadSensitivity = 100f;
    [SerializeField] private float smoothRotation = 10f;

    private PlayerInputAction input;

    private float yaw;
    private float pitch = 20f;

    private float currentYaw;
    private float currentPitch;

    private Vector2 lookInput;
    private bool rotatePressed;

    private void Awake()
    {
        input = new PlayerInputAction();
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    private void Update()
    {
        lookInput = input.Player.Look.ReadValue<Vector2>();
        rotatePressed = input.Player.RotateCamera.IsPressed();

        if (rotatePressed)
        {
            bool usingMouse = Mouse.current != null && Mouse.current.delta.ReadValue() != Vector2.zero;

            if (usingMouse)
            {
                yaw += lookInput.x * mouseSensitivity;
                pitch -= lookInput.y * mouseSensitivity;
            }
            else
            {
                yaw += lookInput.x * gamepadSensitivity * Time.deltaTime;
                pitch -= lookInput.y * gamepadSensitivity * Time.deltaTime;
            }

            pitch = Mathf.Clamp(pitch, -30f, 60f);
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        currentYaw = Mathf.Lerp(currentYaw, yaw, smoothRotation * Time.deltaTime);
        currentPitch = Mathf.Lerp(currentPitch, pitch, smoothRotation * Time.deltaTime);

        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);

        Vector3 desiredPosition =
            target.position
            - (rotation * Vector3.forward * distance)
            + Vector3.up * height;

        transform.position = desiredPosition;
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}