using UnityEngine;

public static class SettingsManager
{
    public static System.Action OnSettingsChanged;

    private const string SFX_VOLUME_KEY = "SFXVolume";
    private const string MOUSE_SENS_KEY = "MouseSensitivity";
    private const string INVERT_CLICK_KEY = "InvertMouseClick";
    private const string HOLD_CAMERA_KEY = "HoldCamera";
    private const string DISCRETE_COMPASS_KEY = "DiscreteCompass";

    // Valores por defecto
    private const float DEFAULT_VOLUME = 0.8f;
    private const float DEFAULT_MOUSE_SENS = 0.1f;
    private const int DEFAULT_INVERT_CLICK = 0;
    private const int DEFAULT_HOLD_CAMERA = 1;
    private const int DEFAULT_DISCRETE_COMPASS = 1;

    public static float SFXVolume
    {
        get => PlayerPrefs.GetFloat(SFX_VOLUME_KEY, DEFAULT_VOLUME);
        set
        {
            PlayerPrefs.SetFloat(SFX_VOLUME_KEY, value);
            PlayerPrefs.Save();
            OnSettingsChanged?.Invoke();
        }
    }

    public static float MouseSensitivity
    {
        get => PlayerPrefs.GetFloat(MOUSE_SENS_KEY, DEFAULT_MOUSE_SENS);
        set
        {
            PlayerPrefs.SetFloat(MOUSE_SENS_KEY, value);
            PlayerPrefs.Save();
            OnSettingsChanged?.Invoke();
        }
    }

    public static bool InvertMouseClick
    {
        get => PlayerPrefs.GetInt(INVERT_CLICK_KEY, DEFAULT_INVERT_CLICK) == 1;
        set
        {
            PlayerPrefs.SetInt(INVERT_CLICK_KEY, value ? 1 : 0);
            PlayerPrefs.Save();
            OnSettingsChanged?.Invoke();
        }
    }

    public static bool HoldToMoveCamera
    {
        get => PlayerPrefs.GetInt(HOLD_CAMERA_KEY, DEFAULT_HOLD_CAMERA) == 1;
        set
        {
            PlayerPrefs.SetInt(HOLD_CAMERA_KEY, value ? 1 : 0);
            PlayerPrefs.Save();
            OnSettingsChanged?.Invoke();
        }
    }

    public static bool UseDiscreteCompass
    {
        get => PlayerPrefs.GetInt(DISCRETE_COMPASS_KEY, DEFAULT_DISCRETE_COMPASS) == 1;
        set
        {
            PlayerPrefs.SetInt(DISCRETE_COMPASS_KEY, value ? 1 : 0);
            PlayerPrefs.Save();
            OnSettingsChanged?.Invoke();
        }
    }
}