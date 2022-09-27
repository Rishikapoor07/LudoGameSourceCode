using System;
using VivoxUnity;

namespace Unity.Services.Vivox
{
    public class Account : AccountId
    {
        public Account(string displayname = null, string[] spokenLanguages = null) :
            base
            (
                VivoxService.Instance.Issuer,
                string.IsNullOrEmpty(VivoxService.Instance.PlayerId) ? Guid.NewGuid().ToString() : VivoxService.Instance.PlayerId,
                VivoxService.Instance.Domain,
                displayname,
                spokenLanguages,
                VivoxService.Instance.EnvironmentId
            )
        {
        }
    }
}