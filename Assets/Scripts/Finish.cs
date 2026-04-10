using UnityEngine;
using UnityEngine.SceneManagement;

public class levelUnlocker : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Конец уровня срабатывает только для игрока
        if (collision.CompareTag("Player"))
        {
            UnlockNextLevel();
        }
    }

    private void UnlockNextLevel()
    {
        //Текущая сцена
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        //перезагружаем текущую сцену
        SceneManager.LoadScene(currentScene);
    }
}