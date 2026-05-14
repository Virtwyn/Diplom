using UnityEngine;
using UnityEngine.Audio;

public class SoundMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    public void SetMasterVolume(float level)
    {
        audioMixer.SetFloat("MasterVolume", ConvertToDB(level));
    }

    public void SetSoundFXVolume(float level)
    {
        audioMixer.SetFloat("SoundFXVolume", ConvertToDB(level));
    }

    public void SetMusicVolume(float level)
    {
        audioMixer.SetFloat("MusicVolume", ConvertToDB(level));
    }

    private float ConvertToDB(float level)
    {
        // level приходит от 0.0001 до 1 (слайдер)
        // Mathf.Log10 переводит в децибелы правильно
        return Mathf.Log10(Mathf.Max(level, 0.0001f)) * 20f;
    }
}