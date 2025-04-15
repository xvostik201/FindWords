using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip musicClip;

    private const string MusicVolumeKey = "MusicVolume";
    private const string SFXVolumeKey = "SFXVolume";
    private const string MusicEnabledKey = "MusicEnabled";
    private const string SFXEnabledKey = "SFXEnabled";

    private void Awake()
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

        float musicVol = PlayerPrefs.GetFloat(MusicVolumeKey, .4f);
        float sfxVol = PlayerPrefs.GetFloat(SFXVolumeKey, .4f);
        bool musicEnabled = PlayerPrefs.GetInt(MusicEnabledKey, 1) == 1;
        bool sfxEnabled = PlayerPrefs.GetInt(SFXEnabledKey, 1) == 1;

        musicSource.clip = musicClip;
        musicSource.loop = true;
        musicSource.volume = musicVol;
        sfxSource.volume = sfxVol;

        musicSource.mute = !musicEnabled;
        sfxSource.mute = !sfxEnabled;

        musicSource.Play();
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
        PlayerPrefs.SetFloat(MusicVolumeKey, volume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
        PlayerPrefs.SetFloat(SFXVolumeKey, volume);
    }

    public void ToggleMusic(bool enabled)
    {
        musicSource.mute = !enabled;
        PlayerPrefs.SetInt(MusicEnabledKey, enabled ? 1 : 0);
    }

    public void ToggleSFX(bool enabled)
    {
        sfxSource.mute = !enabled;
        PlayerPrefs.SetInt(SFXEnabledKey, enabled ? 1 : 0);
    }

    public void PlaySFX(AudioClip clip)
    {
        if (!sfxSource.mute && clip != null)
            sfxSource.PlayOneShot(clip);
    }
}
