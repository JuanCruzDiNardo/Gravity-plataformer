using UnityEngine;
using UnityEngine.InputSystem;
using static GravityController;

public class GravityCompassController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform arrowPivot;
    [SerializeField] private GravityController gravityController;
    [SerializeField] private Transform cameraTransform;

    [Header("Rotación")]
    [SerializeField] private float mouseSensitivity = 0.15f;
    [SerializeField] private float gamepadSensitivity = 120f;
    [SerializeField] private float smoothRotation = 15f;
    [SerializeField] private float verticalLimit = 89f;

    private PlayerInputAction input;

    private float yaw;
    private float pitch;

    private float currentYaw;
    private float currentPitch;

    private Vector2 lookInput;
    private bool rotatePressed;
    private bool wasRotating;

    private void Awake()
    {
        input = new PlayerInputAction();
    }

    private void OnEnable() => input.Enable();
    private void OnDisable() => input.Disable();

    private void Update()
    {
        lookInput = input.Player.Look.ReadValue<Vector2>();
        rotatePressed = input.Player.RotateCompass.IsPressed();

        // ================================
        // MODO EDICIÓN (click derecho)
        // ================================
        if (rotatePressed)
        {
            if (!wasRotating)
            {
                BeginManualRotation();
            }

            bool usingMouse =
                Mouse.current != null &&
                Mouse.current.delta.ReadValue() != Vector2.zero;

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

            pitch = Mathf.Clamp(pitch, -verticalLimit, verticalLimit);
        }
        else
        {
            // Si acabamos de soltar click derecho → aplicar gravedad
            if (wasRotating)
            {
                SnapAndApplyGravity();
            }

            // Mientras NO se edita → seguir gravedad real
            FollowCurrentGravity();
        }

        wasRotating = rotatePressed;
    }

    private void LateUpdate()
    {
        currentYaw = Mathf.Lerp(currentYaw, yaw, smoothRotation * Time.deltaTime);
        currentPitch = Mathf.Lerp(currentPitch, pitch, smoothRotation * Time.deltaTime);

        arrowPivot.localRotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
    }

    // =====================================================
    // MODO NORMAL: siempre apuntar a la gravedad real
    // =====================================================

    private void FollowCurrentGravity()
    {
        Vector3 gravityWorld = gravityController.CurrentGravityVector.normalized;

        // Convertimos gravedad mundo a espacio cámara
        Vector3 gravityCameraSpace =
            cameraTransform.InverseTransformDirection(gravityWorld);

        Quaternion rot = Quaternion.LookRotation(gravityCameraSpace);

        Vector3 euler = rot.eulerAngles;

        yaw = euler.y;
        pitch = NormalizeAngle(euler.x);
    }

    // =====================================================
    // INICIO ROTACIÓN MANUAL
    // =====================================================

    private void BeginManualRotation()
    {
        Vector3 gravityWorld = gravityController.CurrentGravityVector.normalized;

        Vector3 gravityCameraSpace =
            cameraTransform.InverseTransformDirection(gravityWorld);

        Quaternion rot = Quaternion.LookRotation(gravityCameraSpace);

        Vector3 euler = rot.eulerAngles;

        yaw = euler.y;
        pitch = NormalizeAngle(euler.x);

        currentYaw = yaw;
        currentPitch = pitch;
    }

    // =====================================================
    // SNAP Y APLICAR GRAVEDAD
    // =====================================================

    private void SnapAndApplyGravity()
    {
        // Dirección actual en espacio cámara
        Vector3 cameraDir = Quaternion.Euler(pitch, yaw, 0f) * Vector3.forward;

        // Convertir a mundo
        Vector3 worldDir =
            cameraTransform.TransformDirection(cameraDir).normalized;

        GravityDirection closest = GetClosestDirection(worldDir);
        Vector3 snappedVector = DirectionToVector(closest);

        gravityController.SetGravity(closest);

        // Re-alinear inmediatamente
        FollowCurrentGravity();
    }

    // =====================================================

    private GravityDirection GetClosestDirection(Vector3 dir)
    {
        float maxDot = -Mathf.Infinity;
        GravityDirection closest = GravityDirection.PosY;

        Check(Vector3.right, GravityDirection.PosX);
        Check(Vector3.left, GravityDirection.NegX);
        Check(Vector3.up, GravityDirection.PosY);
        Check(Vector3.down, GravityDirection.NegY);
        Check(Vector3.forward, GravityDirection.PosZ);
        Check(Vector3.back, GravityDirection.NegZ);

        return closest;

        void Check(Vector3 axis, GravityDirection gravityDir)
        {
            float dot = Vector3.Dot(dir, axis);
            if (dot > maxDot)
            {
                maxDot = dot;
                closest = gravityDir;
            }
        }
    }

    private Vector3 DirectionToVector(GravityDirection direction)
    {
        switch (direction)
        {
            case GravityDirection.PosX: return Vector3.right;
            case GravityDirection.NegX: return Vector3.left;
            case GravityDirection.PosY: return Vector3.up;
            case GravityDirection.NegY: return Vector3.down;
            case GravityDirection.PosZ: return Vector3.forward;
            case GravityDirection.NegZ: return Vector3.back;
        }

        return Vector3.down;
    }

    private float NormalizeAngle(float angle)
    {
        if (angle > 180f)
            angle -= 360f;

        return angle;
    }
}