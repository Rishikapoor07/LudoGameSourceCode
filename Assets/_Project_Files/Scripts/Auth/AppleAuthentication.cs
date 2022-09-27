using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Extensions;
using AppleAuth.Interfaces;
using AppleAuth.Native;
using System.Text;
using UnityEngine;

public class AppleAuthentication
{
	private const string AppleUserIdKey = "AppleUserId";

	public static System.Action<string, string> OnAppleAuthSuccess;
	public static System.Action<string> OnAppleAuthFailed;
	public static System.Action<string> OnAppleAuthCanceled;

	private IAppleAuthManager _appleAuthManager;

    public static string fullUserName;
    public static string photoURL = "";

    public AppleAuthentication()
    {

        // If the current platform is supported
        if (AppleAuthManager.IsCurrentPlatformSupported)
        {
            // Creates a default JSON deserializer, to transform JSON Native responses to C# instances
            var deserializer = new PayloadDeserializer();
            // Creates an Apple Authentication manager with the deserializer
            this._appleAuthManager = new AppleAuthManager(deserializer);
        }

    }

    public void Update()
    {

        // Updates the AppleAuthManager instance to execute
        // pending callbacks inside Unity's execution loop
        if (this._appleAuthManager != null)
        {
            this._appleAuthManager.Update();
        }

    }

    public void AppleSignIn()
    {
        var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);

        this._appleAuthManager.LoginWithAppleId(
            loginArgs,
            credential =>
            {
                // Obtained credential, cast it to IAppleIDCredential
                var appleIdCredential = credential as IAppleIDCredential;
                if (appleIdCredential != null)
                {
                    try
                    {
                        // Apple User ID
                        // You should save the user ID somewhere in the device
                        var userId = appleIdCredential.User;
                        PlayerPrefs.SetString(StaticKeywords.UserDataKeyWords.socialId, userId);

                        // Email (Received ONLY in the first login)
                        var email = appleIdCredential.Email;

                        // Full name (Received ONLY in the first login)
                        var fullName = appleIdCredential.FullName?.GivenName;
                        Debug.Log("<color=yellow>" + "UserNameVar " + fullName + "</color>");

                        if (fullName != null)
                        {
                            fullUserName = fullName;
                        }

                        // Identity token
                        var identityToken = Encoding.UTF8.GetString(
                                    appleIdCredential.IdentityToken,
                                    0,
                                    appleIdCredential.IdentityToken.Length);
                        // Authorization code
                        var authorizationCode = Encoding.UTF8.GetString(
                                    appleIdCredential.AuthorizationCode,
                                    0,
                                    appleIdCredential.AuthorizationCode.Length);

                        // And now you have all the information to create/login a user in your system
                        CheckStatus(userId, identityToken, authorizationCode);
                    }

                    catch
                    {
                        Debug.LogError("Deftsoft: Unable to login using Apple.");
                    }
                }
            },
            error =>
            {
                // Something went wrong
                var authorizationErrorCode = error.GetAuthorizationErrorCode();
                //if (PlayerDataHolder.isGuestLogin)
                //    GameState.openProfileScreen = true;
                //else
                //    GameState.openProfileScreen = false;
                OnAppleAuthFailed("Login Denied");
            });
    }


    public void CheckStatus(string userId, string identifyToken, string authCode)
    {
        this._appleAuthManager.GetCredentialState(
          userId,
       state =>
       {
           switch (state)
           {
               case CredentialState.Authorized:

                   Debug.Log("Login successfull with apple");
                   if (PlayerDataHolder.isGuestLogin)
                   {
                       PlayerDataHolder.isGuestLogin = false;
                   }
                   OnAppleAuthSuccess(identifyToken, authCode);
                   // User ID is still valid. Login the user.
                   break;
               case CredentialState.Revoked:
                   // User ID was revoked. Go to login screen.
                   Debug.Log("Login Revoked");
                   break;

               case CredentialState.NotFound:
                   // User ID was not found. Go to login screen.
                   break;
           }
       },
       error =>
       {
           // Something went wrong
       });
    }

}
