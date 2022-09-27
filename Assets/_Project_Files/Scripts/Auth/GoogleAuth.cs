using Firebase.Extensions;
using Google;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Deftsoft.Authentication
{
	public class GoogleAuth
	{
		public static Action<string> OnGoogleAuthSuccess;
		public static Action<string> OnGoogleAuthFailed;
		public static Action<string> OnGoogleAuthCanceled;
		private GoogleSignInConfiguration configuration;

		public GoogleAuth()
		{
			configuration = new GoogleSignInConfiguration
			{
				RequestProfile = true,
				RequestIdToken = true,
				RequestEmail = true,
				WebClientId = "405967957864-b8bod1ip4g9eah2immmsmdk9787qt724.apps.googleusercontent.com"
			};
		}

		public void SignInWithGoogle() { OnSignIn(); }
		public void SignOutFromGoogle() { OnSignOut(); }

		private void OnSignIn()
		{
			GoogleSignIn.Configuration = configuration;
			GoogleSignIn.Configuration.UseGameSignIn = false;
			GoogleSignIn.Configuration.RequestIdToken = true;
			AddToInformation("Calling SignIn");

			GoogleSignIn.DefaultInstance.SignIn().ContinueWithOnMainThread(OnAuthenticationFinished);
		}

		private void OnSignOut()
		{
			AddToInformation("Calling SignOut");
			GoogleSignIn.DefaultInstance.SignOut();
		}

		private void OnAuthenticationFinished(Task<GoogleSignInUser> task)
		{
			if (task.IsFaulted)
			{
				AddToInformation("Got Unexpected Exception?!?");
				OnGoogleAuthFailed("Failed");
			}
			else if (task.IsCanceled)
			{
				AddToInformation("Canceled ");
				OnGoogleAuthCanceled("Canceled");
			}
			else
			{
				AddToInformation("Signin Successful");
				PlayerDataHolder.SocialId = task.Result.UserId;
				if (PlayerDataHolder.isGuestLogin)
				{
					PlayerDataHolder.isGuestLogin = false;
				}
				OnGoogleAuthSuccess(task.Result.IdToken);
			}
		}

		private void OnAuthenticationSilently(Task<GoogleSignInUser> task)
		{
			if (task.IsFaulted)
			{
				AddToInformation("Got Unexpected Exception?!?" );
				OnGoogleAuthFailed("Failed");
			}
			else if (task.IsCanceled)
			{
				OnGoogleAuthCanceled("Canceled");
				AddToInformation("Canceled: " + task.IsCanceled);
			}
			else
			{
				AddToInformation("Signin Successful");
				FirebaseAuthentication.OnLoginsuccess();
			}
		}

		#region Unused Features

		public void OnDisconnect()
		{
			AddToInformation("Calling Disconnect");
			GoogleSignIn.DefaultInstance.Disconnect();
		}

		public void OnSignInSilently()
		{
			GoogleSignIn.Configuration = configuration;
			GoogleSignIn.Configuration.UseGameSignIn = false;
			GoogleSignIn.Configuration.RequestIdToken = true;
			AddToInformation("Calling SignIn Silently");
			UIManager.OnGetProfile();
			//GoogleSignIn.DefaultInstance.SignInSilently().ContinueWithOnMainThread(OnAuthenticationFinished);
			//GoogleSignIn.DefaultInstance.SignIn().ContinueWithOnMainThread(OnAuthenticationFinished);
		}

		public void OnGamesSignIn()
		{
			GoogleSignIn.Configuration = configuration;
			GoogleSignIn.Configuration.UseGameSignIn = true;
			GoogleSignIn.Configuration.RequestIdToken = false;

			AddToInformation("Calling Games SignIn");
			GoogleSignIn.DefaultInstance.SignIn().ContinueWithOnMainThread(OnAuthenticationSilently);
		}
		#endregion Unused Features

		private void AddToInformation(string str)
		{
			Debug.Log(str);
		}
	}
}
