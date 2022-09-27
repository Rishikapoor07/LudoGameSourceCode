#define debug
using System;
using System.Threading.Tasks;

namespace VivoxUnity
{
    public class VxTokenGen
    {
        static int defaultExpiration = 60;
        static string key = "";
        private static readonly DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static ulong serialNumber = 0;

        public string IssuerKey { get => key; set => key = value; }

        public string GetLoginToken(string fromUserUri, TimeSpan? expiration = null)
        {
            string issuer = new AccountId(fromUserUri).Issuer;
            return GetToken(issuer, expiration, null, "login", null, null, fromUserUri);
        }

        public string GetLoginToken(string issuer, string fromUserUri, TimeSpan expiration, string tokenSigningKey)
        {
            return GetToken(issuer, expiration, null, "login", tokenSigningKey, null, fromUserUri);
        }

        public string GetJoinToken(string fromUserUri, string conferenceUri, TimeSpan? expiration = null)
        {
            string issuer = new AccountId(fromUserUri).Issuer;
            return GetToken(issuer, expiration, null, "join", null, conferenceUri, fromUserUri);
        }

        public string GetJoinToken(string issuer, string fromUserUri, string conferenceUri, TimeSpan expiration, string tokenSigningKey)
        {
            return GetToken(issuer, expiration, null, "join", tokenSigningKey, conferenceUri, fromUserUri);
        }

        public string GetMuteForAllToken(string fromUserUri, string userUri, string conferenceUri, TimeSpan? expiration = null)
        {
            string issuer = new AccountId(fromUserUri).Issuer;
            return GetToken(issuer, expiration, userUri, "mute", null, conferenceUri, fromUserUri);
        }

        public string GetMuteForAllToken(string issuer, string fromUserUri, string userUri, string conferenceUri, TimeSpan expiration, string tokenSigningKey)
        {
            return GetToken(issuer, expiration, userUri, "mute", tokenSigningKey, conferenceUri, fromUserUri);
        }

        public string GetTranscriptionToken(string fromUserUri, string conferenceUri, TimeSpan? expiration = null)
        {

            string issuer = new AccountId(fromUserUri).Issuer;
            return GetToken(issuer, expiration, null, "trxn", null, conferenceUri, fromUserUri);
        }
        public string GetTranscriptionToken(string issuer, string fromUserUri, string conferenceUri, TimeSpan expiration, string tokenSigningKey)
        {
            return GetToken(issuer, expiration, null, "trxn", tokenSigningKey, conferenceUri, fromUserUri);
        }

        public virtual string GetToken(string issuer = null, TimeSpan? expiration = null, string userUri = null, string action = null, string tokenKey = null, string conferenceUri = null, string fromUserUri = null)
        {
            TimeSpan tokenExpiration = CheckExpiration(expiration);
            CheckInitialized();
            string signingKey = key;
            if (tokenKey != null)
                signingKey = tokenKey;

            if (string.IsNullOrEmpty(signingKey))
            {
                VivoxDebug.Instance.VxExceptionMessage($"Unable to generate local Vivox Access Tokens because a Vivox Key has not been provided.");
                return null;
            }

            return VivoxCoreInstance.vx_debug_generate_token(issuer, SecondsSinceUnixEpochPlusDuration(tokenExpiration), action, serialNumber++, userUri, fromUserUri, conferenceUri, signingKey);
        }

        public virtual async Task<string> GetTokenAsync(string issuer = null, TimeSpan? expiration = null, string userUri = null, string action = null, string tokenKey = null, string conferenceUri = null, string fromUserUri = null)
        {
            return GetToken(issuer, expiration, userUri, action, tokenKey, conferenceUri, fromUserUri);
        }

        private static void CheckInitialized()
        {
            if (!VxClient.Instance.Started)
            {
                throw new NotSupportedException("Method can not be called before Vivox SDK is initialized.");
            }
        }
        private static int SecondsSinceUnixEpochPlusDuration(TimeSpan? duration = null)
        {
            TimeSpan timestamp = DateTime.UtcNow.Subtract(unixEpoch);
            if (duration.HasValue)
            {
                timestamp = timestamp.Add(duration.Value);
            }

            return (int)timestamp.TotalSeconds;
        }

        private static TimeSpan CheckExpiration(TimeSpan? expiration = null)
        {
            if (expiration == null)
            {
                expiration = TimeSpan.FromSeconds(defaultExpiration);
            }
            return (TimeSpan)expiration;
        }
    }
}
