using UnityEngine;

public class Sounds : MonoBehaviour
{
    // Набор звуков, который можно использовать из других скриптов
    public AudioClip[] sounds;
    // Источник звука на этом же объекте.
    private AudioSource audioSrc => GetComponent<AudioSource>();

    // Воспроизвести клип с небольшим рандомом для разнообразия звука
    public void PlaySound(AudioClip clip, float volume = 1f,
    bool destroyed = false, float p1 = 0.85f, float p2 = 1.2f)
    {
        audioSrc.pitch = Random.Range(p1, p2);
        audioSrc.PlayOneShot(clip, volume);
    }
}