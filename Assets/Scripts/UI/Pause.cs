using UnityEngine;

public class Pause : MonoBehaviour
{
    public GameObject panel;
    public void pause()
    {
        panel.SetActive(true);
        Time.timeScale = 0f;
    }
}
