using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject levelSelectCanvas;
    [SerializeField] private GameObject optionsCanvas;

    [Header("Primer nivel jugable")]
    [SerializeField] private int firstLevelIndex = 1;

    [Header("Botones de niveles")]
    [SerializeField] private Button[] levelButtons;

    [Header("Navegación niveles")]
    [SerializeField] private Button nextLevelsButton;
    [SerializeField] private Button previousLevelsButton;

    private int levelsPerPage;
    private int currentPage = 0;
    private int totalLevels;

    private void Start()
    {
        ShowMainMenu();
    }

    // ---------------- MENÚ PRINCIPAL ----------------

    public void PlayGame()
    {
        GameManager.LoadLevel(firstLevelIndex);
    }

    public void OpenLevelSelect()
    {
        mainMenuCanvas.SetActive(false);
        optionsCanvas.SetActive(false);
        levelSelectCanvas.SetActive(true);

        InitializeLevelSelect();
    }

    public void OpenOptions()
    {
        mainMenuCanvas.SetActive(false);
        levelSelectCanvas.SetActive(false);
        optionsCanvas.SetActive(true);
    }

    public void BackToMainMenu()
    {
        ShowMainMenu();
    }

    public void QuitGame()
    {
        Debug.Log("Cerrando juego");
        Application.Quit();
    }

    private void ShowMainMenu()
    {
        mainMenuCanvas.SetActive(true);
        levelSelectCanvas.SetActive(false);
        optionsCanvas.SetActive(false);
    }

    // ---------------- LEVEL SELECT ----------------

    private void InitializeLevelSelect()
    {
        levelsPerPage = levelButtons.Length;
        totalLevels = SceneManager.sceneCountInBuildSettings - firstLevelIndex;

        currentPage = 0;

        RefreshLevelButtons();
    }

    public void NextLevelsPage()
    {
        int maxPage = Mathf.CeilToInt((float)totalLevels / levelsPerPage) - 1;

        if (currentPage < maxPage)
        {
            currentPage++;
            RefreshLevelButtons();
        }
    }

    public void PreviousLevelsPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            RefreshLevelButtons();
        }
    }

    private void RefreshLevelButtons()
    {
        int startLevel = currentPage * levelsPerPage + 1;

        for (int i = 0; i < levelsPerPage; i++)
        {
            int levelNumber = startLevel + i;
            int sceneIndex = firstLevelIndex + levelNumber - 1;

            if (levelNumber <= totalLevels)
            {
                levelButtons[i].gameObject.SetActive(true);

                TMP_Text text = levelButtons[i].GetComponentInChildren<TMP_Text>();
                if (text != null)
                    text.text = levelNumber.ToString();

                int capturedIndex = sceneIndex;

                levelButtons[i].onClick.RemoveAllListeners();
                levelButtons[i].onClick.AddListener(() => GameManager.LoadLevel(capturedIndex));
            }
            else
            {
                levelButtons[i].gameObject.SetActive(false);
            }
        }

        UpdateLevelNavigation();
    }

    private void UpdateLevelNavigation()
    {
        int maxPage = Mathf.CeilToInt((float)totalLevels / levelsPerPage) - 1;

        previousLevelsButton.interactable = currentPage > 0;
        nextLevelsButton.interactable = currentPage < maxPage;
    }
}