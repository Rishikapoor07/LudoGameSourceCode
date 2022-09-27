using System;
using System.Linq;
using System.Text;
using Unity.Services.Authentication.Internal;
using UnityEngine;
using VivoxUnity;

namespace Unity.Services.Vivox
{
    class VivoxServiceInternal : IVivoxService
    {
        /// <summary>
        /// Keys used to fetch Vivox credentials.
        /// </summary>
        internal const string k_ServerKey = "com.unity.services.vivox.server";
        internal const string k_DomainKey = "com.unity.services.vivox.domain";
        internal const string k_IssuerKey = "com.unity.services.vivox.issuer";
        internal const string k_TokenKey = "com.unity.services.vivox.token";
        internal const string k_EnvironmentCustomKey = "com.unity.services.vivox.is-environment-custom";
        internal const string k_TestModeKey = "com.unity.services.vivox.is-test-mode";

        // This is used to determine if we will attempt to generate local Vivox Access Tokens (VATs).
        // If we have a Key, Edit > Project Settings > Services > Vivox > Test Mode is true and we will generate debug Vivox Access Tokens locally.
        internal bool IsTestMode { get; set; }

        public Client Client { get; internal set; }
        public string Server { get; internal set; }
        public string Domain { get; internal set; }
        public string Issuer { get; internal set; }
        public string Key { get; internal set; }
        public bool IsEnvironmentCustom { get; internal set; }
        internal Uri ServerUri => new Uri(Server);

        public IAccessToken AccessTokenComponent { get; internal set; }
        public IPlayerId PlayerIdComponent { get; internal set; }
        public IEnvironmentId EnvironmentIdComponent { get; internal set; }
        public string AccessToken => AccessTokenComponent.AccessToken;
        public string PlayerId => PlayerIdComponent?.PlayerId;
        public string EnvironmentId => EnvironmentIdComponent?.EnvironmentId;
        public bool IsAuthenticated => (PlayerId != null && EnvironmentId != null);

        public void Initialize(VivoxConfig config = null)
        {
            string uriString = Server;

            // If credentials are provided by Udash - append EnvironmentId. Issuer will already be appended to Server Uri as provided by Udash.
            // If custom credentials are in use, do not modify the Server Uri.
            if (!IsEnvironmentCustom)
            {
                string environmentFragment = $"/{EnvironmentId}";
                uriString += environmentFragment;
            }

            Client = new Client(new Uri(uriString));
            Client.Initialize(config);
            if (IsAuthenticated && !IsTestMode && !IsEnvironmentCustom)
            {
                // Use a UAS token since we're authenticated, not trying to generate local VATs, and the enivironment isn't custom.
                Client.tokenGen = new VivoxJWTTokenGen();
            }
            else
            {
                if (IsEnvironmentCustom)
                {
                    // If the user intends to use a custom environment and a Vivox Key was provided let them know that this should only be used for local testing.
                    if (!string.IsNullOrEmpty(Key))
                    {
                        // If we're not authenticated or "Test Mode" is enabled in the Project Settings we'll want to try generating debug tokens.
                        Debug.LogWarning("[Vivox]: We've detected the use of custom credentials with a Vivox Key provided - we will generate Vivox Access Tokens locally using your Vivox Key but this should only be used for testing. "
                            + "\nWhen you are successfully generating server-side Vivox Access Tokens, please remove the Vivox Key from the InitializationOptions.SetVivoxCredentials(...) call used on the object passed into UnityServices.InitializeAsync(...)");
                        Client.tokenGen.IssuerKey = Key;
                    }
                }
                else
                {
                    // Not custom creds, should be test mode unless something is wrong or configured incorrectly.
                    if (IsTestMode)
                    {
                        // If we're not authenticated or "Test Mode" is enabled in the Project Settings we'll want to try generating debug tokens.
                        Debug.LogWarning("[Vivox]: Test Mode is enabled or you are not using the Authentication package - we will generate Vivox Access Tokens locally using your Vivox Key but this should only be used for testing.");
                        if (string.IsNullOrEmpty(Key))
                        {
                            Debug.LogError("[Vivox]: A Vivox Key couldn't be retrieved. " +
                                "Please ensure that a project is properly linked at \"Edit > Project Settings > Services > Vivox\"");
                            return;
                        }
                        Client.tokenGen.IssuerKey = Key;
                    }
                    else
                    {
                        Debug.LogError("[Vivox]: Something is wrong: either a project is not linked, or Authentication was not signed into before calling VivoxService.Instance.Initialize(). " +
                            "Please ensure that a project is properly linked at \"Edit > Project Settings > Services > Vivox\"");
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Sets the current player's Vivox credentials.
        /// </summary>
        internal void SetCredentials(string server, string domain, string issuer, string token, bool customEnvironment, bool isTestMode)
        {
            if (string.IsNullOrEmpty(server))
            {
                throw new ArgumentException($"'{nameof(server)}' is null or empty", nameof(server));
            }
            if (string.IsNullOrEmpty(domain))
            {
                throw new ArgumentException($"'{nameof(domain)}' is null or empty", nameof(domain));
            }
            if (string.IsNullOrEmpty(issuer))
            {
                throw new ArgumentException($"'{nameof(issuer)}' is null or empty", nameof(issuer));
            }

            Server = server;
            Domain = domain;
            Issuer = issuer;
            Key = token;
            IsEnvironmentCustom = customEnvironment;
            IsTestMode = isTestMode;
        }

        /// <summary>
        /// Caches the IPlayerId and IAccessToken Authentication components of a player.
        /// </summary>
        internal void SetAuthenticationComponents(IPlayerId playerIdComponent, IAccessToken accessTokenComponent, IEnvironmentId environmentIdComponent)
        {
            PlayerIdComponent = playerIdComponent;
            AccessTokenComponent = accessTokenComponent;
            EnvironmentIdComponent = environmentIdComponent;
        }
    }

    /// <summary>
    /// Class used to override the token we pass into any Vivox requests.
    /// </summary>
    internal class VivoxJWTTokenGen : VxTokenGen
    {
        public override string GetToken(string issuer = null, TimeSpan? expiration = null, string userUri = null, string action = null, string tokenKey = null, string conferenceUri = null, string fromUserUri = null)
        {
            return VivoxService.Instance.AccessToken;
        }
    }
}