using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataHolder
{
    public static bool syncData;
    public static bool isGuestLogin;
    static Texture playerProfilePic;
    public static Texture PlayerProfilePic
    {
        get => playerProfilePic;
        set => playerProfilePic = value;
    }

    static string playerPicURL;
    public static string PlayerPicURL
    {
        get => playerPicURL;
        set => playerPicURL = value;
    }

    static string playerName;
    public static string PlayerName
    {
        get
        {
            return playerName;
        }
        set
        {
            playerName = value;
            TempPlayerNameToEdit = playerName;
        }
    }

    public static string TempPlayerNameToEdit;
    public static string UserId
    {
        get => PlayerPrefs.GetString(StaticKeywords.UserDataKeyWords.userId);
    }

    public static string SocialId
    {
        get => PlayerPrefs.GetString(StaticKeywords.UserDataKeyWords.socialId);
        set => PlayerPrefs.SetString(StaticKeywords.UserDataKeyWords.socialId, value);
    }

    public static bool IsLoggedIn
    {
        get => PlayerPrefs.GetInt(StaticKeywords.UserDataKeyWords.isLoggedIn) == 1;
    }

    static int coinCount;
    public static int _CoinCount
    {
        get => coinCount;
        set
        {
            coinCount = value;

        }
    }

    static int gemCount;
    public static int _GemCount
    {
        get => gemCount;
        set
        {
            gemCount = value;
        }
    }

    static int totalMatches = 100;
    public static int TotalMatches
    {
        get { return totalMatches; }
        set => totalMatches = value;
    }

    static int totalWins = 50;
    public static int TotalWins
    {
        get { return totalWins; }
        set => totalWins = value;
    }

    static string guestUserId;
    public static string GuestUserId
    {
        get => guestUserId;
        set => guestUserId = value;
    }

    public static void ResetStats()
    {
        syncData = false;
        isGuestLogin = false;
        PlayerProfilePic = null;
        PlayerName = "";
        _GemCount = 0;
        _CoinCount = 0;
        GuestUserId = null;
    }
}

public struct UserDataHolder
{
    public Dictionary<string, object> userData;
    public UserDataHolder(string profileImageUrl, string userId, string userName, string loginMode, string appVersion, string deviceType, string socialId)
    {
        userData = new Dictionary<string, object>() {
      {StaticKeywords.UserDataKeyWords.profileImageUrl , profileImageUrl},
      {StaticKeywords.UserDataKeyWords.userName , userName},
      {StaticKeywords.UserDataKeyWords.accountType , loginMode},
      {StaticKeywords.UserDataKeyWords.appVersion , appVersion},
      {StaticKeywords.UserDataKeyWords.deviceInfo , deviceType},
      {StaticKeywords.UserDataKeyWords.socialId , socialId},
      {StaticKeywords.UserDataKeyWords.userId , userId},
    };
    }
}

public static class StaticKeywords
{
    public static class UserDataKeyWords
    {
        public const string profileImageUrl = "profile";
        public const string userId = "uid";
        public const string userName = "name";
        public const string isLoggedIn = "isLoggedIn";
        public const string coins = "coins";
        public const string gems = "gems";
        public const string accountType = "accountType";
        public const string appVersion = "appVersion";
        public const string deviceInfo = "deviceInfo";
        public const string socialId = "socialId";
        public const string tokenId = "tokenId";
        public const string totalMatches = "totalMatches";
        public const string wins = "wins";
    }

    public static class CollectionKeyWords
    {
        public static readonly string users = "Users";
        public static readonly string consumableItems = "ConsumableItems";
        public static readonly string consumableItemType = "itemType";
        public static readonly string consumableItemCost = "cost";
        public static readonly string consumableItemAmount = "amount";
        public const string currencyTypeToUpdate = "inApp";
        public const string coin = "coin";
        public const string gem = "gem";
    }

    public static class AuthProvider
    {
        public const string facebook = "facebook";
        public const string apple = "apple";
        public const string google = "google";
        public const string guest = "guest";
    }
}