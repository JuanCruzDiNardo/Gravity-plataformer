using UnityEngine;
using UnityEngine.InputSystem;
using static GravityController;

public class GravityCompassController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform arrowPivot;
    [SerializeField] private GravityController gravityController;

    [Header("Rotación")]
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float gamepadSensitivity = 100f;
    [SerializeField] private float smoothRotation = 15f;
    [SerializeField] private float verticalLimit = 89f;

    private PlayerInputAction input;

    private float yaw;
    private float pitch;

    private float currentYaw;
    private float currentPitch;

    private Vector2 lookInput;
    private bool rotatePressed;

    private void Awake()
    {
        input = new PlayerInputAction();
    }

    private void OnEnable() => input.Enable();
    private void OnDisable() => input.Disable();

    private void Start()
    {
        AlignWithCurrentGravity();
    }

    private void Update()
    {
        lookInput = input.Player.Look.ReadValue<Vector2>();
        rotatePressed = input.Player.RotateCompass.IsPressed();

        if (rotatePressed)
        {
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
            // Al soltar → snap
            if (yaw != currentYaw || pitch != currentPitch)
            {
                SnapAndApplyGravity();
            }
        }
    }

    private void LateUpdate()
    {
        currentYaw = Mathf.Lerp(currentYaw, yaw, smoothRotation * Time.deltaTime);
        currentPitch = Mathf.Lerp(currentPitch, pitch, smoothRotation * Time.deltaTime);

        arrowPivot.localRotation =
            Quaternion.Euler(currentPitch, currentYaw, 0f);
    }

    // ==============================
    // SNAP A 6 DIRECCIONES
    // ==============================

    private void SnapAndApplyGravity()
    {
        Vector3 dir = arrowPivot.forward.normalized;

        GravityDirection closest = GetClosestDirection(dir);
        Vector3 snappedVector = DirectionToVector(closest);

        Quaternion snappedRotation = Quaternion.LookRotation(snappedVector);
        Vector3 snappedEuler = snappedRotation.eulerAngles;

        yaw = snappedEuler.y;
        pitch = NormalizeAngle(snappedEuler.x);

        gravityController.SetGravity(closest);
    }

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

    private void AlignWithCurrentGravity()
    {
        Vector3 currentGravity =
            gravityController.CurrentGravityVector.normalized;

        Quaternion rot = Quaternion.LookRotation(currentGravity);
        Vector3 euler = rot.eulerAngles;

        yaw = euler.y;
        pitch = NormalizeAngle(euler.x);

        currentYaw = yaw;
        currentPitch = pitch;

        arrowPivot.localRotation = rot;
    }

    private float NormalizeAngle(float angle)
    {
        if (angle > 180f)
            angle -= 360f;

        return angle;
    }
}