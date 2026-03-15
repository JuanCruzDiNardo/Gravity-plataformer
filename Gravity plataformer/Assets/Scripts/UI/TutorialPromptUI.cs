using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class TutorialPromptUI : MonoBehaviour
{
    public enum TutorialType
    {
        Movement,
        Camera,
        Gravity,
        Restart
    }

    [Header("Tutorial Type")]
    [SerializeField] private TutorialType tutorialType;

    [SerializeField] private Vector2 keyboardSize = new Vector2(100, 100);
    [SerializeField] private Vector2 gamepadSize = new Vector2(70, 70);

    [Header("Panels")]
    [SerializeField] private GameObject panelWide;
    [SerializeField] private GameObject panelSmall;

    [Header("Images")]
    [SerializeField] private Image image1;
    [SerializeField] private Image image2;
    [SerializeField] private Image image3;

    [Header("Text")]
    [SerializeField] private TMP_Text middleText;

    [Header("Movement Sprites")]
    [SerializeField] private Sprite kbMove1;
    [SerializeField] private Sprite kbMove2;
    [SerializeField] private Sprite gpMove1;
    [SerializeField] private Sprite gpMove2;

    [Header("Camera Sprites")]
    [SerializeField] private Sprite kbCam1;
    [SerializeField] private Sprite kbCam1_Inverted;
    [SerializeField] private Sprite kbCam2;

    [SerializeField] private Sprite gpCam1;
    [SerializeField] private Sprite gpCam1_Inverted;
    [SerializeField] private Sprite gpCam2;

    [Header("Gravity Sprites")]
    [SerializeField] private Sprite kbGrav1;
    [SerializeField] private Sprite kbGrav1_Inverted;
    [SerializeField] private Sprite kbGrav2;
    [SerializeField] private Sprite kbGrav2_Discrete;

    [SerializeField] private Sprite gpGrav1;
    [SerializeField] private Sprite gpGrav1_Inverted;
    [SerializeField] private Sprite gpGrav2;
    [SerializeField] private Sprite gpGrav2_Discrete;

    [Header("Restart Sprites")]
    [SerializeField] private Sprite kbRestart;
    [SerializeField] private Sprite gpRestart;

    private bool usingGamepad;

    void OnEnable()
    {
        SettingsManager.OnSettingsChanged += RefreshTutorial;
    }

    void OnDisable()
    {
        SettingsManager.OnSettingsChanged -= RefreshTutorial;
    }

    void Start()
    {
        DetectInitialInput();
        RefreshTutorial();
    }

    void Update()
    {
        DetectInputChange();
    }

    void DetectInitialInput()
    {
        usingGamepad = Gamepad.current != null;
    }

    void DetectInputChange()
    {
        if (Gamepad.current != null && Gamepad.current.wasUpdatedThisFrame)
        {
            if (!usingGamepad)
            {
                usingGamepad = true;
                RefreshTutorial();
            }
        }

        if (Keyboard.current.anyKey.wasPressedThisFrame)
        {
            if (usingGamepad)
            {
                usingGamepad = false;
                RefreshTutorial();
            }
        }
    }

    public void RefreshTutorial()
    {
        switch (tutorialType)
        {
            case TutorialType.Movement:
                SetupMovement();
                break;

            case TutorialType.Camera:
                SetupCamera();
                break;

            case TutorialType.Gravity:
                SetupGravity();
                break;

            case TutorialType.Restart:
                SetupRestart();
                break;
        }
    }

    void SetupMovement()
    {
        Debug.Log("SetupMovement ejecutado");
        ActivateWide();

        if (usingGamepad)
        {
            image1.sprite = gpMove1;
            image2.sprite = gpMove2;
        }
        else
        {
            image1.sprite = kbMove1;
            image2.sprite = kbMove2;
        }

        ApplyImageSize(usingGamepad);
        middleText.text = "O";
    }

    void SetupCamera()
    {
        Debug.Log("SetupCamera ejecutado");
        bool invert = SettingsManager.InvertMouseClick;
        bool hold = SettingsManager.HoldToMoveCamera;

        if (!hold)
        {
            ActivateSmall();
            image3.sprite = usingGamepad ? gpCam2 : kbCam2;
            return;
        }

        ActivateWide();

        if (usingGamepad)
        {
            image1.sprite = invert ? gpCam1_Inverted : gpCam1;
            image2.sprite = gpCam2;
        }
        else
        {
            image1.sprite = invert ? kbCam1_Inverted : kbCam1;
            image2.sprite = kbCam2;
        }

        ApplyImageSize(usingGamepad);
        middleText.text = "+";
    }

    void SetupGravity()
    {
        Debug.Log("SetupGravity ejecutado");

        ActivateWide();

        bool invert = SettingsManager.InvertMouseClick;
        bool discrete = SettingsManager.UseDiscreteCompass;

        if (usingGamepad)
        {
            image1.sprite = invert ? gpGrav1_Inverted : gpGrav1;
            image2.sprite = discrete ? gpGrav2_Discrete : gpGrav2;
        }
        else
        {
            image1.sprite = invert ? kbGrav1_Inverted : kbGrav1;
            image2.sprite = discrete ? kbGrav2_Discrete : kbGrav2;
        }

        ApplyImageSize(usingGamepad);
        middleText.text = "+";
    }

    void SetupRestart()
    {
        Debug.Log("SetupRestart ejecutado");

        ActivateSmall();
        image3.sprite = usingGamepad ? gpRestart : kbRestart;        
    }

    void ActivateWide()
    {
        panelWide.SetActive(true);
        panelSmall.SetActive(false);
    }

    void ActivateSmall()
    {
        panelWide.SetActive(false);
        panelSmall.SetActive(true);
    }

    void ApplyImageSize(bool usingGamepad)
    {
        Vector2 size = usingGamepad ? gamepadSize : keyboardSize;

        image1.rectTransform.sizeDelta = size;
        image2.rectTransform.sizeDelta = size;
        image3.rectTransform.sizeDelta = size;
    }
}