using UnityEngine;

public class GravityController : MonoBehaviour
{
    public enum GravityDirection
    {
        PosX,
        NegX,
        PosY,
        NegY,
        PosZ,
        NegZ
    }

    [Header("Gravity")]
    [SerializeField] private GravityDirection currentDirection = GravityDirection.NegY;
    [SerializeField] private float gravityStrength = 20f;

    private Rigidbody rb;

    public Vector3 CurrentGravityVector { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; // Desactivamos gravedad default
        UpdateGravityVector();
    }

    private void FixedUpdate()
    {
        //Efecto de gravedad direccional
        rb.AddForce(CurrentGravityVector * gravityStrength, ForceMode.Acceleration);
    }

    public void SetGravity(GravityDirection newDirection)
    {
        currentDirection = newDirection;
        UpdateGravityVector();
    }

    private void UpdateGravityVector()
    {
        switch (currentDirection)
        {
            case GravityDirection.PosX: CurrentGravityVector = Vector3.right; break;
            case GravityDirection.NegX: CurrentGravityVector = Vector3.left; break;
            case GravityDirection.PosY: CurrentGravityVector = Vector3.up; break;
            case GravityDirection.NegY: CurrentGravityVector = Vector3.down; break;
            case GravityDirection.PosZ: CurrentGravityVector = Vector3.forward; break;
            case GravityDirection.NegZ: CurrentGravityVector = Vector3.back; break;
        }
    }
}