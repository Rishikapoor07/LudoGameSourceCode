using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class GoogleAdManager : MonoBehaviour
{
    public static GoogleAdManager instance;
    RewardedAd _rewardedAd;
    AdRequest _adRequest;
    Database _database;

	[SerializeField] string androidAdID = "ca-app-pub-3940256099942544/5224354917";
	[SerializeField] string iosAdID = "ca-app-pub-3940256099942544/1712485313";

	private void Start()
	{
        instance = this;
        DontDestroyOnLoad(instance);

		string adUnitId = "";
#if UNITY_EDITOR
		adUnitId = "usused";
#elif UNITY_ANDROID
		adUnitId = androidAdID;
#elif UNITY_IPHONE
        adUnitId = iosAdID;
#endif
		_rewardedAd = new RewardedAd(adUnitId);

        // Called when an ad request has successfully loaded.
        this._rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
        // Called when an ad request failed to load.
        this._rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        // Called when an ad is shown.
        this._rewardedAd.OnAdOpening += HandleRewardedAdOpening;
        // Called when an ad request failed to show.
        this._rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        // Called when the user should be rewarded for interacting with the ad.
        this._rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        // Called when the ad is closed.
        this._rewardedAd.OnAdClosed += HandleRewardedAdClosed;

        HelperUtil.CallAfterCondition(() =>
        {
            // Initialize the Google Mobile Ads SDK.
            MobileAds.Initialize(HandleInitCompleteAction);
        }, () => GameState.isInternetConnected);

        // Create an empty ad request.
        _adRequest = new AdRequest.Builder().Build();

        _database = new Database();
	}

	#region Ad Handle callbacks

	private void HandleInitCompleteAction(InitializationStatus initstatus)
	{
		Debug.Log("Initialization complete.");
        LoadRewardedAd();
	}

    public void HandleRewardedAdLoaded(object sender, EventArgs args)
    {
        Debug.Log("HandleRewardedAdLoaded event received");
    }

    public void HandleRewardedAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        Debug.Log("HandleRewardedAdFailedToLoad event received with message: "
                             + args.LoadAdError.ToString());
        LoadRewardedAd();
    }

    public void HandleRewardedAdOpening(object sender, EventArgs args)
    {
        Debug.Log("HandleRewardedAdOpening event received");
    }

    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    {
        Debug.Log("HandleRewardedAdFailedToShow event received with message: "
                             + args.AdError);
    }

    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        Debug.Log("HandleRewardedAdClosed event received");
    }

    public void HandleUserEarnedReward(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;
        Debug.Log("HandleRewardedAdRewarded event received for "
                        + amount.ToString() + " " + type);
        //_database.AddCollectables(GameInfo.rewardedAdCoin, StaticKeywords.UserDataKeyWords.coins, () =>
        //{
        //    PlayerDataHolder._CoinCount += GameInfo.rewardedAdCoin;
        //    UIManager.OnRewardedAdFininsh(GameInfo.rewardedAdCoin);
        //});

        FirebaseAuthentication.instance.UpdateCurrency("ads",
            onCompletion:
            () =>
            {
                PlayerDataHolder._CoinCount += GameInfo.rewardedAdCoin;
                UIManager.OnRewardedAdFininsh(GameInfo.rewardedAdCoin);
            });

        LoadRewardedAd();
    }

    #endregion Ad Handle callbacks

    /// <summary>
    /// A function that loads rewarded ad and make it ready to show
    /// </summary>
    public void LoadRewardedAd()
	{
        _rewardedAd.LoadAd(_adRequest);
	}

    /// <summary>
    /// A function that shows rewarded ad on the screen 
    /// </summary>
    public void ShowRewardedAd()
	{
        if(!_rewardedAd.IsLoaded()) _rewardedAd.LoadAd(_adRequest);

        HelperUtil.CallAfterCondition(() =>
        {
            _rewardedAd.Show();
        }, () => _rewardedAd.IsLoaded());
	}
}
