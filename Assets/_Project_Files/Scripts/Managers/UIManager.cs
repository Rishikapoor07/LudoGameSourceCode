using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIManager_Exculsive;
using UnityEngine.iOS;
using System.Collections;
using System.Globalization;
using System.Linq;
using UnityEngine.EventSystems;
using ArabicSupport;

public class UIManager : MonoBehaviour
{
	#region Static Actions

	public static Action OnGoogleLoginButton;
	public static Action OnAppleLoginButton;
	public static Action OnFacebookLoginButton;
	public static Action OnGuestLoginButton;
	public static Action OnSignOutButton;
	public static Action OnAccountDeleteButton;
	public static Action<string> OnLogInFailed;
	public static Action<int> OnRewardedAdFininsh;
	public static Action OnGetProfile;
	public static Action OnIAPDone;
	public static Action OnInternetDisconnect;
	public static Action OnUserAlreadyExist;

	#endregion Static Actions

	#region Variables

	[Header("References")]
	[SerializeField] List<PanelData> panels;
	private Dictionary<string, Panel> panelsDictionary = new Dictionary<string, Panel>();
	[Space]
	[SerializeField] private List<UIItemReference<Button>> buttonReferences = new List<UIItemReference<Button>>();
	private Dictionary<string, UIItemReference<Button>> buttonReferenceDictionary = new Dictionary<string, UIItemReference<Button>>();
	[SerializeField] Button[] iapButtons;

	[Header("PopUps")]
	[SerializeField] GameObject micAccessPopUp;
	[SerializeField] GameObject deleteAccountPopUp;
	[SerializeField] GameObject editProfilePopUp;
	[SerializeField] GameObject syncProfilePopUp;
	[SerializeField] Toggle showMicAccessPopUp;
	[SerializeField] Button manualMicAccessButton;

	[Header("Player related reference")]
	public RawImage[] playerProfilePics;
	public Text[] playerNameText;
	public List<Text> coinCountText;
	public Text[] gemCountText;
	public InputField playerName_IF;
	//public TMPro.TMP_InputField playerNameIF_TMP;
	public Text textToShow;
	public GameObject playerNameSuggession;
	public Button saveProfileButton;
	public GameObject playerProfileHolder;
	public Sprite[] saveButtonSprites;
	public Texture defaultImage;
	[SerializeField] Text totalMatchesText;
	[SerializeField] Text totalWinsText;
	[SerializeField] Text winRatioText;

	[Header("Other References")]
	private Database database;
	string syncAccountOption; 
	[SerializeField] Text requiredCoinsForVoiceMode;
	[SerializeField] Text requiredCoinsForChatMode;
	[SerializeField] GameObject guestLogInText;
	private TouchScreenKeyboard keyboard;

	#endregion Variables

	#region Unity Callbacks

	private void OnEnable()
	{
		InitReferenceDictionaries();
		SetAllButtonsListners();

		ResetMembers();
		database = new Database();
		if (!PlayerDataHolder.IsLoggedIn) ActivatePanel(ScreenName.LogIn);
	}

	public void OnSelectEventForIF()
	{
		if (keyboard == null) // get the reference for the keyboard
			keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
		return;
        //if (!string.IsNullOrEmpty(PlayerName) && EventSystem.current.currentSelectedGameObject != playerName_IF)
        //{
        //    playerName_IF.text = PlayerName;
        //}

		//if (!string.IsNullOrEmpty(PlayerName) && EventSystem.current.currentSelectedGameObject != playerNameIF_TMP)
		//{
		//	playerNameIF_TMP.text = PlayerName;
		//}

	}

	public string PlayerName
	{
		get => PlayerDataHolder.TempPlayerNameToEdit;
		set => PlayerDataHolder.TempPlayerNameToEdit = value;
	}
	private void Start()
	{
#if UNITY_IOS || UNITY_EDITOR_OSX
		GetButton(ScreenName.LogIn, "AppleLogIn").gameObject.SetActive(true);
		GetButton(ScreenName.UserProfile, "AppleLogIn").gameObject.SetActive(true);

#elif UNITY_ANDROID || UNITY_EDITOR_WIN
		GetButton(ScreenName.LogIn, "AppleLogIn").gameObject.SetActive(false);
		GetButton(ScreenName.UserProfile, "AppleLogIn").gameObject.SetActive(false);
#endif
		GetButton(ScreenName.LogIn, "GoogleLogIn").gameObject.SetActive(true);
		GetButton(ScreenName.UserProfile, "GoogleLogIn").gameObject.SetActive(true);

		buttonReferences[3].GetItem("FBLogIn").gameObject.SetActive(true);
#if UNITY_IOS || UNITY_EDITOR_OSX
		buttonReferences[3].GetItem("AppleLogIn").gameObject.SetActive(true);
#elif UNITY_ANDROID || UNITY_EDITOR_WIN
		buttonReferences[3].GetItem("AppleLogIn").gameObject.SetActive(false);
#endif
		buttonReferences[3].GetItem("GoogleLogIn").gameObject.SetActive(true);
		buttonReferences[3].GetItem("SignOut").gameObject.SetActive(false);
		buttonReferences[3].GetItem("Support").gameObject.SetActive(true);

		////Subscribe event methods
		FirebaseAuthentication.OnLoginsuccess += OnLoginSuccess;
		FirebaseAuthentication.OnSignOutsuccess += OnSignOutSuccess;
		FirebaseAuthentication.OnAccountDeleteSuccess += OnDeleteAccountSuccess;
		Deftsoft.Authentication.FacebookAuth.OnFacebookAuthFailed += OnFailedLogIn;
		OnRewardedAdFininsh += OnRewardedAdFinish;
		OnLogInFailed += OnFailedLogIn;
		OnGetProfile += OnGetProfileSuccess;
		OnIAPDone += OnIAPComplete;
		OpenHomeScreen += HomeScreen;
		OnUserAlreadyExist += OnUserExist;
		OnInternetDisconnect = () =>
		{
			if (editProfilePopUp.activeInHierarchy)
			{
				DeActivateGameObject(editProfilePopUp);
				ActivateGameObject(playerProfileHolder);
			}
		};

		HelperUtil.ShowLoading();


		//playerName_IF.onValueChanged.AddListener(delegate
		//{
		//	//If arabic fix the input field
		//	switch (playerName_IF.CheckIfEnglish(scene: 0))
		//	{
		//		case TypedLanguage.English:
		//			playerName_IF.caretPosition = playerName_IF.text.Length;
		//			break;
		//		case TypedLanguage.Arabic:
		//		case TypedLanguage.Other:
		//			PlayerName = playerName_IF.text;
		//			playerName_IF.caretPosition = playerName_IF.text.Length;
		//			break;
		//	}
		//	ValidatePlayerName();
		//});

		//playerNameIF_TMP.onValueChanged.AddListener(delegate
		//{
		//	textToShow.text = playerNameIF_TMP.text;
		//	//If arabic fix the input field
		//	switch (playerNameIF_TMP.CheckIfEnglish(scene: 0))
		//	{
		//		case TypedLanguage.English:
		//			playerNameIF_TMP.caretPosition = playerNameIF_TMP.text.Length;
		//			break;
		//		case TypedLanguage.Arabic:
		//		case TypedLanguage.Other:
		//			//PlayerName = playerNameIF_TMP.text;
		//			playerNameIF_TMP.caretPosition = playerNameIF_TMP.text.Length;
		//			if (playerNameIF_TMP.text.Length > 0) textToShow.text = ArabicFixer.Fix(textToShow.text);
		//			break;
		//	}
		//	ValidatePlayerName();
		//});

		playerName_IF.onValueChanged.AddListener(delegate
		{
			textToShow.text = playerName_IF.text;
			//If arabic fix the input field
			switch (playerName_IF.CheckIfEnglish(scene: 0))
			{
				case TypedLanguage.English:
					break;
				case TypedLanguage.Arabic:
				case TypedLanguage.Other:
					//PlayerName = playerNameIF_TMP.text;
					if (playerName_IF.text.Length > 0) textToShow.text = ArabicFixer.Fix(textToShow.text);
					break;
			}
			ValidatePlayerName();
		});

		//playerName_IF.onEndEdit.AddListener(delegate
		//{
		//	//If arabic fix the input field
		//	switch (playerName_IF.CheckIfEnglish(scene: 0))
		//	{
		//		case TypedLanguage.English:
		//			PlayerName = playerName_IF.text;
		//			break;
		//		case TypedLanguage.Arabic:
		//		case TypedLanguage.Other:
		//			PlayerName = playerName_IF.text;
		//			playerName_IF.text = ArabicSupport.ArabicFixer.Fix(playerName_IF.text);
		//			////playerName_IF.textComponent.GetComponent<ArabicFixer>().isInputFieldFix = true;
		//			break;
		//	}

		//	if (keyboard == null) // get the reference for the keyboard
		//		keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);

		//	if (keyboard.status == TouchScreenKeyboard.Status.Done) SavePlayerData();
		//	if (keyboard.status == TouchScreenKeyboard.Status.Canceled) UpdateProfile(false);
		//});

		//playerNameIF_TMP.onDeselect.AddListener(delegate
		//{
		//	////If arabic fix the input field
		//	//switch (playerNameIF_TMP.CheckIfEnglish(scene: 0))
		//	//{
		//	//	case TypedLanguage.English:
		//	//		PlayerName = playerNameIF_TMP.text;
		//	//		break;
		//	//	case TypedLanguage.Arabic:
		//	//	case TypedLanguage.Other:
		//	//		PlayerName = playerNameIF_TMP.text;
		//	//		playerNameIF_TMP.text = ArabicSupport.ArabicFixer.Fix(playerNameIF_TMP.text);
		//	//		////playerName_IF.textComponent.GetComponent<ArabicFixer>().isInputFieldFix = true;
		//	//		break;
		//	//}

		//	if (keyboard == null) // get the reference for the keyboard
		//		keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);

		//	if (keyboard.status == TouchScreenKeyboard.Status.Done) SavePlayerData();
		//	if (keyboard.status == TouchScreenKeyboard.Status.Canceled) UpdateProfile(false);
		//});

		playerName_IF.onEndEdit.AddListener(delegate
		{
			if (keyboard == null) // get the reference for the keyboard
				keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);

			if (keyboard.status == TouchScreenKeyboard.Status.Done) SavePlayerData();
			if (keyboard.status == TouchScreenKeyboard.Status.Canceled) UpdateProfile(false);
		});

