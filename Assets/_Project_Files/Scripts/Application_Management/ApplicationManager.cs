using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ApplicationManager
{
    public static void Reset()
    {
    }
}

public static class Settings
{
    public static bool MusicActive
    {
        get => PlayerPrefs.GetInt(PlayerPrefsKey.Music, 1) == 1;
        set => PlayerPrefs.SetInt(PlayerPrefsKey.Music, value ? 1 : 0);
    }

    public static bool SoundActive
    {
        get => PlayerPrefs.GetInt(PlayerPrefsKey.Sound, 0) == 1;
        set => PlayerPrefs.SetInt(PlayerPrefsKey.Sound, value ? 1 : 0);
    }

    public static bool ShowMicAccessPopUp
    {
        get => PlayerPrefs.GetInt(PlayerPrefsKey.ShowMicPopUp, 1) == 1;
        set => PlayerPrefs.SetInt(PlayerPrefsKey.ShowMicPopUp, value ? 0 : 1);
    }

    public static int UseMic
    {
        get => PlayerPrefs.GetInt(PlayerPrefsKey.MicAccess, 0);
        set => PlayerPrefs.SetInt(PlayerPrefsKey.MicAccess, value);
    }
}


public class PlayerPrefsKey
{
    public static string Music = "Music";
    public static string Sound = "Sound";
    public static string ShowMicPopUp = "ShowMicPopUp";
    public static string MicAccess = "MicAccess";
    public static string LoginType = "loginType";
    public static string MicText = "MicText";
    public static string[] PlayerPrefsKeyArray = { Music, Sound, ShowMicPopUp, MicAccess, LoginType, MicText, StaticKeywords.UserDataKeyWords.socialId, StaticKeywords.UserDataKeyWords.userId, StaticKeywords.UserDataKeyWords.isLoggedIn };

    public static void DeleteAllKeysExcept(string keyName)
    {
        foreach (string _key in PlayerPrefsKeyArray)
        {
            if (!_key.Equals(keyName)) PlayerPrefs.DeleteKey(_key);
        }
    }
}