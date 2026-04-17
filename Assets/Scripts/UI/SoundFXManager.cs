using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SoundFXManager : MonoBehaviour
{
    [SerializeField] private AudioSource soundFXObject;
    
    public static SoundFXManager instance;
    private void Awake()
    {
        if(instance == null)
            instance = this;
    }
    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float voluem)
    {
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        audioSource.clip = audioClip;
        audioSource.volume = voluem;
        audioSource.Play();
        float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, clipLength);
    }
    public void PlayRundomSoundFXClip(AudioClip[] audioClip, Transform spawnTransform, float voluem)
    {
        int rand=Random.Range(0, audioClip.Length);
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        
        audioSource.clip = audioClip[rand];
        audioSource.volume = voluem;
        audioSource.Play();
        float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, clipLength);
    }
}
