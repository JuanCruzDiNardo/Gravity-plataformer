using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [System.Serializable]
    public class Sound
    {
        public string id;
        public AudioClip clip;

        [Range(0f, 1f)]
        public float volume = 1f;

        public bool loop = false;
        public bool spatial = false;

        [HideInInspector]
        public AudioSource source;
    }

    [Header("Sound List")]
    [SerializeField] private List<Sound> sounds = new List<Sound>();

    private float sfxVolume = 1f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadVolume();

        foreach (var s in sounds)
        {
            AudioSource src = gameObject.AddComponent<AudioSource>();

            src.clip = s.clip;
            src.loop = s.loop;
            src.playOnAwake = false;
            src.spatialBlend = s.spatial ? 1f : 0f;

            src.volume = s.volume * sfxVolume;

            s.source = src;
        }
    }

    //suscripcion de eventos
    private void OnEnable()
    {
        GravityController.OnGravityChanged += HandleGravityChanged;
        GameManager.onLevelStart += HandleLevelStartd;
        KeyController.onKeyPickup += HandleKeyPickup;
        SettingsManager.OnSettingsChanged += LoadVolume;
    }

    private void OnDisable()
    {
        GravityController.OnGravityChanged -= HandleGravityChanged;
        GameManager.onLevelStart -= HandleLevelStartd;
        KeyController.onKeyPickup -= HandleKeyPickup; 
        SettingsManager.OnSettingsChanged -= LoadVolume;
    }

    private void HandleGravityChanged(GravityController.GravityDirection dir)
    {
        PlayOneShot("gravity_shift");
    }
    private void HandleKeyPickup()
    {
        PlayOneShot("key_pickup");
    }
    private void HandleLevelStartd()
    {
        PlayOneShot("level_start");
    }

    void LoadVolume()
    {
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        PlayerPrefs.SetFloat("SFXVolume", volume);

        foreach (var s in sounds)
        {
            if (s.source != null)
                s.source.volume = s.volume * sfxVolume;
        }
    }

    public void Play(string id)
    {
        Sound s = sounds.Find(x => x.id == id);

        if (s == null) return;

        s.source.volume = s.volume * sfxVolume;
        s.source.Play();
    }

    public void Stop(string id)
    {
        Sound s = sounds.Find(x => x.id == id);

        if (s == null) return;

        s.source.Stop();
    }

    public void PlayOneShot(string id)
    {
        Sound s = sounds.Find(x => x.id == id);

        if (s == null)
        {
            Debug.Log("Sound not found: " + id);
            return;
        }

        Debug.Log("Playing sound: " + id);
        s.source.PlayOneShot(s.clip, s.volume * sfxVolume);
    }

    public void PlayAtPosition(string id, Vector3 position)
    {
        Sound s = sounds.Find(x => x.id == id);

        if (s == null) return;

        AudioSource.PlayClipAtPoint(s.clip, position, s.volume * sfxVolume);
    }
}