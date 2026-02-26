using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitController : MonoBehaviour
{
    [Header("Llaves")]
    [SerializeField] private List<KeyController> keys = new List<KeyController>();

    [Header("Cubos Indicadores")]
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private Transform cubeContainer;
    [SerializeField] private float spacing = 1.2f;

    [Header("Colores")]
    [SerializeField] private Color inactiveColor = Color.gray;
    [SerializeField] private Color activeColor = Color.green;
    [SerializeField] private Color platformCompletedColor = Color.cyan;

    [Header("Referencia Plataforma")]
    [SerializeField] private Renderer platformRenderer;

    private List<Renderer> cubeRenderers = new List<Renderer>();
    private int activatedKeys = 0;
    private bool levelUnlocked = false;

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

    // =============================
    // GENERAR CUBOS DINÁMICAMENTE
    // =============================

    private void GenerateIndicatorCubes()
    {
        cubeRenderers.Clear();

        for (int i = 0; i < keys.Count; i++)
        {
            GameObject cube = Instantiate(cubePrefab, cubeContainer);
            cube.transform.localPosition = new Vector3(0f, i * spacing, 0f);

            Renderer rend = cube.GetComponent<Renderer>();
            rend.material.color = inactiveColor;

            cubeRenderers.Add(rend);
        }
    }

    // =============================
    // REGISTRAR LLAVES
    // =============================

    private void RegisterKeys()
    {
        for (int i = 0; i < keys.Count; i++)
        {
            int index = i;
            keys[i].Initialize(this, index);
        }
    }

    // =============================
    // LLAVE ACTIVADA
    // =============================

    public void ActivateKey(int index)
    {
        if (cubeRenderers[index].material.color == activeColor)
            return;

        cubeRenderers[index].material.color = activeColor;
        activatedKeys++;

        if (activatedKeys >= keys.Count)
        {
            UnlockPlatform();
        }
    }

    private void UnlockPlatform()
    {
        levelUnlocked = true;
        platformRenderer.material.color = platformCompletedColor;
    }

    // =============================
    // DETECTAR JUGADOR
    // =============================

    private void OnTriggerEnter(Collider other)
    {
        if (!levelUnlocked)
            return;

        if (other.CompareTag("Player"))
        {
            LoadNextLevel();
        }
    }

    private void LoadNextLevel()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex + 1);
    }
}