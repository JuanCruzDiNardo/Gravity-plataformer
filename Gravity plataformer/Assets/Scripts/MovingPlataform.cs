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

    [Header("Referencia")]
    [SerializeField] private GravityController gravityController;

    private Rigidbody rb;

    private Vector3 startPosition;
    private Vector3 targetMin;
    private Vector3 targetMax;
    private Vector3 currentTarget;

    private bool isBlocked;

    // =========================
    // INITIALIZATION
    // =========================

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        // Configuración automática del Rigidbody
        rb.useGravity = false;
        rb.isKinematic = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        if (gravityController == null)
            gravityController = FindFirstObjectByType<GravityController>();

        startPosition = transform.position;
        currentTarget = startPosition;

        CalculatePositions();

    }

    // =========================
    // EVENT SUBSCRIPTION
    // =========================

    private void OnEnable()
    {
        GravityController.OnGravityChanged += UpdateTarget;
    }

    private void OnDisable()
    {
        GravityController.OnGravityChanged -= UpdateTarget;
    }

    // =========================
    // EDITOR LIVE UPDATE
    // =========================

    private void OnValidate()
    {
        if (!Application.isPlaying)
            return;

        startPosition = transform.position;
        CalculatePositions();

    }

    // =========================
    // PHYSICS UPDATE
    // =========================

    private void FixedUpdate()
    {
        MovePlatform();
    }

    private void MovePlatform()
    {
        if (isBlocked)
            return;

        Vector3 newPosition = Vector3.MoveTowards(
            rb.position,
            currentTarget,
            moveSpeed * Time.fixedDeltaTime
        );

        rb.MovePosition(newPosition);
    }

    // =========================
    // COLLISION CONTROL
    // =========================

    private void OnCollisionEnter(Collision collision)
    {
        isBlocked = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        isBlocked = false;
    }

    // =========================
    // POSITION CALCULATION
    // =========================

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

        targetMax = startPosition + offset;
    }

    // =========================
    // GRAVITY REACTION
    // =========================

    private void UpdateTarget(GravityDirection gravity)
    {
        CalculatePositions();

        switch (axis)
        {
            case MovementAxis.X:
                if (gravity == GravityDirection.PosX)
                    currentTarget = targetMax;
                else if (gravity == GravityDirection.NegX)
                    currentTarget = targetMin;
                break;

            case MovementAxis.Y:
                if (gravity == GravityDirection.PosY)
                    currentTarget = targetMax;
                else if (gravity == GravityDirection.NegY)
                    currentTarget = targetMin;
                break;

            case MovementAxis.Z:
                if (gravity == GravityDirection.PosZ)
                    currentTarget = targetMax;
                else if (gravity == GravityDirection.NegZ)
                    currentTarget = targetMin;
                break;
        }

        isBlocked = false; // reset bloqueo cuando cambia gravedad
    }
}