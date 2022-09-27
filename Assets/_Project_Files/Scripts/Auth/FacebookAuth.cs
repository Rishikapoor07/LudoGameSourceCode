using System;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;

namespace Deftsoft.Authentication
{
    public class FacebookAuth
    {
        public static Action<string> OnFacebookAuthSuccess;
        public static Action<string> OnFacebookAuthFailed;
        public FacebookAuth()
        {
            if (!FB.IsInitialized)
            {
                // Initialize the Facebook SDK
                FB.Init(InitCallback, OnHideUnity);
            }
            else
            {
                FB.ActivateApp();
            }
        }

        private void InitCallback()
        {
            if (FB.IsInitialized)
            {
                FB.ActivateApp();
            }
            else
            {
                Debug.Log("Failed to Initialize the Facebook SDK");
            }
        }

        private void OnHideUnity(bool isGameShown)
        {
            if (!isGameShown)
            {
                // Pause the game - we will need to hide
                Time.timeScale = 0;
            }
            else
            {
                // Resume the game - we're getting focus again
                Time.timeScale = 1;
            }
        }

        public void SignInWithFacebook() { OnSignIn(); }

        public void SignOut() { FB.LogOut(); }

        private void OnSignIn()
        {
            var perms = new List<string>() { "public_profile", "email" };
            FB.LogInWithReadPermissions(perms, AuthCallback);
        }

        private void AuthCallback(ILoginResult result)
        {
            if (result.Error != null)
            {
                Debug.Log(result.Error);
                OnFacebookAuthFailed(result.Error);
            }
            else if (result.Cancelled)
            {
                Debug.Log(result.Cancelled);
                OnFacebookAuthFailed(result.Error);
            }
            else
            {
                var aToken = result.AccessToken;
                PlayerDataHolder.SocialId = result.AccessToken.UserId;
                Debug.Log("Facebook Auth Success");
                
                OnFacebookAuthSuccess(aToken.TokenString);
            }
        }
    }
}
