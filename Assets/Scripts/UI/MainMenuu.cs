using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButton : MonoBehaviour
{
    public void menu()
    {
        SceneManager.LoadScene("Menu");
    }
}
