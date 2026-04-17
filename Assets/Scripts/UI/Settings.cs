using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class Settings : MonoBehaviour
{
    public TMP_Dropdown resolutinDropdown;
    [SerializeField] private Pause pause;
    Resolution[] resolutions;
    void Start()
    {
        resolutinDropdown.ClearOptions();
        List<string> options=new List<string>();
        resolutions=Screen.resolutions;
        int currentResolutionindex = 0;

        for(int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height + " " + resolutions[i].refreshRateRatio + "Hz";
            options.Add(option);
            if (resolutions[i].width==Screen.currentResolution.width && resolutions[i].height==Screen.currentResolution.height)
                currentResolutionindex = i;
        }
        resolutinDropdown.AddOptions(options);
        resolutinDropdown.RefreshShownValue();
        LoadSettings(currentResolutionindex);
    }
    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }
    public void SetResolution(int resolutionindex)
    {
        Resolution resolution = resolutions[resolutionindex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
    public void ExitSettings()
    {
        // Возвращаемся из настроек в обычную паузу (без перезагрузки сцены).
        if (pause == null)
        {
            pause = FindFirstObjectByType<Pause>();
        }

        if (pause != null)
        {
            pause.CloseOptions();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    public void SaveSetting()
    {
        PlayerPrefs.SetInt("ResolutionPreference", resolutinDropdown.value);
        PlayerPrefs.SetInt("FullscreenPreference", System.Convert.ToInt32(Screen.fullScreen));
    }
    public void LoadSettings(int currentResolutionIndex)
    {
        if (PlayerPrefs.HasKey("ResolutionPreference"))
            resolutinDropdown.value = PlayerPrefs.GetInt("ResolutionPreference");
        else
            resolutinDropdown.value = currentResolutionIndex;
        if (PlayerPrefs.HasKey("FullscreenPreference"))
            Screen.fullScreen = System.Convert.ToBoolean(PlayerPrefs.GetInt("FullscreenPreference"));
        else 
            Screen.fullScreen = true;
    }
}
