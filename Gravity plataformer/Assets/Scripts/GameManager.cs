using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private static PlayerInputAction action;

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

        action = new PlayerInputAction();
        action.Enable();
    }

    private void Update()
    {
        if(GetCurrentLevelIndex() == 0) return;

        CheckPauseInput();
        CheckResetInput();
    }

    private void CheckPauseInput()
    {
        if (action.Player.Pause.WasPressedThisFrame())
        {
            if (PauseManager.Instance.isPaused)
                PauseManager.Instance.UnPause();
            else
                PauseManager.Instance.Pause();
        }
    }

    private void CheckResetInput()
    {
        if (action.Player.Reset.WasPressedThisFrame())
        {
            ResetLevel();
        }
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