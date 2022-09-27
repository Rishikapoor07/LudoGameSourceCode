using Deftsoft.Authentication;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Functions;
using Firebase.Storage;
using System;
using UnityEngine;

public class FirebaseAuthentication : MonoBehaviour
{
	public TextAsset guestNamesJson;
	public static Action OnLoginsuccess;
	public static Action OnSignOutsuccess;
	public static Action OnAccountDeleteSuccess;
	public static Action OnGoogleLoginSilently;

	public Database database;
	private GoogleAuth googleAuth;
	private FacebookAuth facebookAuth;
	private AppleAuthentication appleAuth;
	[HideInInspector] public GuestAuth guestAuth;
	[HideInInspector] public FirebaseAuth auth;
	public Firebase.Firestore.FirebaseFirestore firebaseFirestore;

	FirebaseStorage storage;
	StorageReference storageReference;
	string storageURL = "gs://ludo-f8e67.appspot.com/";
	public static FirebaseAuthentication instance;

	private void Awake()
	{
		instance = this;
		DontDestroyOnLoad(instance);

		database = new Database();
		googleAuth = new GoogleAuth();
		facebookAuth = new FacebookAuth();
		appleAuth = new AppleAuthentication();
		guestAuth = new GuestAuth();
		firebaseFunctions = FirebaseFunctions.DefaultInstance;
		CheckFirebaseDependencies();

		GoogleAuth.OnGoogleAuthSuccess += OnGoogleAuthSuccess;
		GoogleAuth.OnGoogleAuthCanceled += OnAuthFailed;
		GoogleAuth.OnGoogleAuthFailed += OnAuthFailed;
		AppleAuthentication.OnAppleAuthSuccess += OnAppleAuthSuccess;
		AppleAuthentication.OnAppleAuthFailed += OnAuthFailed;
		FacebookAuth.OnFacebookAuthSuccess += OnFacebookAuthSuccess;
		UIManager.OnGoogleLoginButton += SignInWithGoogle;
		UIManager.OnAppleLoginButton += SignInWithApple;
		UIManager.OnFacebookLoginButton += SignInWithFacebook;
		UIManager.OnGuestLoginButton += GuestSignIn;
		UIManager.OnSignOutButton += SignOut;
		UIManager.OnAccountDeleteButton += DeleteAccount;
		OnGoogleLoginSilently += googleAuth.OnSignInSilently;
	}
	private void Update()
	{
		if (appleAuth != null)
			appleAuth.Update();
	}
	private void CheckFirebaseDependencies(Action SuccessAction = null)
	{
		FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
		{
			if (task.IsCompleted)
			{
				if (task.Result == DependencyStatus.Available)
				{
					auth = FirebaseAuth.DefaultInstance;
					if (storage != FirebaseStorage.DefaultInstance) storage = FirebaseStorage.DefaultInstance;
					storageReference = storage.GetReferenceFromUrl(storageURL);
					firebaseFirestore = Firebase.Firestore.FirebaseFirestore.DefaultInstance;
					if (SuccessAction != null) SuccessAction();
				}
				else
					Debug.LogError("Could not resolve all Firebase dependencies: " + task.Result.ToString());
			}
			else
			{
				Debug.LogError("Dependency check was not completed. Error : " + task.Exception.Message);
			}
		});
	}

	#region Methods for Buttons

	private void SignInWithGoogle()
	{
		googleAuth.SignInWithGoogle();
	}

	private void SignInWithApple()
	{
		appleAuth.AppleSignIn();
	}

	private void SignInWithFacebook()
	{
		facebookAuth.SignInWithFacebook();
	}

	private void GuestSignIn()
	{
		SignInAnonymously();
	}

	private void SignOut()
	{
		if (GameState._LogInType == "facebook") facebookAuth.SignOut();
		if (GameState._LogInType == "google") googleAuth.SignOutFromGoogle();
		auth.SignOut();
		OnSignOutsuccess();
	}

	private void DeleteAccount()
	{
		HelperUtil.ShowLoading();
		if (Database.listner != null)
		{
			Database.listner = null;
		}
		Database.OnSuccess = null;
		DeleteUserAccount();
		if (GameState._LogInType == "google") googleAuth.SignOutFromGoogle();
	}

	#endregion Methods for Buttons

	#region Auth Success Callbacks

	private void OnGoogleAuthSuccess(string idToken)
	{
		SignInOnFirebase(StaticKeywords.AuthProvider.google, idToken);
	}

	private void OnAppleAuthSuccess(string idToken, string accessToken)
	{
		SignInOnFirebase(StaticKeywords.AuthProvider.apple, idToken, accessToken);
	}

	private void OnFacebookAuthSuccess(string idToken)
	{
		SignInOnFirebase(StaticKeywords.AuthProvider.facebook, idToken);
	}

	private void OnAuthFailed(string error)
	{
		UIManager.OnLogInFailed(error);
	}

	#endregion Auth Success Callbacks

	#region Firebase Signin method

	private void SignInOnFirebase(string provider, string idToken, string accessToken = null)
	{
		if (GameState.NeedToSync)
		{
			database.CheckUserAccount(() =>
			{
				UIManager.OnUserAlreadyExist();
				HelperUtil.HideLoading();
				return;
			}, () =>
			{
				FirebaseSignIn();
			});
		}
		else FirebaseSignIn();

		void FirebaseSignIn()
		{
			Credential credential = null;
			switch (provider)
			{
				case StaticKeywords.AuthProvider.facebook:
					credential = FacebookAuthProvider.GetCredential(idToken);
					break;
				case StaticKeywords.AuthProvider.google:
					credential = GoogleAuthProvider.GetCredential(idToken, null);
					break;
				case StaticKeywords.AuthProvider.apple:

					credential = OAuthProvider.GetCredential("apple.com", idToken, accessToken);
					break;
			}

			auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
			{
				if (task.IsFaulted)
				{
					HelperUtil.HideLoading();
					Debug.LogError("Error : " + task.Exception.Message);
				}
				else if (task.IsCanceled)
				{
					HelperUtil.HideLoading();
					Debug.LogError("Firebase login canceled");
				}
				else if (task.IsCompleted)
				{
					Debug.LogError("Firebase login Success");

					//todo
					string photoUrl = auth.CurrentUser.PhotoUrl != null ? auth.CurrentUser.PhotoUrl.ToString() : GetImageUri().ToString();   //make sure the url is not null as is in the case ofapple
					if (string.IsNullOrEmpty(PlayerDataHolder.UserId)) PlayerPrefs.SetString(StaticKeywords.UserDataKeyWords.userId, auth.CurrentUser.UserId); //set user id locally
					string username = auth.CurrentUser.DisplayName;  //in normal cases store the display name

					if (provider == StaticKeywords.AuthProvider.apple)
					{
						if (!string.IsNullOrEmpty(AppleAuthentication.fullUserName))
						{
							username = AppleAuthentication.fullUserName;  // if display name is null get the full user name
						}
					}

					if (!GameState.NeedToSync)
					{
						CreateUserOnDB(auth.CurrentUser.UserId, username, GameInfo.AppVersion, GameInfo.DeviceType, PlayerDataHolder.SocialId, photoUrl, () =>
						{
							Database.OnSuccess = null;
							Database.listner = null;
							Database.OnSuccess = (userData) =>
							{
								PlayerPrefs.SetString(StaticKeywords.UserDataKeyWords.userId, userData[StaticKeywords.UserDataKeyWords.userId].ToString());
								OnLoginsuccess();
							};
							database.OnDatabseDocUpdate(auth.CurrentUser.UserId);
						});
					}
					else
					{
						CreateSocialUser(PlayerDataHolder.UserId, username, PlayerDataHolder.SocialId, photoUrl, auth.CurrentUser.UserId, () =>
						 {
							 Database.OnSuccess = null;
							 Database.listner = null;
							 Database.OnSuccess = (userData) =>
							 {
								 PlayerPrefs.SetString(StaticKeywords.UserDataKeyWords.userId, userData[StaticKeywords.UserDataKeyWords.userId].ToString());
								 OnLoginsuccess();
								 UIManager.OpenHomeScreen();
							 };
							 database.OnDatabseDocUpdate(auth.CurrentUser.UserId);
						 });
					}
				}
			});
		}
	}

	private System.Threading.Tasks.Task<string> CreateSocialUser(string userId, string displayName, string socialID, string profileURL,string newUid, Action onCompletion)
	{
		// Create the arguments to the callable function.
		var data = new System.Collections.Generic.Dictionary<string, object>();

		data.Add("uid", userId);
		data.Add("name", displayName);
		data.Add("newUid", newUid);
		if (!String.IsNullOrEmpty(socialID))
		{
			data.Add("profile", profileURL);
			data.Add("socialId", socialID);
		}
		// Call the function and extract the operation from the result.
		var function = firebaseFunctions.GetHttpsCallable("syncUser");
		return function.CallAsync(data).ContinueWithOnMainThread((task) =>
		{
			task.ContinueWithOnMainThread((result) =>
			{
				if (result.IsCompleted)
				{
					Debug.Log("Function called: " + result.Result.Data.ToString());
					if (onCompletion != null)
					{
						onCompletion();
					}
				}
				else if (result.IsFaulted || result.IsCanceled)
				{
					HelperUtil.HideLoading();
					Debug.Log("Error: " + result.Exception.ToString());
				}
			});
			return (string)task.Result.Data;
		});
	}

	public void WriteData(FirebaseUser user, string loginMode)
	{
		if (user != null)
		{
			WriteUserOnDatabase(user, loginMode, GameInfo.AppVersion, GameInfo.DeviceType, PlayerDataHolder.SocialId, PlayerDataHolder.syncData);
		}
		else
		{
			Debug.Log("Login Failed");
		}
	}


	public void WriteUserOnDatabase(FirebaseUser user, string loginMode, string appVersion, string deviceType, string socialId, bool shouldSync)
	{
		user.TokenAsync(true).ContinueWithOnMainThread(task =>
		{
			if (task.IsCompleted)
			{
				string photoUrl = user.PhotoUrl != null ? user.PhotoUrl.ToString() : GetImageUri().ToString();

				if (string.IsNullOrEmpty(PlayerDataHolder.UserId)) PlayerPrefs.SetString(StaticKeywords.UserDataKeyWords.userId, user.UserId);

				UserDataHolder userDataHolder = new UserDataHolder();

				if (loginMode == StaticKeywords.AuthProvider.apple)  //if logging in from apple
				{
					Debug.Log("Apple name: " + AppleAuthentication.fullUserName);
					if (!string.IsNullOrEmpty(AppleAuthentication.fullUserName))   //if user name is not empty
					{
						UpdateProfile(user, AppleAuthentication.fullUserName, () =>
						{
							Debug.Log("User name and profile updated!!!");
							userDataHolder = new UserDataHolder(photoUrl, PlayerDataHolder.UserId, AppleAuthentication.fullUserName, loginMode, appVersion, deviceType, socialId);
							database.WriteNewUser(userDataHolder, shouldSync, OnLoginsuccess);
						}, GetImageUri());
					}
					else
					{
						Debug.Log("Apple name is null: " + user.DisplayName);
						userDataHolder = new UserDataHolder(photoUrl, PlayerDataHolder.UserId, user.DisplayName, loginMode, appVersion, deviceType, socialId);
						database.WriteNewUser(userDataHolder, shouldSync, OnLoginsuccess);
					}
				}
				else
				{
					userDataHolder = new UserDataHolder(photoUrl, PlayerDataHolder.UserId, user.DisplayName, loginMode, appVersion, deviceType, socialId);
					database.WriteNewUser(userDataHolder, shouldSync, OnLoginsuccess);
				}
			}
		});
	}

	#endregion Firebase Signin methods

	public enum TypesOfCurrency
	{
		None,
		pack1,
		pack2,
		pack3,
		pack4
	}

	#region Other Methods
	[HideInInspector] public FirebaseFunctions firebaseFunctions;

	public System.Threading.Tasks.Task<string> UpdateCurrency(string type, string itemType = null, TypesOfCurrency currency = TypesOfCurrency.None, System.Action onCompletion = null)
	{
		// Create the arguments to the callable function.
		var data = new System.Collections.Generic.Dictionary<string, object>();

		data.Add("type", type);
		if (type == "ads") { }
		else if (type == "inApp")
		{
			data.Add("itemType", itemType);
			if (currency != TypesOfCurrency.None)
			{
				data.Add("value", (int)currency);
			}

		}

		// Call the function and extract the operation from the result.
		var function = firebaseFunctions.GetHttpsCallable("updateUserCurrency");
		return function.CallAsync(data).ContinueWithOnMainThread((task) =>
		{
			task.ContinueWithOnMainThread((result) =>
			{
				if (result.IsCompleted)
				{
					Debug.Log("Function called: " + result.ToString());
					Debug.Log("Cost Updated");
					if (onCompletion != null)
					{
						onCompletion();
					}

				}
				else if (result.IsFaulted || result.IsCanceled)
				{
					Debug.Log("Error: " + result.Exception.ToString());
				}
			});
			return (string)task.Result.Data;
		});
	}

	public System.Threading.Tasks.Task<string> CreateUserOnDB(string userId, string userName, string appVersion, string deviceInfo, string socialId = "", string displayImageURL = "", System.Action onCompletion = null)
	{
		// Create the arguments to the callable function.
		var data = new System.Collections.Generic.Dictionary<string, object>();

		data.Add("uid", userId);
		data.Add("name", userName);
		data.Add("appVersion", appVersion);
		data.Add("deviceInfo", deviceInfo);
		if (!String.IsNullOrEmpty(socialId))
		{
			data.Add("profile", displayImageURL);
			data.Add("socialId", socialId);
		}
		Debug.Log("user id "+userId+ " name "+userName+" version" +appVersion+" device"+ deviceInfo+" social"+ socialId+" profile"+ displayImageURL);

		// Call the function and extract the operation from the result.
		var function = firebaseFunctions.GetHttpsCallable("createUser");
		Debug.Log("called firebase method");

		return function.CallAsync(data).ContinueWithOnMainThread((task) =>
		{
			task.ContinueWithOnMainThread((result) =>
			{
				if (result.IsCompleted)
				{
					Debug.Log("Function called: " + result.Result.Data.ToString());
					if (onCompletion != null)
					{
						onCompletion();
					}

				}
				else if (result.IsFaulted || result.IsCanceled)
				{
					Debug.Log("Error: " + result.Exception.ToString());
				}
			});
			return (string)task.Result.Data;
		});
	}

	#endregion


	// Guest Login
	private void SignInAnonymously()
	{
		auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
		{
			if (task.IsCanceled)
			{
				HelperUtil.HideLoading();
				Debug.LogError("SignInAnonymouslyAsync was canceled.");
				return;
			}
			else if (task.IsFaulted)
			{
				HelperUtil.HideLoading();
				Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
				return;
			}
			else if (task.IsCompleted)
			{
				Debug.Log("Guest login successfully!!!");
				FirebaseUser user = auth.CurrentUser;//task.Result;
				UpdateProfile(user, guestAuth.GetGuestName(guestNamesJson.text), () =>
				{
					Debug.LogFormat("User signed in successfully: {0} ({1})", user.DisplayName, user.UserId);
					//toreplicate
					PlayerPrefs.SetString(StaticKeywords.UserDataKeyWords.userId, user.UserId);
					CreateUserOnDB(user.UserId, user.DisplayName, GameInfo.AppVersion, GameInfo.DeviceType, "", "", () =>
					  {
						  Database.OnSuccess = null;
						  Database.listner = null;
						  Database.OnSuccess = (userData) => OnLoginsuccess();
						  Debug.Log("Id: " + PlayerDataHolder.UserId);
						  database.OnDatabseDocUpdate(user.UserId);
					  });
					//WriteData(user, StaticKeywords.AuthProvider.guest);
				}, GetImageUri());
			}
		});
	}

	public void UpdateProfile(FirebaseUser currentUser, string newName, Action SuccessCallback, Uri newImageUri = null)
	{
		if (newImageUri == null)
			newImageUri = currentUser.PhotoUrl;

		UserProfile profile = new UserProfile
		{
			DisplayName = newName,
			PhotoUrl = newImageUri
		};

		currentUser.UpdateUserProfileAsync(profile).ContinueWithOnMainThread((task) =>
		{
			if (task.IsFaulted)
			{
				HelperUtil.HideLoading();
				Debug.LogError("Update profile failed: " + task.Exception.ToString());
			}
			else if (task.IsCanceled)
			{
				HelperUtil.HideLoading();
				Debug.LogError("Update profile canceled");
			}
			else if (task.IsCompleted)
			{
				Debug.Log("Update profile Success");
				SuccessCallback();
			}
		});
	}

	private void DeleteUserAccount()
	{
		FirebaseUser user = auth.CurrentUser;
		string tempUserId = PlayerDataHolder.UserId;
		if (user != null)
		{
			//database.DeleteUser(PlayerDataHolder.UserId, OnAccountDeleteSuccess);
			database.DeleteUser(tempUserId, () =>
			{
				// Delete user account
				user.DeleteAsync().ContinueWithOnMainThread(task =>
				{
					if (task.IsCanceled)
					{
						HelperUtil.HideLoading();
						Debug.LogError("DeleteAsync was canceled.");
					}
					else if (task.IsFaulted)
					{
						HelperUtil.CallAfterDelay(OnAccountDeleteSuccess, 3);
						Debug.LogError("DeleteAsync encountered an error: " + task.Exception);
					}
					else if (task.IsCompleted)
					{
						Debug.Log("User deleted successfully.");
						OnAccountDeleteSuccess();
					}
				});
			});

			// Create a reference to the file to delete.
			StorageReference storgeRef = storageReference.Child(tempUserId + "/userProfile.jpeg");
			storgeRef.DeleteAsync().ContinueWithOnMainThread(task =>
			{
				if (task.IsCompleted)
				{
					Debug.Log("File deleted successfully.");
				}
				else
				{
					Debug.Log("Error in file deletion!!!");
					HelperUtil.HideLoading();
				}
			});
		}
	}

	public void UploadImage(byte[] imageByteArray, Action OnSuccess)
	{
		var newMeta = new MetadataChange();
		newMeta.ContentType = "image/jpeg";

		//Create a reference to where the file needs to be uploaded
		StorageReference uploadRef = storageReference.Child(PlayerDataHolder.UserId + "/userProfile.jpeg");
		uploadRef.PutBytesAsync(imageByteArray, newMeta).ContinueWithOnMainThread((task) =>
		{
			if (task.IsFaulted || task.IsCanceled)
			{
				HelperUtil.HideLoader();
				HelperUtil.HideLoading();
				Debug.Log(task.Exception.ToString());
			}
			else
			{
				Debug.Log("File Uploaded Successfully!");
				OnSuccess();
			}
		});
	}

	Uri urlToReturn = null;
	public void GetProfileURL(Action<Uri> OnSuccess)
	{
		StorageReference gsReference = storage.GetReferenceFromUrl(storageURL + PlayerDataHolder.UserId + "/userProfile.jpeg");
		// Fetch the download URL
		gsReference.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
		{
			if (!task.IsFaulted && !task.IsCanceled)
			{
				urlToReturn = task.Result;
				OnSuccess(urlToReturn);
			}
		});
	}

	public Uri GetImageUri()
	{
		return new Uri("https://firebasestorage.googleapis.com/v0/b/ludo-f8e67.appspot.com/o/User%20profile.png?alt=media");
	}

	public static void ResetMembers()
	{
		////UnSubscribe event methods
		OnLoginsuccess = null;
		OnSignOutsuccess = null;
		OnAccountDeleteSuccess = null;
	}

}