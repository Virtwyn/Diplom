using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public Button[] buttons;
    private int unlockScenes;

    void Start()
    {
        unlockScenes = PlayerPrefs.GetInt("Scenes", 1);

        for (int i = 0; i < buttons.Length; i++)
            if (i >= unlockScenes)
                buttons[i].interactable = false;
    }
    public void SceneLoad(int levelindex)
    {
        SceneManager.LoadScene(levelindex);
    }
}
