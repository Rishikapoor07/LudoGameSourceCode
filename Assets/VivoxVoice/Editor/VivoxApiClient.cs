using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Unity.Services.Core.Editor;

namespace Unity.Services.Vivox.Editor
{
    class VivoxApiClient
    {
        static VivoxApiClient s_Instance;

        internal static VivoxApiClient Instance
        {
            get
            {
                if (s_Instance is null)
                {
                    s_Instance = new VivoxApiClient();
                }

                return s_Instance;
            }
        }

        class VivoxApiConfig
        {
            public string credentials { get; set; } = $"https://services.unity.com/api/vivox/v1/projects";


            public string services { get; set; } = "https://services.unity.com/api/auth/v1/genesis-token-exchange/unity";
        }

        CdnConfiguredEndpoint<VivoxApiConfig> m_ClientConfig;

        public VivoxApiClient()
        {
            m_ClientConfig = new CdnConfiguredEndpoint<VivoxApiConfig>();
#if ENABLE_EDITOR_GAME_SERVICES
            CloudProjectSettingsEventManager.instance.projectStateChanged += OnProjectBindChanged;
#endif
        }

        ~VivoxApiClient()
        {
#if ENABLE_EDITOR_GAME_SERVICES
            CloudProjectSettingsEventManager.instance.projectStateChanged -= OnProjectBindChanged;
#endif
        }

#if ENABLE_EDITOR_GAME_SERVICES
        private void OnProjectBindChanged()
        {
            Debug.Log("Project Bind Changed");
            if (!CloudProjectSettings.projectBound)
            {
                VivoxSettings.Instance.Server = "";
                VivoxSettings.Instance.Domain = "";
                VivoxSettings.Instance.TokenKey = "";
                VivoxSettings.Instance.TokenIssuer = "";
                VivoxSettings.Instance.Save();
            }
            else if(!VivoxSettings.Instance.IsEnvironmentCustom)
            {
                GetGatewayToken(OnGatewayTokenFetched, Debug.LogError);

                void OnGatewayTokenFetched(TokenExchangeResponse gateResp)
                {
                    GetCredentials(gateResp.Token, OnCredentialsFetched, Debug.LogError);
                    void OnCredentialsFetched(GetVivoxCredentialsResponse credResp)
                    {
                        if(VivoxSettings.Instance.IsTestMode)
                        {
                            VivoxSettings.Instance.TokenKey = credResp.Credentials.Key;
                        }
                        VivoxSettings.Instance.TokenIssuer = credResp.Credentials.Issuer;
                        VivoxSettings.Instance.Server = credResp.Credentials.Environment.ServerUri;
                        VivoxSettings.Instance.Domain = credResp.Credentials.Environment.Domain;
                        VivoxSettings.Instance.Save();
                    }
                }
            }
        }
#endif

        internal void GetCredentials(string token, Action<GetVivoxCredentialsResponse> onSuccess, Action<Exception> onError)
        {
            CreateJsonGetRequest(GetEndPointUrl, onSuccess, onError, token);
            
            string GetEndPointUrl(VivoxApiConfig config)
            {
                return $"{config.credentials}/{CloudProjectSettings.projectId}/credentials";
            }
        }

        internal void GetGatewayToken(Action<TokenExchangeResponse> onSuccess, Action<Exception> onError)
        {
            var request = new TokenExchangeRequest();
            request.Token = CloudProjectSettings.accessToken;
            CreateJsonPostRequest(GetEndPointUrl, request, onSuccess, onError, CloudProjectSettings.accessToken);

            string GetEndPointUrl(VivoxApiConfig config)
            {
                return $"{config.services}";
            }
        }

        void CreateJsonPostRequest<TRequestType, TResponseType>(
            Func<VivoxApiConfig, string> endpointConstructor, TRequestType request,
            Action<TResponseType> onSuccess, Action<Exception> onError, string token)
        {
            m_ClientConfig.GetConfiguration(OnGetConfigurationCompleted);

            void OnGetConfigurationCompleted(VivoxApiConfig configuration)
            {
                try
                {
                    var url = endpointConstructor(configuration);
                    var payload = JsonConvert.SerializeObject(request);
                    var uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(payload));
                    var postRequest = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST)
                    {
                        downloadHandler = new DownloadHandlerBuffer(),
                        uploadHandler = uploadHandler
                    };
                    postRequest.SetRequestHeader("Content-Type", "application/json;charset=utf-8");
                    Authorize(postRequest, token);
                    postRequest.SendWebRequest().completed += CreateJsonResponseHandler(onSuccess, onError);
                }
                catch (Exception reason)
                {
                    onError?.Invoke(reason);
                }
            }
        }

        void CreateJsonGetRequest<T>(
            Func<VivoxApiConfig, string> endpointConstructor, Action<T> onSuccess, Action<Exception> onError, string token)
        {
            m_ClientConfig.GetConfiguration(OnGetConfigurationCompleted);

            void OnGetConfigurationCompleted(VivoxApiConfig configuration)
            {
                try
                {
                    var url = endpointConstructor(configuration);
                    var getRequest = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET)
                    {
                        downloadHandler = new DownloadHandlerBuffer()
                    };
                    Authorize(getRequest, token);
                    getRequest.SendWebRequest().completed += CreateJsonResponseHandler(onSuccess, onError);
                }
                catch (Exception reason)
                {
                    onError?.Invoke(reason);
                }
            }
        }

        static Action<AsyncOperation> CreateJsonResponseHandler<T>(Action<T> onSuccess, Action<Exception> onError)
        {
            return JsonResponseHandler;

            void JsonResponseHandler(AsyncOperation unityOperation)
            {
                var callbackWebRequest = ((UnityWebRequestAsyncOperation)unityOperation).webRequest;
                if (WebRequestSucceeded(callbackWebRequest))
                {
                    try
                    {
                        var deserializedObject = JsonConvert.DeserializeObject<T>(
                            callbackWebRequest.downloadHandler.text);
                        onSuccess?.Invoke(deserializedObject);
                    }
                    catch (Exception deserializeError)
                    {
                        onError?.Invoke(deserializeError);
                    }
                }
                else
                {
                    onError?.Invoke(new Exception(callbackWebRequest.error));
                }

                callbackWebRequest.Dispose();
            }
        }

        static bool WebRequestSucceeded(UnityWebRequest request)
        {
#if UNITY_2020_2_OR_NEWER
            return request.result == UnityWebRequest.Result.Success;
#else
            return request.isDone && !request.isHttpError && !request.isNetworkError;
#endif
        }

        static void Authorize(UnityWebRequest request, string token)
        {
            request.SetRequestHeader("AUTHORIZATION", $"Bearer {token}");
        }
    }
}
