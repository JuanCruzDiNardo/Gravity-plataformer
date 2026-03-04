using UnityEngine;
using UnityEngine.InputSystem;
using static GravityController;

public class CompassController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform arrowPivot; //Representaciopn visual
    [SerializeField] private GravityController gravityController;

    [Header("Rotación")]
    [SerializeField] private float mouseSensitivity = 0.2f;
    [SerializeField] private float gamepadSensitivity = 150f;
    [SerializeField] private float smoothRotation = 15f;

    private PlayerInputAction input;

    private Vector3 currentDirection;
    private Vector3 visualDirection;

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
        currentDirection = gravityController.CurrentGravityVector.normalized;
        visualDirection = currentDirection;
        arrowPivot.rotation = Quaternion.LookRotation(currentDirection);
    }

    private void Update()
    {
        lookInput = input.Player.Look.ReadValue<Vector2>();
        rotatePressed = input.Player.RotateCompass.IsPressed();

        if (rotatePressed)
        {
            //Seleccion de direccion
            RotateDirection();
        }
        else
        {
            //Ajuste al eje mas cercano al soltar
            SnapAndApplyGravity();
        }
    }

    private void LateUpdate()
    {
        visualDirection = Vector3.Lerp(visualDirection, currentDirection, smoothRotation * Time.deltaTime);
        arrowPivot.rotation = Quaternion.LookRotation(visualDirection);
    }

    private void RotateDirection()
    {
        //Deteccion del control
        bool usingMouse =
            Mouse.current != null &&
            Mouse.current.delta.ReadValue() != Vector2.zero;

        float sensitivity = usingMouse ? mouseSensitivity : gamepadSensitivity * Time.deltaTime;

        float yaw = lookInput.x * sensitivity;
        float pitch = -lookInput.y * sensitivity;

        //Rotaciones relativas al eje global
        Quaternion yawRotation = Quaternion.AngleAxis(yaw, Vector3.up);
        Quaternion pitchRotation = Quaternion.AngleAxis(pitch, Vector3.right);

        currentDirection = yawRotation * pitchRotation * currentDirection;
        currentDirection.Normalize();
    }

    private void SnapAndApplyGravity()
    {
        if (rotatePressed) return;
        //Detecta el eje mas cercano
        GravityDirection closest = GetClosestDirection(currentDirection);
        //Convierte el enum en un vector 3
        Vector3 snapped = DirectionToVector(closest);

        currentDirection = snapped; //Ajsute visual
        gravityController.SetGravity(closest);
    }

    private GravityDirection GetClosestDirection(Vector3 dir)
    {
        float maxDot = -Mathf.Infinity;
        GravityDirection closest = GravityDirection.PosY;

        //Se evaluan todos los ejes
        Check(Vector3.right, GravityDirection.PosX);
        Check(Vector3.left, GravityDirection.NegX);
        Check(Vector3.up, GravityDirection.PosY);
        Check(Vector3.down, GravityDirection.NegY);
        Check(Vector3.forward, GravityDirection.PosZ);
        Check(Vector3.back, GravityDirection.NegZ);

        return closest;

        void Check(Vector3 axis, GravityDirection gravityDir)
        {
            float dot = Vector3.Dot(dir, axis); //Detecta el eje con el que se alineo mas
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