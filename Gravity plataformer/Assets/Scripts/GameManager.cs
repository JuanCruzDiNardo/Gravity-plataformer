using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        // Singleton simple
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static void ResetLevel()
    {
        LoadLevel(GetCurrentLevelIndex());
    }

    public static void NextLevel()
    {
        LoadLevel(GetCurrentLevelIndex() + 1);
    }

    public static void PreviousLevel()
    {
        LoadLevel(GetCurrentLevelIndex() - 1);
    }

    public static void LoadLevel(int index)
    {
        if (index >= 0 && index < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(index);
        else
            Debug.Log($"El índice {index} no pertenece a una escena válida.");
    }

    private static int GetCurrentLevelIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }
}