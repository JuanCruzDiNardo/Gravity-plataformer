using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target; //Player
    [SerializeField] private Transform CompassCamera; 

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
    private bool rotateCompassPressed;

    private void Awake()
    {
        input = new PlayerInputAction();
        UpdateSensitivity();
    }

    private void OnEnable()
    {
        input.Enable();
        SettingsManager.OnSettingsChanged += UpdateSensitivity;
    }

    private void OnDisable()
    {
        input.Disable();
        SettingsManager.OnSettingsChanged -= UpdateSensitivity;
    }

    public void UpdateSensitivity()
    {
        mouseSensitivity = SettingsManager.MouseSensitivity;
    }

    private void Update()
    {
        // lectura de input
        lookInput = input.Player.Look.ReadValue<Vector2>();

        // determina qué acción usar según settings
        if (SettingsManager.InvertMouseClick)
            rotatePressed = input.Player.RotateCameraInverted.IsPressed();
        else
            rotatePressed = input.Player.RotateCamera.IsPressed();

        if (SettingsManager.InvertMouseClick)
            rotateCompassPressed = input.Player.RotateCompassInverted.IsPressed();
        else
            rotateCompassPressed = input.Player.RotateCompass.IsPressed();

        // si no es necesario mantener el botón, siempre rota
        if (!SettingsManager.HoldToMoveCamera)
            rotatePressed = true;

        //si mantiene el click empieza a rotar
        if (rotatePressed && !rotateCompassPressed)
        {
            //detecta el tipo de control
            bool usingMouse = Mouse.current != null && Mouse.current.delta.ReadValue() != Vector2.zero;

            if (usingMouse)
            {
                yaw += lookInput.x * mouseSensitivity;
                pitch -= lookInput.y * mouseSensitivity; //el pitch se resta para invertir el movimiento, mover el mouse hacia arriba te hace mirar hacia arriba
            }
            else
            {
                yaw += lookInput.x * gamepadSensitivity * Time.deltaTime;
                pitch -= lookInput.y * gamepadSensitivity * Time.deltaTime;
            }

            //limite de movimiento vertical
            pitch = Mathf.Clamp(pitch, -30f, 60f);
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        currentYaw = Mathf.Lerp(currentYaw, yaw, smoothRotation * Time.deltaTime);
        currentPitch = Mathf.Lerp(currentPitch, pitch, smoothRotation * Time.deltaTime);

        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);

        //calcula una orbita al rededor del jugador
        Vector3 desiredPosition =
            target.position
            - (rotation * Vector3.forward * distance)
            + Vector3.up * height;

        transform.position = desiredPosition;
        transform.LookAt(target.position + Vector3.up * 1.5f);

        CompassCamera.transform.position = desiredPosition;
        CompassCamera.LookAt(target.position + Vector3.up * 1.5f);

    }
}