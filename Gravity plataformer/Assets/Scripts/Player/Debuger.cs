using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(GravityController))]
public class Debuger : MonoBehaviour
{
    private GravityController gravity;

    private void Awake()
    {
        gravity = GetComponent<GravityController>();
    }

    private void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            gravity.SetGravity(GravityController.GravityDirection.PosX);
            Debug.Log("Gravity: +X");
        }

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            gravity.SetGravity(GravityController.GravityDirection.NegX);
            Debug.Log("Gravity: -X");
        }

        if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            gravity.SetGravity(GravityController.GravityDirection.PosY);
            Debug.Log("Gravity: +Y");
        }

        if (Keyboard.current.digit4Key.wasPressedThisFrame)
        {
            gravity.SetGravity(GravityController.GravityDirection.NegY);
            Debug.Log("Gravity: -Y");
        }

        if (Keyboard.current.digit5Key.wasPressedThisFrame)
        {
            gravity.SetGravity(GravityController.GravityDirection.PosZ);
            Debug.Log("Gravity: +Z");
        }

        if (Keyboard.current.digit6Key.wasPressedThisFrame)
        {
            gravity.SetGravity(GravityController.GravityDirection.NegZ);
            Debug.Log("Gravity: -Z");
        }
    }
}