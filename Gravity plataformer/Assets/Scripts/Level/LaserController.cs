using UnityEngine;

public class LaserController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform laserOrigin;
    [SerializeField] private Transform laserVisual;

    [Header("Configuración")]
    [SerializeField] private float maxDistance = 20f;

    // pequeño ajuste para que el láser toque la superficie
    [SerializeField] private float surfaceOffset = 0.7f;

    private void Update()
    {
        ShootLaser();
    }

    void ShootLaser()
    {
        Vector3 origin = laserOrigin.position;
        Vector3 direction = laserOrigin.up;

        RaycastHit hit;

        float distance = maxDistance;

        if (Physics.Raycast(origin, direction, out hit, maxDistance))
        {
            //Debug.Log("Laser hit: " + hit.collider.name);

            if (hit.collider.CompareTag("Platform"))
            {
                distance = hit.distance + surfaceOffset;
            }

            if (hit.collider.CompareTag("Player"))
            {
                GameManager.ResetLevel();
            }
        }

        Vector3 endPoint = origin + direction * distance;

        // visualizar el rayo en la escena
        Debug.DrawRay(origin, direction * maxDistance, Color.red);

        UpdateLaserVisual(distance);
    }

    void UpdateLaserVisual(float distance)
    {
        laserVisual.localScale = new Vector3(
            laserVisual.localScale.x,
            distance * 0.5f,
            laserVisual.localScale.z
        );

        laserVisual.localPosition = new Vector3(
            0,
            distance * 0.5f,
            0
        );
    }
}