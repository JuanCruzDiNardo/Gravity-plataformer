using System.Collections.Generic;
using UnityEngine;

public class CameraObstacleTransparency : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Camera cam;

    [Header("Settings")]
    [SerializeField] private float sphereRadius = 0.3f;
    [SerializeField] private float transparency = 0.5f;
    [SerializeField] private LayerMask obstacleLayers;

    private Dictionary<Renderer, Material> originalMaterials = new Dictionary<Renderer, Material>();
    private List<Renderer> currentObstacles = new List<Renderer>();

    void LateUpdate()
    {
        if (player == null || cam == null) return;

        Vector3 direction = player.position - cam.transform.position;
        float distance = direction.magnitude;

        RaycastHit[] hits = Physics.SphereCastAll(
            cam.transform.position,
            sphereRadius,
            direction.normalized,
            distance,
            obstacleLayers
        );

        HashSet<Renderer> newObstacles = new HashSet<Renderer>();

        foreach (RaycastHit hit in hits)
        {
            Renderer rend = hit.collider.GetComponent<Renderer>();
            if (rend == null) continue;

            newObstacles.Add(rend);

            if (!originalMaterials.ContainsKey(rend))
            {
                Material original = rend.material;
                originalMaterials[rend] = original;

                Material instance = new Material(original);
                rend.material = instance;

                SetOpacity(instance, transparency);
            }
        }

        List<Renderer> toRestore = new List<Renderer>();

        foreach (var pair in originalMaterials)
        {
            if (!newObstacles.Contains(pair.Key))
            {
                RestoreMaterial(pair.Key);
                toRestore.Add(pair.Key);
            }
        }

        foreach (Renderer r in toRestore)
        {
            originalMaterials.Remove(r);
        }
    }

    void RestoreMaterial(Renderer rend)
    {
        if (rend == null) return;

        rend.material = originalMaterials[rend];
    }

    void SetOpacity(Material mat, float value)
    {
        if (mat.HasProperty("_Opacity"))
            mat.SetFloat("_Opacity", value);
        else if (mat.HasProperty("_BaseColor"))
        {
            Color c = mat.GetColor("_BaseColor");
            c.a = value;
            mat.SetColor("_BaseColor", c);
        }
    }
}