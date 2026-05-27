using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
public class LoadScreen : MonoBehaviour
{
    public GameObject LoadingScreen;
    public Slider scale;
    public void Loading()
    {
        LoadingScreen.SetActive(true);
        StartCoroutine(LoadAcync());
    }
    IEnumerator LoadAcync()
    {
        AsyncOperation loadAsync = SceneManager.LoadSceneAsync("Level1");
        loadAsync.allowSceneActivation = false;

        while (!loadAsync.isDone)
        {
            scale.value=loadAsync.progress;

            if(loadAsync.progress>= .9f && !loadAsync.allowSceneActivation)
            {
                yield return new WaitForSeconds(1.5f);
                loadAsync.allowSceneActivation=true;
            }
            yield return null;
        }
    }
}
