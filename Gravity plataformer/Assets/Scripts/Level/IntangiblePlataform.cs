using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Renderer))]
public class IntangiblePlatform : MonoBehaviour, IActivable
{
    [Header("Materiales")]
    [SerializeField] private Material solidMaterial;
    [SerializeField] private Material intangibleMaterial;
    [SerializeField] private bool startIntenngible = false;
    [SerializeField] private bool isIntangible = false;

    private Collider platformCollider;
    private Renderer platformRenderer;    

    private void Awake()
    {
        platformCollider = GetComponent<Collider>();
        platformRenderer = GetComponent<Renderer>();

        SetSolidState();
        if (startIntenngible)
            ToggleState();
    }

    public void ToggleState()
    {
        if (isIntangible)
            SetSolidState();
        else
            SetIntangibleState();
    }

    public void SetIntangibleState()
    {
        isIntangible = true;

        platformCollider.enabled = false;

        if (intangibleMaterial != null)
            platformRenderer.material = intangibleMaterial;
    }

    public void SetSolidState()
    {
        isIntangible = false;

        platformCollider.enabled = true;

        if (solidMaterial != null)
            platformRenderer.material = solidMaterial;
    }

    public void Switch()
    {
        ToggleState();
    }
}