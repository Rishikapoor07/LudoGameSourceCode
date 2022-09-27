using System.Threading.Tasks;
using Unity.Services.Authentication.Internal;
using Unity.Services.Core.Configuration.Internal;
using Unity.Services.Core.Internal;
using UnityEngine;

namespace Unity.Services.Vivox
{
#if !UNITY_STANDALONE_LINUX
    class VivoxPackageInitializer : IInitializablePackage
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Register()
        {
            CoreRegistry.Instance.RegisterPackage(new VivoxPackageInitializer())
                .DependsOn<IProjectConfiguration>()
                .OptionallyDependsOn<IAccessToken>()
                .OptionallyDependsOn<IPlayerId>()
                .OptionallyDependsOn<IEnvironmentId>();
        }

        public Task Initialize(CoreRegistry registry)
        {
            try
            {
                var vivoxService = new VivoxServiceInternal();
                VivoxService.Instance = vivoxService;

                var config = registry.GetServiceComponent<IProjectConfiguration>();
                var server = config.GetString(VivoxServiceInternal.k_ServerKey);
                var domain = config.GetString(VivoxServiceInternal.k_DomainKey);
                var issuer = config.GetString(VivoxServiceInternal.k_IssuerKey);
                var token = config.GetString(VivoxServiceInternal.k_TokenKey);
                var isEnvironmentCustom = config.GetBool(VivoxServiceInternal.k_EnvironmentCustomKey);
                var isTestMode = config.GetBool(VivoxServiceInternal.k_TestModeKey);
                vivoxService.SetCredentials(server, domain, issuer, token, isEnvironmentCustom, isTestMode);
                vivoxService.SetAuthenticationComponents(registry.GetServiceComponent<IPlayerId>(), registry.GetServiceComponent<IAccessToken>(), registry.GetServiceComponent<IEnvironmentId>());
            }
            catch
            {
                Debug.LogError($"[Vivox]: Unable to initialize Vivox. "
                    + "\nPlease ensure that a project is properly linked at \"Edit > Project Settings > Services > Vivox\" if you intend to use Unity Game Services. "
                    + "\nIf you would like to use custom credentials, you can set them by creating an InitializationOptions instance, calling SetVivoxCredentials(...) on it while providing your credentials, and passing the object into UnityServices.InitializeAsync(...)");
                throw;
            }

            return Task.CompletedTask;
        }
    }
#endif
}