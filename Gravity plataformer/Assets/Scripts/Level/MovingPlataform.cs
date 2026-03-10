using Unity.VisualScripting;
using UnityEngine;
using static GravityController;

[RequireComponent(typeof(Rigidbody))]
public class MovingPlatform : MonoBehaviour
{
    public enum MovementAxis
    {
        X,
        Y,
        Z
    }

    [Header("Movimiento")]
    [SerializeField] private MovementAxis axis = MovementAxis.X;
    [SerializeField] private float maxDisplacement = 5f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private bool toNegative = false;
    [SerializeField] private bool invertDirection = false;

    [Header("Referencia")]
    [SerializeField] private GravityController gravityController;

    private Rigidbody rb;

    private Vector3 startPosition;
    private Vector3 targetMin;
    private Vector3 targetMax;
    private Vector3 currentTarget;

    private int collisionCount = 0;
    private bool isBlocked;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        // Configuración automática del Rigidbody
        rb.useGravity = false;
        rb.isKinematic = true;
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        if (gravityController == null)
            gravityController = FindFirstObjectByType<GravityController>();

        startPosition = transform.position;
        currentTarget = startPosition;

        CalculatePositions();

    }

    private void OnEnable()
    {
        GravityController.OnGravityChanged += UpdateTarget;
    }

    private void OnDisable()
    {
        GravityController.OnGravityChanged -= UpdateTarget;
    }

    private void OnValidate()
    {
        if (!Application.isPlaying)
            return;

        //Actualización de eje en el inspector
        startPosition = transform.position;
        CalculatePositions();

    }

    private void FixedUpdate()
    {
        MovePlatform();
    }

    private void MovePlatform()
    {
        if (isBlocked)
            return;

        Vector3 direction = (currentTarget - rb.position);
        float distance = direction.magnitude;

        if (distance < 0.05f)
            return;

        direction.Normalize();

        Vector3 newPosition =
            rb.position + direction * moveSpeed * Time.fixedDeltaTime;

        rb.MovePosition(newPosition);
    }

    private void OnCollisionEnter(Collision collision)
    {
        collisionCount++;
        isBlocked = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        collisionCount--;

        if (collisionCount <= 0)
            isBlocked = false;
    }

    private void CalculatePositions()
    {
        targetMin = startPosition;

        Vector3 offset = Vector3.zero;

        switch (axis)
        {
            case MovementAxis.X:
                offset = Vector3.right * maxDisplacement;
                break;

            case MovementAxis.Y:
                offset = Vector3.up * maxDisplacement;
                break;

            case MovementAxis.Z:
                offset = Vector3.forward * maxDisplacement;
                break;
        }

        if (invertDirection)
            targetMax = startPosition - offset;
        else
            targetMax = startPosition + offset;
    }

    private void UpdateTarget(GravityDirection gravity)
    {
        CalculatePositions();

        switch (axis)
        {
            case MovementAxis.X:
                if (gravity == GravityDirection.PosX)
                    currentTarget = toNegative ? targetMin : targetMax;
                else if (gravity == GravityDirection.NegX)
                    currentTarget = toNegative ? targetMax : targetMin;
                break;

            case MovementAxis.Y:
                if (gravity == GravityDirection.PosY)
                    currentTarget = toNegative ? targetMin : targetMax;
                else if (gravity == GravityDirection.NegY)
                    currentTarget = toNegative ? targetMax : targetMin;
                break;

            case MovementAxis.Z:
                if (gravity == GravityDirection.PosZ)
                    currentTarget = toNegative ? targetMin : targetMax;
                else if (gravity == GravityDirection.NegZ)
                    currentTarget = toNegative ? targetMax : targetMin;
                break;
        }

        isBlocked = false; // Reset del bloqueo cuando cambia la gravedad
    }

#if UNITY_EDITOR
    //dibuja el desplazamiento de la plataforma en el inspector
    private void OnDrawGizmosSelected()
    {
        Vector3 start = transform.position;
        Vector3 offset = Vector3.zero;

        switch (axis)
        {
            case MovementAxis.X:
                offset = Vector3.right * maxDisplacement;
                break;

            case MovementAxis.Y:
                offset = Vector3.up * maxDisplacement;
                break;

            case MovementAxis.Z:
                offset = Vector3.forward * maxDisplacement;
                break;
        }

        Vector3 end;

        if (invertDirection)
            end = start - offset;
        else
            end = start + offset;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(start, end);

        Gizmos.color = new Color(0f, 1f, 0f, 0.4f);
        Gizmos.DrawCube(end, transform.localScale);
    }
#endif
}