using UnityEngine;
using UnityEngine.SceneManagement;

public class levelUnlocker : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            UnlockNextLevel();
        }
    }

    private void UnlockNextLevel()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        if (currentScene >= PlayerPrefs.GetInt("Scenes"))
        {
            PlayerPrefs.SetInt("Scenes", currentScene + 1);
        }
        SceneManager.LoadScene(currentScene + 1);
    }
}