		PlayerDataHolder.PlayerProfilePic = defaultImage;
		foreach (RawImage playerProfile in playerProfilePics) playerProfile.texture = PlayerDataHolder.PlayerProfilePic;
		
		if (PlayerDataHolder.IsLoggedIn)
		{
			ActivatePanel(ScreenName.Home);
			UpdatePlayerData();
			if (GameState._LogInType == "google") FirebaseAuthentication.OnGoogleLoginSilently();
			else OnLoginSuccess();
		}
		else
		{
			ActivatePanel(ScreenName.LogIn);
			HelperUtil.HideLoading();
		}

		showMicAccessPopUp.onValueChanged.AddListener(delegate { Settings.ShowMicAccessPopUp = showMicAccessPopUp.isOn; });
		requiredCoinsForVoiceMode.text = GameInfo.requiredMoneyToPlay.ToString();
		requiredCoinsForChatMode.text = GameInfo.requiredMoneyToPlay.ToString();

		foreach(Button iapButton in iapButtons)
		{
			iapButton.onClick.RemoveAllListeners();
			iapButton.onClick.AddListener(()=>/*HelperUtil.ShowLoading()*/ HelperUtil.ShowLoader());
		}

#if UNITY_ANDROID

	    AndroidRuntimePermissions.Permission audioPermissions = AndroidRuntimePermissions.CheckPermission("android.permission.RECORD_AUDIO");
        if (audioPermissions == AndroidRuntimePermissions.Permission.Granted) Settings.UseMic = 1;
        else if(audioPermissions == AndroidRuntimePermissions.Permission.Denied) Settings.UseMic = 2;

#elif UNITY_IOS

		if (GameState.ShowMicText == 0) Settings.UseMic = 0;
		else
		{
			if (Application.HasUserAuthorization(UserAuthorization.Microphone)) Settings.UseMic = 1;
			else if (!Application.HasUserAuthorization(UserAuthorization.Microphone)) Settings.UseMic = 2;
		}

		#endif

        if (Settings.UseMic == 2) ActivateGameObject(manualMicAccessButton.gameObject);
        else if (Settings.UseMic == 1) DeActivateGameObject(manualMicAccessButton.gameObject);
        else DeActivateGameObject(manualMicAccessButton.gameObject);
	}


	private void FixedUpdate()
	{
		if (Input.GetKeyDown(KeyCode.Escape)&& Application.platform == RuntimePlatform.Android)
		{
			if (panelsDictionary[ScreenName.Home].gameObject.activeInHierarchy || panelsDictionary[ScreenName.LogIn].gameObject.activeInHierarchy) Application.Quit();
		}

#if UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (panelsDictionary[ScreenName.Home].gameObject.activeInHierarchy || panelsDictionary[ScreenName.LogIn].gameObject.activeInHierarchy) UnityEditor.EditorApplication.isPlaying = false;
		}
#endif
	}

