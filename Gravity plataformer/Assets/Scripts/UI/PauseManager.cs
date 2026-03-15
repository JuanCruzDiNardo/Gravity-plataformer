using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseManager : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] private GameObject PausePanel;
    [SerializeField] private GameObject PauseCanvas;
    [SerializeField] private GameObject optionsCanvas;
    
    [SerializeField] public bool isPaused;    
    
    public static PauseManager Instance;

    private void Start()
    {
        // Singleton simple
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        UnPause();
    }

    // ---------------- PAUSA ----------------

    public void OpenOptions()
    {
        PauseCanvas.SetActive(false);
        optionsCanvas.SetActive(true);
    }

    public void BackToMainMenu()
    {
        UnPause();
        GameManager.LoadLevel(0); //index 0 es menu principal
    }

    public void UnPause()
    {
        isPaused = false;
        PausePanel.SetActive(false);
        PauseCanvas.SetActive(false);
        optionsCanvas.SetActive(false);
        Time.timeScale = 1;
    }

    public void Pause()
    {
        isPaused = true;
        PausePanel.SetActive(true);
        PauseCanvas.SetActive(true);
        optionsCanvas.SetActive(false);
        Time.timeScale = 0;
    }
}