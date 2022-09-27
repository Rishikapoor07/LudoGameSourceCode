using System.Collections.Generic;
using UnityEngine;

public static class GameInfo 
{
    public static int requiredMoneyToPlay = 500;
    public static int rewardedAdCoin = 500;
    public static bool vivoxLogIn = false;
    public static string AppVersion
	{
        get => Application.version;
	}
    public static string DeviceType
	{
		get 
        {
            if (Application.platform == RuntimePlatform.Android) return "android";
            else if (Application.platform == RuntimePlatform.IPhonePlayer) return "ios";
            else return "android";
		}
	}
}

public enum ApplicationStatus
{
    None,
    Minimized,
    Maximized
}

public static class GameState
{
    public static bool isInternetConnected = false;
    public static bool needToGetProfile = false;

    //public static bool openProfileScreen = false;

    private static bool isMuted = false;

    public static bool _IsMuted
	{
        get => _IsMuted;
        set => _IsMuted = value;
	}
    private static string logInType;
    public static string _LogInType
	{
        get => PlayerPrefs.GetString(PlayerPrefsKey.LoginType);
		set
		{
            logInType = value;
            PlayerPrefs.SetString(PlayerPrefsKey.LoginType, logInType);
		}
	}
    private static bool needToSync;
    public static bool NeedToSync
	{
        get => needToSync;
        set => needToSync = value;
	}

    public static GameScene CurrentScene
    {
        get
        {
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == SceneName.Init) return GameScene.Loading;
            else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == SceneName.UI) return GameScene.Menu;
            else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == SceneName.Game) return GameScene.GamePlay;
            else return GameScene.None;
        }
    }

    private static string currentScreenName;
    public static string CurrentScreenName
    {
        get => currentScreenName;
        set => currentScreenName = value;
    }

    public static int ShowMicText
    {
        get => PlayerPrefs.GetInt(PlayerPrefsKey.MicText, 0);
        set => PlayerPrefs.SetInt(PlayerPrefsKey.MicText, value);
    }

    public static void ResetMembers()
    {
        needToGetProfile = false;
        _LogInType = "";
        NeedToSync = false;
        CurrentScreenName = "";
    }
}

public enum GameScene
{
    None,
    Loading,
    Menu,
    GamePlay
}

#region Enums

public enum ObjectType
{
    Screen,
    PopUp
}

public enum TypedLanguage
{
    Other,
    English,
    Arabic
}

public static class ScreenName
{
    public const string LogIn = "LogIn";
    public const string GuestLogin = "GuestLogin";
    public const string Home = "Home";
    public const string Shop = "Shop";
    public const string UserProfile = "UserProfile";
    public const string AccountBanned = "AccountBanned";

    public static class PopUpName
    {
        public const string MicAccess = "MicAccess";
        public const string SyncAccount = "SyncAccount";
        public const string DeleteAccount = "DeleteAccount";
        public const string EditProfile = "EditProfile";
    }
}

#endregion Enums