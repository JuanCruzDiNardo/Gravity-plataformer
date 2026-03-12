using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Renderer))]
public class IntangiblePlatform : MonoBehaviour, IActivable
{
    [Header("Visual")]
    [Range(0f, 1f)]
    [SerializeField] private float intangibleOpacity = 0.3f;

    [SerializeField] private float solidOpacity = 1f;

    private Collider platformCollider;
    private Renderer platformRenderer;

    private MaterialPropertyBlock propBlock;

    private bool isIntangible = false;

    private static readonly int OpacityID = Shader.PropertyToID("_Opacity");

    private void Awake()
    {
        platformCollider = GetComponent<Collider>();
        platformRenderer = GetComponent<Renderer>();

        propBlock = new MaterialPropertyBlock();

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

        SetOpacity(intangibleOpacity);
    }

    public void SetSolidState()
    {
        isIntangible = false;

        platformCollider.enabled = true;

        SetOpacity(solidOpacity);
    }

    private void SetOpacity(float value)
    {
        platformRenderer.GetPropertyBlock(propBlock);
        propBlock.SetFloat(OpacityID, value);
        platformRenderer.SetPropertyBlock(propBlock);
    }

    public void Switch()
    {
        ToggleState();
    }
}