#endregion Unity Callbacks

	#region ListnersMethod

	/// <summary>
	/// When log in process completed successfully
	/// </summary>
	private void OnLoginSuccess()
	{
		OnGetProfileSuccess();
		Database.needToUpdateProfile = false;
		GameState.NeedToSync = false;
		syncAccountOption = null;
	}

	public void OnUpdateProfile() {
	  Dictionary<string, object> updatedData = new Dictionary<string, object>() {
		{StaticKeywords.UserDataKeyWords.userName, FirebaseAuthentication.instance.auth.CurrentUser.DisplayName},
		{StaticKeywords.UserDataKeyWords.profileImageUrl, FirebaseAuthentication.instance.auth.CurrentUser.PhotoUrl.ToString() }
	  };
	  database.UpdateUserData(updatedData, GetUserProfile);
	}

	public void OnGetProfileSuccess()
	{
		database.ReadUserData(OnGetProfileSuccess, OnGetProfileFailed);
	}

	public void GetUserProfile() {
	  HelperUtil.CallAfterDelay(() =>
	  {
		  database.ReadUserData(OnGetProfileSuccess, OnGetProfileFailed);
	  }, 0.2f);
    }

	/// <summary>
	/// When user profile fetched successfully
	/// </summary>
	private void OnGetProfileSuccess(Dictionary<string, object> userData)
	{
		Debug.Log("Get profile success!!" + userData[StaticKeywords.UserDataKeyWords.profileImageUrl].ToString());
		HelperUtil.HideLoading();
		PlayerPrefs.SetInt(StaticKeywords.UserDataKeyWords.isLoggedIn, 1);  ////Set user logged in playerprefs

		if (GameState.CurrentScreenName == ScreenName.LogIn || panelsDictionary[ScreenName.LogIn].gameObject.activeInHierarchy) ActivatePanel(ScreenName.Home);
		HelperUtil.HideLoading();
		Debug.Log("Account type "+userData[StaticKeywords.UserDataKeyWords.accountType].ToString());
		GameState._LogInType = userData[StaticKeywords.UserDataKeyWords.accountType].ToString();
		PlayerPrefs.SetString(StaticKeywords.UserDataKeyWords.userId, userData[StaticKeywords.UserDataKeyWords.userId].ToString());
		PlayerDataHolder.PlayerName = userData[StaticKeywords.UserDataKeyWords.userName].ToString();

		foreach (Text userName in playerNameText)
		{
			userName.text = PlayerDataHolder.PlayerName;   ////Set user name on all screens
			switch (DeftsoftExtensions.CheckIfEnglish(userName.text))
			{
				case TypedLanguage.English:
					break;
				case TypedLanguage.Arabic:
				case TypedLanguage.Other:
					userName.text = ArabicFixer.Fix(userName.text);
					break;
			}
		}

		if (!GameInfo.vivoxLogIn) HelperUtil.instance.vivoxManager.LogInToVivox(PlayerDataHolder.PlayerName);
		//else HelperUtil.HideLoading();

		if (!GameInfo.vivoxLogIn)
		{
			HelperUtil.instance.vivoxManager.LogInToVivox(PlayerDataHolder.PlayerName);
		}
		else
		{
			HelperUtil.HideLoading();
		}
		//todo
		if (GameState._LogInType.Equals(StaticKeywords.AuthProvider.guest))
		{
			PlayerDataHolder.isGuestLogin = true;
			PlayerDataHolder.GuestUserId = PlayerDataHolder.UserId;
		}
		else PlayerDataHolder.isGuestLogin = false;

		if (userData.ContainsKey(StaticKeywords.UserDataKeyWords.coins)) PlayerDataHolder._CoinCount = int.Parse(userData[StaticKeywords.UserDataKeyWords.coins].ToString());
		if (userData.ContainsKey(StaticKeywords.UserDataKeyWords.gems))PlayerDataHolder._GemCount = int.Parse(userData[StaticKeywords.UserDataKeyWords.gems].ToString());
		foreach (Text coinText in coinCountText) coinText.text = PlayerDataHolder._CoinCount.ToString();   ////Set Coins count
		foreach (Text gemText in gemCountText) gemText.text = PlayerDataHolder._GemCount.ToString();   ////Set Gems count

		if(userData.ContainsKey(StaticKeywords.UserDataKeyWords.totalMatches)) PlayerDataHolder.TotalMatches = int.Parse(userData[StaticKeywords.UserDataKeyWords.totalMatches].ToString());
		if(userData.ContainsKey(StaticKeywords.UserDataKeyWords.wins))PlayerDataHolder.TotalWins = int.Parse(userData[StaticKeywords.UserDataKeyWords.wins].ToString());
		//totalMatchesText.text = PlayerDataHolder.TotalMatches.ToString().CustomReverse();
		//totalWinsText.text = PlayerDataHolder.TotalWins.ToString().CustomReverse();
		//winRatioText.text = "%05";//(((float)PlayerDataHolder.TotalWins / (float)PlayerDataHolder.TotalMatches) * 100).ToString()

		PlayerDataHolder.PlayerPicURL = userData[StaticKeywords.UserDataKeyWords.profileImageUrl].ToString();
		HelperUtil.SetTexture(PlayerDataHolder.PlayerPicURL, SetProfilePicture);
	}

	void SetProfilePicture(Texture image)
	{
		foreach (RawImage playerProfile in playerProfilePics) playerProfile.texture = image;
		PlayerDataHolder.PlayerProfilePic = image;
	}

	/// <summary>
	/// When user profile fetching failed
	/// </summary>
	private void OnGetProfileFailed()
	{
		Debug.Log("Get profile failed!!!");
		PlayerPrefs.SetInt(StaticKeywords.UserDataKeyWords.isLoggedIn, 0);
		Settings.ShowMicAccessPopUp = true;
		GameInfo.vivoxLogIn = false;
		showMicAccessPopUp.isOn = false;
		PlayerPrefsKey.DeleteAllKeysExcept(PlayerPrefsKey.MicText);
		ActivatePanel(ScreenName.LogIn);
		HelperUtil.HideLoading();
	}

	/// <summary>
	/// When user signed out successfully
	/// </summary>
	private void OnSignOutSuccess()
	{
		showMicAccessPopUp.isOn = false;
		PlayerPrefsKey.DeleteAllKeysExcept(PlayerPrefsKey.MicText);
		HelperUtil.instance.vivoxManager.LogOutVivox();
		PlayerDataHolder.ResetStats();
		ResetUIData();
		GameState.ResetMembers();
		foreach (RawImage playerProfile in playerProfilePics) playerProfile.texture = defaultImage;
		ActivatePanel(ScreenName.LogIn);
		HelperUtil.CallAfterDelay(() =>
		{
			HelperUtil.HideLoading();
			Debug.Log("User signed out successfully!!!");
		}, 0.5f);
	}

	/// <summary>
	/// When user account deleted successfully
	/// </summary>
	private void OnDeleteAccountSuccess()
	{
		showMicAccessPopUp.isOn = false;
		PlayerPrefsKey.DeleteAllKeysExcept(PlayerPrefsKey.MicText);
		HelperUtil.HideLoading();
		if (HelperUtil.instance.vivoxManager.LoginState == VivoxUnity.LoginState.LoggedIn) HelperUtil.instance.vivoxManager.LogOutVivox();
		PlayerDataHolder.ResetStats();
		ResetUIData();
		GameState.ResetMembers();
		foreach (RawImage playerProfile in playerProfilePics) playerProfile.texture = defaultImage;
		ActivatePanel(ScreenName.LogIn);
	}

	/// <summary>
	/// When reward ad finished then earned coins are updated on board
	/// </summary>
	/// <param name="earnedCoins">amount of earned coins</param>
	private void OnRewardedAdFinish(int earnedCoins)
	{
		Database.listner = null;
		Database.OnSuccess = null;
		Database.OnSuccess = (_userData) => OnGetProfile();
		database.OnDatabseDocUpdate();
	}

	/// <summary>
	/// If fb login get failed due to any reason
	/// </summary>
	/// <param name="message">failed message</param>
	private void OnFailedLogIn(string message)
	{
		Debug.Log("Failed to login!!!");
        HelperUtil.CallAfterDelay(() =>
		{
			HelperUtil.HideLoading();
			if (GameState.CurrentScreenName == ScreenName.LogIn) ActivatePanel(ScreenName.LogIn);
			GameState.NeedToSync = false;
			Database.needToUpdateProfile = false;
		}, 1);
	}

	#endregion ListnersMethod

	#region Core 

	/// <summary>
	/// This function set all the listners function to each button
	/// </summary>
	public void SetAllButtonsListners()
	{
		//WELCOME_PANEL
		SetButton(ScreenName.LogIn, "FBLogIn", () =>
		{
			if (!GameState.isInternetConnected)
			{
				HelperUtil.ShowInternetPopUp(new MessageActionData(GameState.CurrentScene == GameScene.GamePlay ? "Leave" : "Quit", () =>
				{
					if (GameState.CurrentScene == GameScene.GamePlay) HelperUtil.LoadScene(SceneName.UI);
					else Application.Quit();
				}),
			    new MessageActionData("Retry", () =>
				{
					HelperUtil.HidePopUp(HelperUtil.instance.internetPopUp);
				}));

				return;
			}

			HelperUtil.ShowLoading();
			OnFacebookLoginButton();
		});
		SetButton(ScreenName.LogIn, "GoogleLogIn", () =>
		{
			if (!GameState.isInternetConnected)
			{
				HelperUtil.ShowInternetPopUp(new MessageActionData(GameState.CurrentScene == GameScene.GamePlay ? "Leave" : "Quit", () =>
				{
					if (GameState.CurrentScene == GameScene.GamePlay) HelperUtil.LoadScene(SceneName.UI);
					else Application.Quit();
				}),
				new MessageActionData("Retry", () =>
				{
					HelperUtil.HidePopUp(HelperUtil.instance.internetPopUp);
				}));

				return;
			}
			HelperUtil.ShowLoading();
			OnGoogleLoginButton();
		});
		SetButton(ScreenName.LogIn, "AppleLogIn", () =>
		{
			if (!GameState.isInternetConnected)
			{
				HelperUtil.ShowInternetPopUp(new MessageActionData(GameState.CurrentScene == GameScene.GamePlay ? "Leave" : "Quit", () =>
				{
					if (GameState.CurrentScene == GameScene.GamePlay) HelperUtil.LoadScene(SceneName.UI);
					else Application.Quit();
				}),
				new MessageActionData("Retry", () =>
				{
					HelperUtil.HidePopUp(HelperUtil.instance.internetPopUp);
				}));

				return;
			}
			HelperUtil.ShowLoading();
			OnAppleLoginButton();
		});
		SetButton(ScreenName.LogIn, "GuestLogIn", () =>
		{
			if (!GameState.isInternetConnected)
			{
				HelperUtil.ShowInternetPopUp(new MessageActionData(GameState.CurrentScene == GameScene.GamePlay ? "Leave" : "Quit", () =>
				{
					if (GameState.CurrentScene == GameScene.GamePlay) HelperUtil.LoadScene(SceneName.UI);
					else Application.Quit();
				}),
				new MessageActionData("Retry", () =>
				{
					HelperUtil.HidePopUp(HelperUtil.instance.internetPopUp);
				}));

				return;
			}
			HelperUtil.ShowLoading();
			OnGuestLoginButton();
		});
		SetButton(ScreenName.LogIn, "TermsButton", () =>
		{
			ActivatePanel(ScreenName.AccountBanned);
			HelperUtil.CallAfterDelay(() => ActivatePanel(ScreenName.LogIn), 5);
		});

        //HOME_PANEL
        SetButton(ScreenName.Home, "UserProfile", () =>
        {
            //todo
            DeactivateLoginButtons();
            ActivateGameObject(playerProfileHolder);
            ActivatePanel(ScreenName.UserProfile);
        });
		SetButton(ScreenName.Home, "BuyCoins", () =>
		{
			ActivatePanel(ScreenName.Shop);
		});
		SetButton(ScreenName.Home, "BuyGems", () =>
		{
			ActivatePanel(ScreenName.Shop);
		});
		SetButton(ScreenName.Home, "VoiceMode", () =>
		{
			if (!GameState.isInternetConnected)
			{
				HelperUtil.ShowInternetPopUp(new MessageActionData(GameState.CurrentScene == GameScene.GamePlay ? "Leave" : "Quit", () =>
				{
					if (GameState.CurrentScene == GameScene.GamePlay) HelperUtil.LoadScene(SceneName.UI);
					else Application.Quit();
				}),
				new MessageActionData("Retry", () =>
				{
					HelperUtil.HidePopUp(HelperUtil.instance.internetPopUp);
				}));

				return;
			}

			if (Settings.ShowMicAccessPopUp)
			{
				ActivateGameObject(micAccessPopUp);
			}
			else
			{
				if (PlayerDataHolder._CoinCount <= 0)
				{
					HelperUtil.HideLoading();
					HelperUtil.HideLoader();
					HelperUtil.ShowRewardedAdPopUp(new MessageActionData("Watch", () => GoogleAdManager.instance.ShowRewardedAd()), new MessageActionData("Cancel", () => HelperUtil.HidePopUp(HelperUtil.instance.rewardedAdPopUp)));
					return;
				}
#if UNITY_IOS
				//HelperUtil.ShowLoading(0, true);
				HelperUtil.ShowLoader();
				Settings.UseMic = 1;
				JoinGameRoom(Settings.UseMic == 1 ? true : false);
				return;

				AudioSession.SetAudioPlayAndRecord();
				var ab = Microphone.Start(Microphone.devices[0], true, 300, 44100);

				StartCoroutine(checkMicrophoneAccess());
				IEnumerator checkMicrophoneAccess()
				{
					yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
					if (Application.HasUserAuthorization(UserAuthorization.Microphone))
					{
						Debug.Log("Access Granted to Microphone");
						Settings.UseMic = 1;
						GameState.ShowMicText = 1;
						HelperUtil.ShowLoading();
						HelperUtil.LoadScene(SceneName.Game);
						HelperUtil.instance.vivoxManager.JoinVivoxChannel("Demo");
						database.DeductCurrency(GameInfo.requiredMoneyToPlay, StaticKeywords.UserDataKeyWords.coins, () =>
						{
						}, () =>
						{
							HelperUtil.HideLoading();
							HelperUtil.HideLoader();
							DeActivateGameObject(micAccessPopUp);
							HelperUtil.ShowRewardedAdPopUp(new MessageActionData("Watch", () => GoogleAdManager.instance.ShowRewardedAd()), new MessageActionData("Cancel", () => HelperUtil.HidePopUp(HelperUtil.instance.rewardedAdPopUp)));
						});
					}
					else
					{
						Debug.Log("Access denied to Microphone");
						Settings.UseMic = 2;
						GameState.ShowMicText = 1;
						HelperUtil.HideLoading();
						HelperUtil.HideLoader();
						DeActivateGameObject(micAccessPopUp);
						ActivatePanel(ScreenName.Home);
						ActivateGameObject(manualMicAccessButton.gameObject);
					}

				}

				if (Settings.UseMic != 1)
					return;
#endif
                //HelperUtil.ShowLoading(0, true);
                //database.DeductCurrency(GameInfo.requiredMoneyToPlay, StaticKeywords.UserDataKeyWords.coins, () =>
                //{
                //    JoinGameRoom(Settings.UseMic == 1 ? true : false);
                //    HelperUtil.ShowLoading();
                //}, () =>
                //{
                //    HelperUtil.HideLoading();
                //    HelperUtil.ShowRewardedAdPopUp(new MessageActionData("Watch", () => GoogleAdManager.instance.ShowRewardedAd()), new MessageActionData("Cancel", () => HelperUtil.HidePopUp(HelperUtil.instance.rewardedAdPopUp)));
                //});
            }
		});
		SetButton(ScreenName.Home, "ChatMode", () =>
		{
			if (!GameState.isInternetConnected)
			{
				HelperUtil.ShowInternetPopUp(new MessageActionData(GameState.CurrentScene == GameScene.GamePlay ? "Leave" : "Quit", () =>
				{
					if (GameState.CurrentScene == GameScene.GamePlay) HelperUtil.LoadScene(SceneName.UI);
					else Application.Quit();
				}),
				new MessageActionData("Retry", () =>
				{
					HelperUtil.HidePopUp(HelperUtil.instance.internetPopUp);
				}));

				return;
			}

			if (PlayerDataHolder._CoinCount <= 0)
			{
				HelperUtil.HideLoading();
				HelperUtil.HideLoader();
				HelperUtil.ShowRewardedAdPopUp(new MessageActionData("Watch", () => GoogleAdManager.instance.ShowRewardedAd()), new MessageActionData("Cancel", () => HelperUtil.HidePopUp(HelperUtil.instance.rewardedAdPopUp)));
				return;
			}
			//HelperUtil.ShowLoading(0, true);
			HelperUtil.ShowLoader();
			JoinGameRoom(false);
			database.DeductCurrency(GameInfo.requiredMoneyToPlay, StaticKeywords.UserDataKeyWords.coins, () =>
			{
				//HelperUtil.ShowLoading(0, true);
			}, () =>
			{
				HelperUtil.HideLoading();
				HelperUtil.HideLoader();
				//HelperUtil.ShowRewardedAdPopUp(new MessageActionData("Watch", () => GoogleAdManager.instance.ShowRewardedAd()), new MessageActionData("Cancel", () => HelperUtil.HidePopUp(HelperUtil.instance.rewardedAdPopUp)));
			});
		});
		SetButton(ScreenName.Home, "Settings", () =>
		{
			OpenSettings();
		});

		//HOME_PANEL_POPUP
		SetButton(ScreenName.PopUpName.MicAccess, "Yes", () =>
		{
			if (!GameState.isInternetConnected)
			{
				HelperUtil.ShowInternetPopUp(new MessageActionData(GameState.CurrentScene == GameScene.GamePlay ? "Leave" : "Quit", () =>
				{
					if (GameState.CurrentScene == GameScene.GamePlay) HelperUtil.LoadScene(SceneName.UI);
					else Application.Quit();
				}),
				new MessageActionData("Retry", () =>
				{
					HelperUtil.HidePopUp(HelperUtil.instance.internetPopUp);
				}));

				return;
			}

			if (PlayerDataHolder._CoinCount <= 0)
			{
				HelperUtil.HideLoading();
				HelperUtil.HideLoader();
				DeActivateGameObject(micAccessPopUp);
				HelperUtil.ShowRewardedAdPopUp(new MessageActionData("Watch", () => GoogleAdManager.instance.ShowRewardedAd()), new MessageActionData("Cancel", () => HelperUtil.HidePopUp(HelperUtil.instance.rewardedAdPopUp)));
				return;
			}
			//HelperUtil.ShowLoading(0,true);
			HelperUtil.ShowLoader();
			Settings.UseMic = 1;
			JoinGameRoom(Settings.UseMic == 1 ? true : false);
		});
		SetButton(ScreenName.PopUpName.MicAccess, "Cancel", () =>
		{
			if (!GameState.isInternetConnected)
			{
				HelperUtil.ShowInternetPopUp(new MessageActionData(GameState.CurrentScene == GameScene.GamePlay ? "Leave" : "Quit", () =>
				{
					if (GameState.CurrentScene == GameScene.GamePlay) HelperUtil.LoadScene(SceneName.UI);
					else Application.Quit();
				}),
				new MessageActionData("Retry", () =>
				{
					HelperUtil.HidePopUp(HelperUtil.instance.internetPopUp);
				}));

				return;
			}

			if (PlayerDataHolder._CoinCount <= 0)
			{
				HelperUtil.HideLoading();
				HelperUtil.HideLoader();
				DeActivateGameObject(micAccessPopUp);
				HelperUtil.ShowRewardedAdPopUp(new MessageActionData("Watch", () => GoogleAdManager.instance.ShowRewardedAd()), new MessageActionData("Cancel", () => HelperUtil.HidePopUp(HelperUtil.instance.rewardedAdPopUp)));
				return;
			}
			//HelperUtil.ShowLoading(0,true);
			HelperUtil.ShowLoader();
			Settings.UseMic = 2;
			JoinGameRoom(Settings.UseMic == 1 ? true : false);
			database.DeductCurrency(GameInfo.requiredMoneyToPlay, StaticKeywords.UserDataKeyWords.coins, () =>
			{
			}, () =>
			{
				HelperUtil.HideLoading();
				HelperUtil.HideLoader();
				DeActivateGameObject(micAccessPopUp);
				HelperUtil.ShowRewardedAdPopUp(new MessageActionData("Watch", () => GoogleAdManager.instance.ShowRewardedAd()), new MessageActionData("Cancel", () => HelperUtil.HidePopUp(HelperUtil.instance.rewardedAdPopUp)));
			});
		});
		SetButton(ScreenName.PopUpName.MicAccess, "FreeSpace", () =>
		{
			DeActivateGameObject(panelsDictionary[ScreenName.PopUpName.MicAccess].gameObject);
		});

		//PURCHASES
		SetButton(ScreenName.Shop, "Cross", () =>
		{
			ActivatePanel(ScreenName.Home);
		});
		SetButton(ScreenName.Shop, "FreeSpace", () =>
		{
			ActivatePanel(ScreenName.Home);
		});

		//User Profile
		SetButton(ScreenName.UserProfile, "BackButton", () =>
		{
			ActivatePanel(ScreenName.Home);
		});
		SetButton(ScreenName.UserProfile, "DeleteAccount", () =>
		{
			ActivateGameObject(deleteAccountPopUp);
		});
		SetButton(ScreenName.UserProfile, "FBLogIn", () =>
		{
			if (!GameState.isInternetConnected)
			{
				HelperUtil.ShowInternetPopUp(new MessageActionData(GameState.CurrentScene == GameScene.GamePlay ? "Leave" : "Quit", () =>
				{
					if (GameState.CurrentScene == GameScene.GamePlay) HelperUtil.LoadScene(SceneName.UI);
					else Application.Quit();
				}),
				new MessageActionData("Retry", () =>
				{
					HelperUtil.HidePopUp(HelperUtil.instance.internetPopUp);
				}));

				return;
			}
			//ActivateGameObject(syncProfilePopUp);
			syncAccountOption = StaticKeywords.AuthProvider.facebook;
			SyncAccount(true);
		});
		//todo
		SetButton(ScreenName.UserProfile, "GoogleLogIn", () =>
		{
			if (!GameState.isInternetConnected)
			{
				HelperUtil.ShowInternetPopUp(new MessageActionData(GameState.CurrentScene == GameScene.GamePlay ? "Leave" : "Quit", () =>
				{
					if (GameState.CurrentScene == GameScene.GamePlay) HelperUtil.LoadScene(SceneName.UI);
					else Application.Quit();
				}),
				new MessageActionData("Retry", () =>
				{
					HelperUtil.HidePopUp(HelperUtil.instance.internetPopUp);
				}));

				return;
			}
			//ActivateGameObject(syncProfilePopUp);
			syncAccountOption = StaticKeywords.AuthProvider.google;
			SyncAccount(true);
		});
		SetButton(ScreenName.UserProfile, "AppleLogIn", () =>
		{
			if (!GameState.isInternetConnected)
			{
				HelperUtil.ShowInternetPopUp(new MessageActionData(GameState.CurrentScene == GameScene.GamePlay ? "Leave" : "Quit", () =>
				{
					if (GameState.CurrentScene == GameScene.GamePlay) HelperUtil.LoadScene(SceneName.UI);
					else Application.Quit();
				}),
				new MessageActionData("Retry", () =>
				{
					HelperUtil.HidePopUp(HelperUtil.instance.internetPopUp);
				}));

				return;
			}
			//ActivateGameObject(syncProfilePopUp);
			syncAccountOption = StaticKeywords.AuthProvider.apple;
			SyncAccount(true);
		});
		SetButton(ScreenName.UserProfile, "EditName", () =>
		{
			ActivateGameObject(editProfilePopUp);
			DeActivateGameObject(playerProfileHolder);
			//PlayerName = playerNameText[1].text;
			PlayerName = PlayerDataHolder.PlayerName;

			playerName_IF.text = PlayerName;
            playerName_IF.placeholder.GetComponent<Text>().text = PlayerName;
            playerName_IF.ActivateInputField();

            //playerNameIF_TMP.text = PlayerName;
            //playerNameIF_TMP.placeholder.GetComponent<TMPro.TextMeshProUGUI>().text = PlayerName;
            //playerNameIF_TMP.ActivateInputField();
            if (keyboard == null) // get the reference for the keyboard
				keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
		});
		SetButton(ScreenName.UserProfile, "EditImage", () =>
		{
			HelperUtil.PickImage(512, (returnedTexture) =>
			{
				ActivateGameObject(editProfilePopUp);
				DeActivateGameObject(playerProfileHolder);
				//PlayerName = playerNameText[1].text;
				PlayerName = PlayerDataHolder.PlayerName;

				playerName_IF.text = PlayerName;
				playerName_IF.placeholder.GetComponent<Text>().text = PlayerName;

				//playerNameIF_TMP.text = PlayerName;
				//playerNameIF_TMP.placeholder.GetComponent<TMPro.TextMeshProUGUI>().text = PlayerName;
				UpdateProfilePics(returnedTexture);
			}, null);
		});
		SetButton(ScreenName.UserProfile, "SignOut", () =>
		{
			if (!GameState.isInternetConnected)
			{
				HelperUtil.ShowInternetPopUp(new MessageActionData(GameState.CurrentScene == GameScene.GamePlay ? "Leave" : "Quit", () =>
				{
					if (GameState.CurrentScene == GameScene.GamePlay) HelperUtil.LoadScene(SceneName.UI);
					else Application.Quit();
				}),
				new MessageActionData("Retry", () =>
				{
					HelperUtil.HidePopUp(HelperUtil.instance.internetPopUp);
				}));

				return;
			}

			HelperUtil.ShowLoading();
			OnSignOutButton();
		});
		SetButton(ScreenName.UserProfile, "Support", () =>
		{
			string email = "example@gmail.com";
			Application.OpenURL("mailto:" + email);
		});

		//EDIT_PROFILE_POPUP
		SetButton(ScreenName.PopUpName.EditProfile, "Save", () =>
		{
			SavePlayerData();
		});
		SetButton(ScreenName.PopUpName.EditProfile, "Cancel", () =>
		{
			UpdateProfile(false);
		});
		SetButton(ScreenName.PopUpName.EditProfile, "PickImage", () =>
		{
			HelperUtil.PickImage(512, (returnedTexture)=>
			{
				//PlayerName = playerNameText[1].text;
				//playerName_IF.text = PlayerName;
				//playerName_IF.placeholder.GetComponent<Text>().text = PlayerName;
				UpdateProfilePics(returnedTexture);
			}, null );
		});

		//DELETE_PROFILE_POPUP
		SetButton(ScreenName.PopUpName.DeleteAccount, "Delete", () =>
		{
			if (!GameState.isInternetConnected)
			{
				HelperUtil.ShowInternetPopUp(new MessageActionData(GameState.CurrentScene == GameScene.GamePlay ? "Leave" : "Quit", () =>
				{
					if (GameState.CurrentScene == GameScene.GamePlay) HelperUtil.LoadScene(SceneName.UI);
					else Application.Quit();
				}),
				new MessageActionData("Retry", () =>
				{
					HelperUtil.HidePopUp(HelperUtil.instance.internetPopUp);
				}));

				return;
			}

			HelperUtil.ShowLoading();
			OnAccountDeleteButton();
		});
		SetButton(ScreenName.PopUpName.DeleteAccount, "FreeSpace", () =>
		{
			DeActivateGameObject(deleteAccountPopUp);
		});
		SetButton(ScreenName.PopUpName.DeleteAccount, "Cancel", () =>
		{
			DeActivateGameObject(deleteAccountPopUp);
		});

		//SYNC_PROFILE_POPUP
		SetButton(ScreenName.PopUpName.SyncAccount, "Retry", () =>
		{
			if (!GameState.isInternetConnected)
			{
				HelperUtil.ShowInternetPopUp(new MessageActionData(GameState.CurrentScene == GameScene.GamePlay ? "Leave" : "Quit", () =>
				{
					if (GameState.CurrentScene == GameScene.GamePlay) HelperUtil.LoadScene(SceneName.UI);
					else Application.Quit();
				}),
				new MessageActionData("Retry", () =>
				{
					HelperUtil.HidePopUp(HelperUtil.instance.internetPopUp);
				}));

				return;
			}
			SyncAccount(true);
			DeActivateGameObject(syncProfilePopUp);
		});
		SetButton(ScreenName.PopUpName.SyncAccount, "Later", () =>
		{
			//if (!GameState.isInternetConnected)
			//{
			//	HelperUtil.ShowInternetPopUp(new MessageActionData(GameState.CurrentScene == GameScene.GamePlay ? "Leave" : "Quit", () =>
			//	{
			//		if (GameState.CurrentScene == GameScene.GamePlay) HelperUtil.LoadScene(SceneName.UI);
			//		else Application.Quit();
			//	}),
			//	new MessageActionData("Retry", () =>
			//	{
			//		HelperUtil.HidePopUp(HelperUtil.instance.internetPopUp);
			//	}));

			//	return;
			//}
			//SyncAccount(false);
			DeActivateGameObject(syncProfilePopUp);
		});
		SetButton(ScreenName.PopUpName.SyncAccount, "FreeSpace", () =>
		{
			DeActivateGameObject(syncProfilePopUp);
		});
		SetButton(ScreenName.PopUpName.SyncAccount, "Ok", () =>
		{
			DeActivateGameObject(syncProfilePopUp);
		});
	}

    private void DeactivateLoginButtons()
    {
        if (PlayerDataHolder.isGuestLogin || GameState._LogInType == StaticKeywords.AuthProvider.guest)
        {
            buttonReferences[3].GetItem("FBLogIn").gameObject.SetActive(true);

#if UNITY_IOS || UNITY_EDITOR_OSX
            buttonReferences[3].GetItem("AppleLogIn").gameObject.SetActive(true);
#elif UNITY_ANDROID || UNITY_EDITOR_WIN
				buttonReferences[3].GetItem("AppleLogIn").gameObject.SetActive(false);
#endif
            buttonReferences[3].GetItem("GoogleLogIn").gameObject.SetActive(true);
            buttonReferences[3].GetItem("SignOut").gameObject.SetActive(false);
            buttonReferences[3].GetItem("Support").gameObject.SetActive(true);
            ActivateGameObject(guestLogInText);
        }
        else
        {
            buttonReferences[3].GetItem("FBLogIn").gameObject.SetActive(false);
            buttonReferences[3].GetItem("AppleLogIn").gameObject.SetActive(false);
            buttonReferences[3].GetItem("GoogleLogIn").gameObject.SetActive(false);
            buttonReferences[3].GetItem("SignOut").gameObject.SetActive(true);
            buttonReferences[3].GetItem("Support").gameObject.SetActive(false);
            DeActivateGameObject(guestLogInText);
        }
    }

    public void SavePlayerData()
	{
		if (!GameState.isInternetConnected)
		{
			HelperUtil.ShowInternetPopUp(new MessageActionData(GameState.CurrentScene == GameScene.GamePlay ? "Leave" : "Quit", () =>
			{
				if (GameState.CurrentScene == GameScene.GamePlay) HelperUtil.LoadScene(SceneName.UI);
				else Application.Quit();
			}),
			new MessageActionData("Retry", () =>
			{
				HelperUtil.HidePopUp(HelperUtil.instance.internetPopUp);
			}));

			return;
		}

		//if (playerNameIF_TMP.text.Length < 4) return;
		if (playerName_IF.text.Length < 4) return;
		if (HelperUtil.pickedImageBytes == null)
		{
			if (playerName_IF.text.Length < 4) return;
			if (playerName_IF.text == PlayerDataHolder.PlayerName) { DeActivateGameObject(editProfilePopUp); ActivateGameObject(playerProfileHolder); return; }
			//if (playerNameIF_TMP.text.Length < 4) return;
			//if (playerNameIF_TMP.text == PlayerDataHolder.PlayerName) { DeActivateGameObject(editProfilePopUp); ActivateGameObject(playerProfileHolder); return; }
		}
		if (HelperUtil.pickedImageBytes != null)
		{
			//HelperUtil.ShowLoading();
			HelperUtil.ShowLoader();
			FirebaseAuthentication.instance.UploadImage(HelperUtil.pickedImageBytes, () =>
			{
				FirebaseAuthentication.instance.GetProfileURL((url) =>
				{
					Debug.Log("Photo URL After profile upload: " + url);
					FirebaseAuthentication.instance.UpdateProfile(FirebaseAuthentication.instance.auth.CurrentUser, string.IsNullOrEmpty(playerName_IF/*playerNameIF_TMP*/.text) ? PlayerDataHolder.PlayerName : playerName_IF/*playerNameIF_TMP*/.text, () =>
					{
						Dictionary<string, object> userProfilePic = new Dictionary<string, object>() {
						  { StaticKeywords.UserDataKeyWords.userName, FirebaseAuthentication.instance.auth.CurrentUser.DisplayName },
						  { StaticKeywords.UserDataKeyWords.profileImageUrl, FirebaseAuthentication.instance.auth.CurrentUser.PhotoUrl.ToString() }
					    };
						Database.listner = null;
						Database.OnSuccess = null;
						Database.OnSuccess = (_userData) =>
						{
							Database.listner = null;
							Database.OnSuccess = null;
							Database.OnSuccess = (_userdata) =>
							{
								UpdateProfile(true);
							};
							database.OnDatabseDocUpdate();
						};
						database.UpdateUserData(userProfilePic, () =>
						{
							Database.OnSuccess(null);
						});
					}, url);
				});
			});
		}
		else
		{
			UpdateProfile(true);
			FirebaseAuthentication.instance.UpdateProfile(FirebaseAuthentication.instance.auth.CurrentUser, string.IsNullOrEmpty(playerName_IF/*playerNameIF_TMP*/.text) ? PlayerDataHolder.PlayerName : playerName_IF/*playerNameIF_TMP*/.text, () =>
			{
				Dictionary<string, object> userProfilePic = new Dictionary<string, object>()
				{
					{ StaticKeywords.UserDataKeyWords.userName, FirebaseAuthentication.instance.auth.CurrentUser.DisplayName }
				};
				//Database.listner = null;
				//Database.OnSuccess = null;
				//Database.OnSuccess = (_userData) =>
				//{
				//	Database.listner = null;
				//	Database.OnSuccess = null;
				//	Database.OnSuccess = (_userData) =>
				//	{
						//UpdateProfile(true);
				//	};
				//	database.OnDatabseDocUpdate();
				//};
				database.UpdateUserData(userProfilePic, () =>
				{
					//Database.OnSuccess(null);
				});
			});
		}
	}

	/// <summary>
	/// Initializing reference dictionaries
	/// </summary>
	public void InitReferenceDictionaries()
	{
		for (int i = 0; i < buttonReferences.Count; i++) buttonReferenceDictionary.Add(buttonReferences[i].group, buttonReferences[i]);
		for (int i = 0; i < panels.Count; i++) panelsDictionary.Add(panels[i].name, panels[i].panel);
	}

