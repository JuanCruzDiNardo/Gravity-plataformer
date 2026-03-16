using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonController : MonoBehaviour
{
    public enum ButtonMode
    {
        Pressure,   //Funciona mientras se pisa
        Toggle      //Interruptor
    }

    [Header("Modo")]
    [SerializeField] private ButtonMode buttonMode = ButtonMode.Pressure;

    [Header("Referencias")]
    [SerializeField] private Transform buttonTop;

    [Header("Movimiento")]
    [SerializeField] private float pressDistance = 0.2f;
    [SerializeField] private float pressSpeed = 5f;

    [Header("Objetos activables")]
    [SerializeField] private List<MonoBehaviour> activables;
    
    public static event Action OnPressed;
    public static event Action OnReleased;

    private List<IActivable> cachedActivables = new List<IActivable>();

    private Vector3 startLocalPos;
    private Vector3 pressedLocalPos;

    private bool isPressed;    

    void Start()
    {
        startLocalPos = buttonTop.localPosition;
        pressedLocalPos = startLocalPos + Vector3.down * pressDistance;

        foreach (var obj in activables)
        {
            if (obj is IActivable activable)
                cachedActivables.Add(activable);
        }
    }

    private void ActivateObjects()
    {
        foreach (var activable in cachedActivables)
        {
            activable.Switch();
        }
    }

    void Update()
    {
        Vector3 target = isPressed ? pressedLocalPos : startLocalPos;

        buttonTop.localPosition = Vector3.Lerp(
            buttonTop.localPosition,
            target,
            pressSpeed * Time.deltaTime
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;        

        if (buttonMode == ButtonMode.Pressure)
        {
            if (!isPressed)
            {
                isPressed = true;
                ActivateObjects();
                OnPressed?.Invoke();
            }
        }
        else // Toggle
        {
            isPressed = !isPressed;
            ActivateObjects();

            if (isPressed)
                OnPressed?.Invoke();
            else
                OnReleased?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (buttonMode == ButtonMode.Pressure)
        {
            if (isPressed)
            {
                isPressed = false;
                ActivateObjects();
                OnReleased?.Invoke();
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (activables == null) return;

        Gizmos.color = new Color(1f, 1f, 0f, 0.4f);

        foreach (var obj in activables)
        {
            if (obj == null) continue;

            Gizmos.DrawLine(transform.position, obj.transform.position);
            Gizmos.DrawSphere(obj.transform.position, 0.1f);
        }
    }
#endif
}