using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Renderer))]
public class IntangiblePlatform : MonoBehaviour, IActivable
{
    [Header("Visual")]
    [Range(0f, 1f)]
    [SerializeField] private float intangibleAlpha = 0.3f;

    private Collider platformCollider;
    private Renderer platformRenderer;
    private Material runtimeMaterial;

    private bool isIntangible = false;
    private float originalAlpha;

    private void Awake()
    {
        platformCollider = GetComponent<Collider>();
        platformRenderer = GetComponent<Renderer>();

        // Instanciamos material para no modificar el sharedMaterial
        runtimeMaterial = platformRenderer.material;

        originalAlpha = runtimeMaterial.color.a;

        SetSolidState();
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

        SetAlpha(intangibleAlpha);
    }

    public void SetSolidState()
    {
        isIntangible = false;

        platformCollider.enabled = true;

        SetAlpha(originalAlpha);
    }

    private void SetAlpha(float alpha)
    {
        Color color = runtimeMaterial.color;
        color.a = alpha;
        runtimeMaterial.color = color;
    }

    public void Switch()
    {
        ToggleState();
    }
}