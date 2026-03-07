using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform buttonTop;

    [Header("Movimiento")]
    [SerializeField] private float pressDistance = 0.2f;
    [SerializeField] private float pressSpeed = 5f;

    [Header("Objetos activables")]
    [SerializeField] private List<MonoBehaviour> activables;

    private List<IActivable> cachedActivables = new List<IActivable>();

    private Vector3 startLocalPos;
    private Vector3 pressedLocalPos;

    private bool isPressed;

    void Start()
    {
        startLocalPos = buttonTop.localPosition;

        //Se mueve en el eje local negativo de la base
        pressedLocalPos = startLocalPos + Vector3.down * pressDistance;

        foreach (var obj in activables)
        {
            if (obj is IActivable activable)
            {
                cachedActivables.Add(activable);
            }
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
        if (other.gameObject.CompareTag("Player"))
        {
            isPressed = true;
            ActivateObjects();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPressed = false;
            ActivateObjects();
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