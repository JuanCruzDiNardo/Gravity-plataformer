using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider SFXVolumeSlider;
    [SerializeField] private Slider mouseSensitivitySlider;

    [Header("Toggles")]
    [SerializeField] private Toggle invertClickToggle;
    [SerializeField] private Toggle holdCameraToggle;
    [SerializeField] private Toggle discreteCompassToggle;

    private void Start()
    {
        LoadSettings();
        RegisterUIEvents();
    }

    private void LoadSettings()
    {
        SFXVolumeSlider.value = SettingsManager.SFXVolume;
        mouseSensitivitySlider.value = SettingsManager.MouseSensitivity;

        invertClickToggle.isOn = SettingsManager.InvertMouseClick;
        holdCameraToggle.isOn = SettingsManager.HoldToMoveCamera;
        discreteCompassToggle.isOn = SettingsManager.UseDiscreteCompass;
    }

    private void RegisterUIEvents()
    {
        SFXVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        mouseSensitivitySlider.onValueChanged.AddListener(SetMouseSensitivity);

        invertClickToggle.onValueChanged.AddListener(SetInvertClick);
        holdCameraToggle.onValueChanged.AddListener(SetHoldCamera);
        discreteCompassToggle.onValueChanged.AddListener(SetDiscreteCompass);
    }

    private void SetSFXVolume(float value)
    {
        SettingsManager.SFXVolume = value;
    }

    private void SetMouseSensitivity(float value)
    {
        SettingsManager.MouseSensitivity = value;
    }

    private void SetInvertClick(bool value)
    {
        SettingsManager.InvertMouseClick = value;
    }

    private void SetHoldCamera(bool value)
    {
        SettingsManager.HoldToMoveCamera = value;
    }

    private void SetDiscreteCompass(bool value)
    {
        SettingsManager.UseDiscreteCompass = value;
    }
}