#endregion Core

	#region UTILITY

	/// <summary>
	/// Returns button according to screen and button name entered in parameters
	/// </summary>
	/// <param name="screenName">screen name where button is located</param>
	/// <param name="buttonName">button name which is going to return</param>
	/// <returns></returns>
	public Button GetButton(string screenName, string buttonName)
	{
		if (!buttonReferenceDictionary.ContainsKey(screenName)) return null;

		return buttonReferenceDictionary[screenName].GetItem(buttonName);
	}

	/// <summary>
	/// Sets button listner according to screen name and button name
	/// </summary>
	/// <param name="screenName">screen name where button is located</param>
	/// <param name="buttonName">button name which is going to use for listner</param>
	/// <param name="action">action of listner</param>
	public void SetButton(string screenName, string buttonName, System.Action action)
	{
		Button theButton = GetButton(screenName, buttonName);
		if (theButton == null) return;
		theButton.onClick.AddListener(() => action());
	}

	#endregion

	#region Other Callbacks

	public void ResetUIData()
	{
		foreach (var playerName in playerNameText) playerName.text = "";
		foreach (var playerProfile in playerProfilePics) playerProfile.texture = defaultImage;
		foreach (var coinCount in coinCountText) coinCount.text = "0";
		foreach (var gemCount in gemCountText) gemCount.text = "0";
	}

	/// <summary>
	/// A function that updates user profile pics from all screens
	/// </summary>
	/// <param name="profileTexture">texture to update on profile pic</param>
	public void UpdateProfilePics(Texture2D profileTexture)
	{
		foreach (RawImage playerProfile in playerProfilePics) playerProfile.texture = profileTexture;
		if (playerName_IF.text.Length < 4) return;
		saveProfileButton.image.sprite = saveButtonSprites[0];
		saveProfileButton.image.raycastTarget = true;
	}

	/// <summary>
	/// Activate panel
	/// </summary>
	/// <param name="panelName">panel name to activate</param>
	public void ActivatePanel(string panelName)
	{
		//if (!GameState.isInternetConnected)
		//{
		//	HelperUtil.ShowInternetPopUp(new MessageActionData(GameState.CurrentScene == GameScene.GamePlay ? "Leave" : "Quit", () =>
		//	{
		//		if (GameState.CurrentScene == GameScene.GamePlay) HelperUtil.LoadScene(SceneName.UI);
		//		else Application.Quit();
		//	}),
		//	new MessageActionData("Retry", () =>
		//	{
		//		HelperUtil.HidePopUp(HelperUtil.instance.internetPopUp);
		//	}));

		//	return;
		//}

		//Hide all the panels first.
		foreach (Panel thePanel in panelsDictionary.Values) thePanel.Hide();

		//Enable the required panel and set the header.
		Panel panelToShow = panelsDictionary[panelName];
		panelToShow.Show();

		GameState.CurrentScreenName = panelName;
	}

	/// <summary>
	/// A function to de activate any game object
	/// </summary>
	/// <param name="gameobjectToDeActivate">game object reference to deactivate</param>
	public void DeActivateGameObject(GameObject gameobjectToDeActivate)
	{
		gameobjectToDeActivate.SetActive(false);
	}

	/// <summary>
	/// A function to activate any game object
	/// </summary>
	/// <param name="gameobjectToActivate">game object reference to activate</param>
	public void ActivateGameObject(GameObject gameobjectToActivate)
	{
		gameobjectToActivate.SetActive(true);
	}

	/// <summary>
	/// A function that validates the player name when entered in input field
	/// </summary>
	public void ValidatePlayerName()
	{
		if (playerName_IF/*playerNameIF_TMP*/.text == " ")
		{
			playerName_IF/*playerNameIF_TMP*/.text = "";
			playerName_IF/*playerNameIF_TMP*/.placeholder.GetComponent<Text/*TMPro.TextMeshProUGUI*/>().text = "";
		}
		if (!string.IsNullOrEmpty(playerName_IF/*playerNameIF_TMP*/.text) && playerName_IF/*playerNameIF_TMP*/.text.Substring(0, 1) == " ")
		{
			playerName_IF/*playerNameIF_TMP*/.text = playerName_IF/*playerNameIF_TMP*/.text.Substring(1, playerName_IF/*playerNameIF_TMP*/.text.Length - 1);
		}
		if (string.IsNullOrEmpty(playerName_IF/*playerNameIF_TMP*/.text))
		{
			playerName_IF/*playerNameIF_TMP*/.placeholder.GetComponent<Text/*TMPro.TextMeshProUGUI*/>().text = "";
			////playerName_IF.placeholder.GetComponent<ArabicFixer>().fixedText = "";
		}
		HelperUtil.CheckMinLength(playerName_IF/*playerNameIF_TMP*/, 4, () =>
		{
			ActivateGameObject(playerNameSuggession);
			saveProfileButton.image.sprite = saveButtonSprites[1];
			saveProfileButton.image.raycastTarget = false;
		},
		() =>
		{
			DeActivateGameObject(playerNameSuggession);
			if (playerName_IF/*playerNameIF_TMP*/.text != PlayerDataHolder.PlayerName)
			{
				saveProfileButton.image.sprite = saveButtonSprites[0];
				saveProfileButton.image.raycastTarget = true;
			}
			else
			{
				saveProfileButton.image.sprite = saveButtonSprites[1];
				saveProfileButton.image.raycastTarget = false;
			}
		});
	}

	/// <summary>
	/// When user cancel to edit profile
	/// </summary>
	public void UpdateProfile(bool isUpdate)
	{
		if (isUpdate)
		{
			ActivatePanel(ScreenName.UserProfile);
			ActivateGameObject(playerProfileHolder);
			PlayerDataHolder.PlayerProfilePic = HelperUtil.pickedImage == null ? PlayerDataHolder.PlayerProfilePic : HelperUtil.pickedImage;
			PlayerDataHolder.PlayerName = string.IsNullOrEmpty(playerName_IF/*playerNameIF_TMP*/.text) ? PlayerDataHolder.PlayerName : playerName_IF/*playerNameIF_TMP*/.text;
		//	PlayerDataHolder.PlayerName = FirebaseAuthentication.instance.auth.CurrentUser.DisplayName;

			if (FirebaseAuthentication.instance.auth.CurrentUser.PhotoUrl != null)
			{
				HelperUtil.SetTexture(FirebaseAuthentication.instance.auth.CurrentUser.PhotoUrl.ToString(), (downloadedTexture) =>
				{
					PlayerDataHolder.PlayerProfilePic = downloadedTexture;
					foreach (RawImage playerProfilePic in playerProfilePics) playerProfilePic.texture = PlayerDataHolder.PlayerProfilePic;
					foreach (Text playerName in playerNameText) playerName.text = PlayerDataHolder.PlayerName;
				});
			}
			foreach (RawImage playerProfilePic in playerProfilePics) playerProfilePic.texture = PlayerDataHolder.PlayerProfilePic;
			foreach (Text playerName in playerNameText) playerName.text = PlayerDataHolder.PlayerName;
			saveProfileButton.image.sprite = saveButtonSprites[1];
			saveProfileButton.image.raycastTarget = false;
			HelperUtil.pickedImageBytes = null;
		}
		else
		{
			ActivatePanel(ScreenName.UserProfile);
			ActivateGameObject(playerProfileHolder);
			foreach (RawImage playerProfilePic in playerProfilePics) playerProfilePic.texture = PlayerDataHolder.PlayerProfilePic;
			foreach (Text playerName in playerNameText) playerName.text = PlayerDataHolder.PlayerName;
			saveProfileButton.image.sprite = saveButtonSprites[1];
			saveProfileButton.image.raycastTarget = false;
			HelperUtil.pickedImageBytes = null;
		}
		playerName_IF/*playerNameIF_TMP*/.text = PlayerDataHolder.PlayerName;
		playerName_IF/*playerNameIF_TMP*/.placeholder.GetComponent<Text/*TMPro.TextMeshProUGUI*/>().text = "";
		foreach (Text playerName in playerNameText)
		{
			switch (playerName_IF/*playerNameIF_TMP*/.CheckIfEnglish(scene: 0))
			{
				case TypedLanguage.English:
					playerName_IF/*playerNameIF_TMP*/.caretPosition = playerName_IF/*playerNameIF_TMP*/.text.Length;
					break;
				case TypedLanguage.Arabic:
				case TypedLanguage.Other:
					playerName.text = ArabicFixer.Fix(playerName.text);
					break;
			}
		}
		HelperUtil.pickedImage = null;
		HelperUtil.HideLoading();
		HelperUtil.HideLoader();
	}

	/// <summary>
	/// Join gameplay room
	/// </summary>
	/// <param name="isVoiceMode">enter in voice mode or message mode</param>
	public void JoinGameRoom(bool isVoiceMode)
	{
		Debug.Log("Joining room with Voice: " + isVoiceMode);
		//HelperUtil.HideLoading();
		switch (isVoiceMode)
		{
			case true:
#if UNITY_ANDROID
				AndroidRuntimePermissions.Permission result = AndroidRuntimePermissions.RequestPermission("android.permission.RECORD_AUDIO");
				if (result == AndroidRuntimePermissions.Permission.Granted)
				{
					Debug.Log("We have permission to access external storage!");
					HelperUtil.ShowLoading();
					Settings.UseMic = true;
					HelperUtil.LoadScene(SceneName.Game);
					HelperUtil.instance.vivoxManager.JoinVivoxChannel("Demo");
				}
				else
				{
					Debug.Log("Permission state: " + result);
					HelperUtil.LoadScene(SceneName.Game);
					Settings.UseMic = false;
				}
#elif UNITY_IOS
				AudioSession.SetAudioPlayAndRecord();
				var ab = Microphone.Start(Microphone.devices[0], true, 300, 44100);

				StartCoroutine(checkMicrophoneAccess());
				IEnumerator checkMicrophoneAccess()
				{
					Debug.Log(Application.HasUserAuthorization(UserAuthorization.Microphone));
					yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
					if (Application.HasUserAuthorization(UserAuthorization.Microphone))
					{
						Debug.Log("Access Granted to Microphone");
						//HelperUtil.ShowLoading(0, true);
						HelperUtil.ShowLoader();
						Settings.UseMic = 1;
						GameState.ShowMicText = 1;
						DeActivateGameObject(micAccessPopUp);
						HelperUtil.LoadScene(SceneName.Game);
						HelperUtil.instance.vivoxManager.JoinVivoxChannel("Demo");
						database.DeductCurrency(GameInfo.requiredMoneyToPlay, StaticKeywords.UserDataKeyWords.coins, () =>
						{
						}, () =>
						{
							HelperUtil.HideLoading();
							HelperUtil.HideLoader();
							DeActivateGameObject(micAccessPopUp);
							HelperUtil.ShowRewardedAdPopUp(new MessageActionData("Watch", () => GoogleAdManager.instance.ShowRewardedAd()), new MessageActionData("Cancel", () => HelperUtil.HidePopUp(HelperUtil.instance.rewardedAdPopUp)));
						});
					}
					else
					{
						Debug.Log("Access denied to Microphone");
						if (GameState.ShowMicText != 0) OpenSettings();
						Settings.UseMic = 2;
						GameState.ShowMicText = 1;
						HelperUtil.HideLoading();
						HelperUtil.HideLoader();
						DeActivateGameObject(micAccessPopUp);
						ActivatePanel(ScreenName.Home);
						ActivateGameObject(manualMicAccessButton.gameObject);
					}
				}
#endif

				break;
			case false:
				//HelperUtil.ShowLoading(0, true);
				HelperUtil.ShowLoader();
				Settings.UseMic = 2;
				HelperUtil.LoadScene(SceneName.Game);
				break;
		}
	}

	public void OpenSettings()
    {
#if UNITY_IOS
		Application.OpenURL("app-settings:");
#elif UNITY_ANDROID
        AndroidRuntimePermissions.OpenSettings();
#endif
	}

	/// <summary>
	/// Sync account when user logged in as Guest
	/// </summary>
	/// <param name="accountType">Account type to sync</param>
	public void SyncAccount(bool shouldSync)
	{
		switch (syncAccountOption)
		{
			case StaticKeywords.AuthProvider.google:
				HelperUtil.ShowLoading();
				HelperUtil.instance.vivoxManager.LogOutVivox();
				PlayerDataHolder.syncData = shouldSync;
				GameState.NeedToSync = shouldSync;
				if (!shouldSync) Database.needToUpdateProfile = false;
				else Database.needToUpdateProfile = true;
				OnGoogleLoginButton();
				if (Database.listner != null) Database.listner.Stop();
				break;
			case StaticKeywords.AuthProvider.facebook:
				HelperUtil.ShowLoading();
				HelperUtil.instance.vivoxManager.LogOutVivox();
				GameState.NeedToSync = shouldSync;
				PlayerDataHolder.syncData = shouldSync;
				if (!shouldSync) Database.needToUpdateProfile = false;
				else Database.needToUpdateProfile = true;
				OnFacebookLoginButton();
				if (Database.listner != null) Database.listner.Stop();
				break;
			case StaticKeywords.AuthProvider.apple:
				HelperUtil.ShowLoading();
				HelperUtil.instance.vivoxManager.LogOutVivox();
				GameState.NeedToSync = shouldSync;
				PlayerDataHolder.syncData = shouldSync;
				if (!shouldSync) Database.needToUpdateProfile = false;
				else Database.needToUpdateProfile = true;
				OnAppleLoginButton();
				if (Database.listner != null) Database.listner.Stop();
				break;
		}
	}

	private void UpdatePlayerData()
	{

		foreach (Text userName in playerNameText)
			userName.text = PlayerDataHolder.PlayerName;   ////Set user name on all screens
		foreach (Text coinText in coinCountText)
			coinText.text = PlayerDataHolder._CoinCount.ToString();   ////Set Coins count
		foreach (Text gemText in gemCountText)
			gemText.text = PlayerDataHolder._GemCount.ToString();   ////Set Gems count

		if (PlayerDataHolder.PlayerProfilePic != null)
		{
			foreach (RawImage playerProfile in playerProfilePics) playerProfile.texture = PlayerDataHolder.PlayerProfilePic;
		}
	}

	public static Action OpenHomeScreen;
	public void HomeScreen()
    {
		ActivatePanel(ScreenName.Home);
    }

	public void OnUserExist()
    {
		PlayerDataHolder.SocialId = "";
		ActivateGameObject(syncProfilePopUp);
    }

	#endregion Other Callbacks

	#region IAP calling functions

	public void OnPurchaseComplete(UnityEngine.Purchasing.Product product)
	{
		IAPManager.instance.OnPurchaseComplete(product);
	}

	public void OnPurchaseCompleteForGem(UnityEngine.Purchasing.Product product)
	{
		IAPManager.instance.OnPurchaseComplete(product);
	}

	public void OnPurchaseFailed(UnityEngine.Purchasing.Product product, UnityEngine.Purchasing.PurchaseFailureReason reason)
	{
		IAPManager.instance.OnPurchaseFailed(product, reason);
	}

	public void OnIAPComplete()
    {
		foreach (Text coinsCount in coinCountText) coinsCount.text = PlayerDataHolder._CoinCount.ToString();
		foreach (Text gemText in gemCountText) gemText.text = PlayerDataHolder._GemCount.ToString();
		HelperUtil.HideLoading();
		HelperUtil.HideLoader();
	}


	#endregion IAP calling functions

	/// <summary>
	/// Resets all static values
	/// </summary>
	public static void ResetMembers()
	{
		FirebaseAuthentication.ResetMembers();
		OnRewardedAdFininsh = null;
		OnGetProfile = null;
		OnLogInFailed = null;
		Deftsoft.Authentication.FacebookAuth.OnFacebookAuthFailed = null;
		OnIAPDone = null;
		OpenHomeScreen = null;
		OnUserAlreadyExist = null;
	}
}



namespace UIManager_Exculsive
{
    [System.Serializable]
    public class UIItemReference<T>
    {
        public string group;
        [SerializeField] private List<UIItemRefData<T>> uiItems;

        private Dictionary<string, T> itemRefDataDictionary = null;

        public T GetItem(string itemName)
        {
            if (itemRefDataDictionary == null)
            {
                itemRefDataDictionary = new Dictionary<string, T>();
                for (int i = 0; i < uiItems.Count; i++) itemRefDataDictionary.Add(uiItems[i].key, uiItems[i].item);
            }

            if (itemRefDataDictionary.ContainsKey(itemName)) return itemRefDataDictionary[itemName];
            return default(T);
        }
    }

    [System.Serializable]
    public struct UIItemRefData<T>
    {
        public string key;
        public T item;
    }

    [System.Serializable]
    public struct PanelData
    {
        public string name;
        public Panel panel;
    }
}