using UnityEngine;
using UnityEngine.InputSystem;
using static GravityController;

public class CompassController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform arrowPivot;
    [SerializeField] private GravityController gravityController;

    [Header("Materiales de estado")]
    [SerializeField] private Material idleMaterial;
    [SerializeField] private Material rotatingMaterial;
    [SerializeField] private Material disabledMaterial;

    private Renderer[] arrowRenderers;

    private enum CompassState { Idle, Rotating, Disabled }
    private CompassState currentState;

    [Header("Rotación libre (mouse / gamepad)")]
    [SerializeField] private float mouseSensitivity = 0.2f;
    [SerializeField] private float gamepadSensitivity = 150f;
    [SerializeField] private float smoothRotation = 15f;

    private PlayerInputAction input;

    private Vector3 currentDirection;
    private Vector3 visualDirection;

    private Vector2 lookInput;
    private Vector2 moveInput;

    private bool rotatePressed;
    private bool discreteMode;
    private bool inputConsumed;

    private void Awake()
    {
        input = new PlayerInputAction();
        arrowRenderers = arrowPivot.GetComponentsInChildren<Renderer>();
    }

    private void OnEnable() => input.Enable();
    private void OnDisable() => input.Disable();

    private void Start()
    {
        currentDirection = gravityController.CurrentGravityVector.normalized;
        visualDirection = currentDirection;

        arrowPivot.rotation = Quaternion.LookRotation(currentDirection);

        SetState(CompassState.Idle);
    }

    private void Update()
    {
        lookInput = input.Player.Look.ReadValue<Vector2>();
        moveInput = input.Player.Move.ReadValue<Vector2>();

        if (SettingsManager.InvertMouseClick)
            rotatePressed = input.Player.RotateCompassInverted.IsPressed();
        else
            rotatePressed = input.Player.RotateCompass.IsPressed();

        UpdateVisualState();

        if (rotatePressed && PlayerMovement.isOnPlatform)
        {
            if (SettingsManager.UseDiscreteCompass)
            {
                if (!discreteMode)
                {
                    discreteMode = true;
                    inputConsumed = false;
                }

                HandleDiscreteRotation();
            }
            else
            {
                RotateDirection();
            }
        }
        else
        {
            if (discreteMode)
            {
                discreteMode = false;
                SnapAndApplyGravity();
            }
            else
            {
                SnapAndApplyGravity();
            }
        }
    }

    private void LateUpdate()
    {
        visualDirection = Vector3.Lerp(visualDirection, currentDirection, smoothRotation * Time.deltaTime);
        arrowPivot.rotation = Quaternion.LookRotation(visualDirection);
    }

    private void UpdateVisualState()
    {
        if (!PlayerMovement.isOnPlatform)
            SetState(CompassState.Disabled);
        else if (rotatePressed)
            SetState(CompassState.Rotating);
        else
            SetState(CompassState.Idle);
    }

    private void SetState(CompassState newState)
    {
        if (currentState == newState) return;

        currentState = newState;

        Material mat = idleMaterial;

        switch (currentState)
        {
            case CompassState.Idle:
                mat = idleMaterial;
                break;

            case CompassState.Rotating:
                mat = rotatingMaterial;
                break;

            case CompassState.Disabled:
                mat = disabledMaterial;
                break;
        }

        ApplyMaterial(mat);
    }

    private void ApplyMaterial(Material mat)
    {
        for (int i = 0; i < arrowRenderers.Length; i++)
        {
            arrowRenderers[i].material = mat;
        }
    }

    // ------------------------------
    // ROTACIÓN LIBRE ORIGINAL
    // ------------------------------

    private void RotateDirection()
    {
        bool usingMouse = Mouse.current != null && Mouse.current.delta.ReadValue() != Vector2.zero;

        float sensitivity = usingMouse
            ? mouseSensitivity
            : gamepadSensitivity * Time.deltaTime;

        float yaw = lookInput.x * sensitivity;
        float pitch = -lookInput.y * sensitivity;

        Quaternion yawRotation = Quaternion.AngleAxis(yaw, Vector3.up);
        Quaternion pitchRotation = Quaternion.AngleAxis(pitch, Vector3.right);

        currentDirection = yawRotation * pitchRotation * currentDirection;
        currentDirection.Normalize();
    }

    // ------------------------------
    // ROTACIÓN DISCRETA WASD
    // ------------------------------

    private void HandleDiscreteRotation()
    {
        if (moveInput.magnitude < 0.5f)
        {
            inputConsumed = false;
            return;
        }

        if (inputConsumed) return;
        inputConsumed = true;

        Vector3 forward = currentDirection;

        Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;

        if (right.sqrMagnitude < 0.01f)
            right = Vector3.Cross(Vector3.forward, forward).normalized;

        Vector3 up = Vector3.Cross(forward, right).normalized;

        if (moveInput.y > 0.5f)        // W
            currentDirection = up;

        else if (moveInput.y < -0.5f)  // S
            currentDirection = -up;

        else if (moveInput.x > 0.5f)   // D
            currentDirection = right;

        else if (moveInput.x < -0.5f)  // A
            currentDirection = -right;

        currentDirection.Normalize();
    }

    // ------------------------------
    // SNAP Y APLICAR GRAVEDAD
    // ------------------------------

    private void SnapAndApplyGravity()
    {
        if (rotatePressed) return;

        GravityDirection closest = GetClosestDirection(currentDirection);

        Vector3 snapped = DirectionToVector(closest);

        currentDirection = snapped;

        gravityController.SetGravity(closest);
    }

    // ------------------------------
    // UTILIDADES
    // ------------------------------

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
}