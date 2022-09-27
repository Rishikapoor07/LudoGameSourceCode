using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameInitManager : MonoBehaviour
{
	private void Awake()
	{
        ChangeLocale(0);
    }

	private void Start()
    {
        HelperUtil.ShowLoading();
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        ApplicationManager.Reset();
        SceneManager.LoadScene(SceneName.UI);
    }

	private void FixedUpdate()
	{
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Application.platform == RuntimePlatform.Android) Application.Quit();
        }
    }

    public void ChangeLocale(int localeID)
    {
        StartCoroutine(SetLocale(localeID));
    }

    IEnumerator SetLocale(int _localeID)
    {
        yield return UnityEngine.Localization.Settings.LocalizationSettings.InitializationOperation;
        UnityEngine.Localization.Settings.LocalizationSettings.SelectedLocale = UnityEngine.Localization.Settings.LocalizationSettings.AvailableLocales.Locales[_localeID];
    }
}
