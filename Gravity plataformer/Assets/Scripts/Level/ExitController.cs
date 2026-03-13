using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitController : MonoBehaviour
{
    [Header("Llaves")]
    [SerializeField] private List<KeyController> keys = new List<KeyController>();

    [Header("Indicadores")]
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private Transform cubeContainer;
    [SerializeField] private float spacing = 1.2f;

    [Header("Materiales de Indicadores")]
    [SerializeField] private Material inactiveMaterial;
    [SerializeField] private Material activeMaterial;

    [Header("Material Plataforma")]
    [SerializeField] private Material platformCompletedMaterial;

    [Header("Referencia Plataforma")]
    [SerializeField] private Renderer platformRenderer;

    [Header("Trigger Salida")]
    [SerializeField] private GameObject exitTriggerObject;

    private Collider exitCollider;

    private List<Renderer> cubeRenderers = new List<Renderer>();
    private int activatedKeys = 0;
    private bool levelUnlocked = false;

    private void Awake()
    {
        if (exitTriggerObject != null)
        {
            exitCollider = exitTriggerObject.GetComponent<Collider>();
            exitTriggerObject.SetActive(false); // empieza deshabilitado
        }
    }

    private void Start()
    {
        GenerateIndicatorCubes();
        RegisterKeys();

        CheckIfNoKeys();
    }

    private void CheckIfNoKeys()
    {
        if (keys == null || keys.Count == 0)
        {
            UnlockPlatform();
        }
    }

    private void GenerateIndicatorCubes()
    {
        cubeRenderers.Clear();

        for (int i = 0; i < keys.Count; i++)
        {
            GameObject cube = Instantiate(cubePrefab, cubeContainer);
            cube.transform.localPosition = new Vector3(0f, i * spacing, 0f);

            Renderer rend = cube.GetComponent<Renderer>();
            rend.sharedMaterial = inactiveMaterial;

            cubeRenderers.Add(rend);
        }
    }

    private void RegisterKeys()
    {
        for (int i = 0; i < keys.Count; i++)
        {
            int index = i;
            keys[i].Initialize(this, index);
        }
    }

    public void ActivateKey(int index)
    {
        if (cubeRenderers[index].sharedMaterial == activeMaterial)
            return;

        cubeRenderers[index].sharedMaterial = activeMaterial;
        activatedKeys++;

        if (activatedKeys >= keys.Count)
        {
            UnlockPlatform();
        }
    }

    private void UnlockPlatform()
    {
        levelUnlocked = true;

        if (platformRenderer != null)
            platformRenderer.sharedMaterial = platformCompletedMaterial;

        if (exitTriggerObject != null)
            exitTriggerObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!levelUnlocked)
            return;

        if (other.CompareTag("Player"))
        {
            Debug.Log("exit level...");
            GameManager.NextLevel();
        }
    }

}