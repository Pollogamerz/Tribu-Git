using UnityEngine;
using UnityEngine.Audio;

public class AudioControl : MonoBehaviour
{
    public AudioMixer mixer;

    public void SetMasterVolume(float volume)
    {
        float dB = Mathf.Lerp(-80f, 0f, volume);
        Debug.Log($"Volumen ajustado a: {dB} dB con input de slider: {volume}");
        mixer.SetFloat("MasterVolume", dB);
    }
}
