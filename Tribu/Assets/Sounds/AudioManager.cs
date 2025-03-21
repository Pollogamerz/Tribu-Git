using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header ("Audio Source")]
    public AudioSource musicSource;
    public AudioSource SFXSource;
    [SerializeField] AudioMixer mixer;

    [Header("Audio Clip")]
    public AudioClip background;
    public AudioClip death;

    [Header("VolumeSliders")]
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        musicSource.clip = background;
        musicSource.Play();

        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.onValueChanged.AddListener(ChangeMasterVolume);
        }

        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.onValueChanged.AddListener(ChangeMusicVolume);
        }

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.onValueChanged.AddListener(ChangeSFXVolume);
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    public void ChangeMasterVolume(float sliderValue)
    {
        // Convertimos el rango del slider (0 a 1) a un rango de dB (-80 a 0)
        float volumeInDB = (1 - Mathf.Sqrt(sliderValue)) * -80f;
        mixer.SetFloat("MasterVolume", volumeInDB);
        Debug.Log($"Volumen ajustado a: {volumeInDB} dB con valor del slider: {sliderValue}");
    }

    public void ChangeMusicVolume(float sliderValue)
    {
        // Convertimos el rango del slider (0 a 1) a un rango de dB (-80 a 0)
        float volumeInDB = (1 - Mathf.Sqrt(sliderValue)) * -80f;
        mixer.SetFloat("MusicVolume", volumeInDB);
        Debug.Log($"Volumen ajustado a: {volumeInDB} dB con valor del slider: {sliderValue}");
    }

    public void ChangeSFXVolume(float sliderValue)
    {
        // Convertimos el rango del slider (0 a 1) a un rango de dB (-80 a 0)
        float volumeInDB = (1 - Mathf.Sqrt(sliderValue)) * -80f;
        mixer.SetFloat("SFXVolume", volumeInDB);
        Debug.Log($"Volumen ajustado a: {volumeInDB} dB con valor del slider: {sliderValue}");
    }

}
