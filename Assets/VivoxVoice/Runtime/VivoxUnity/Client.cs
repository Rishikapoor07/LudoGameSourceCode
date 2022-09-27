using System;
using System.Collections.Generic;
using System.Threading;
using VivoxUnity.Common;
using VivoxUnity.Private;

namespace VivoxUnity
{
    /// <summary>
    /// Provide access to the Vivox system.
    /// Note: An application should have only one Client object. 
    /// </summary>
    public sealed class Client : IDisposable
    {
        #region Member Variables
        private readonly ReadWriteDictionary<AccountId, ILoginSession, LoginSession> _loginSessions = new ReadWriteDictionary<AccountId, ILoginSession, LoginSession>();
        private readonly AudioInputDevices _inputDevices = new AudioInputDevices(VxClient.Instance);
        private readonly AudioOutputDevices _outputDevices = new AudioOutputDevices(VxClient.Instance);
        private readonly Uri _serverUri;
        private static int _nextHandle;
        private string _connectorHandle;
        private readonly Queue<IAsyncResult> _pendingConnectorCreateRequests = new Queue<IAsyncResult>();
        private bool _ttsIsInitialized;

        private uint _ttsManagerId;
        internal uint TTSManagerId => _ttsManagerId;

        public static VxTokenGen tokenGen
        {
            get { return VxClient.Instance.tokenGen; }
            set { VxClient.Instance.tokenGen = value; }
        }

        /// <summary>
        /// The domain that the server determines during client connector creation. Otherwise, this is NULL.
        /// </summary>
        public static string defaultRealm
        {
            get {return VxClient.Instance.defaultRealm; }
            private set { VxClient.Instance.defaultRealm = value; }
        }
        #endregion

        #region Helpers

        void CheckInitialized()
        {
            if (!Initialized || VxClient.PlatformNotSupported)
                throw new InvalidOperationException();
        }

        #endregion

        public Client(Uri serverUri = null)
        {
            _serverUri = serverUri;
        }

        /// <summary>
        /// Initialize this Client instance.
        /// Note: If this Client instance is already initialized, then this does nothing.
        /// <param name="config">Optional: config to set on initialize.</param>
        /// </summary>
        public void Initialize(VivoxConfig config = null)
        {
            if (Initialized || VxClient.PlatformNotSupported)
                return;

            VxClient.Instance.Start(config);

            // Refresh audio devices to ensure they are up-to-date when the client is initialized.
            AudioInputDevices.BeginRefresh(null);
            AudioOutputDevices.BeginRefresh(null);
        }

        internal IAsyncResult BeginGetConnectorHandle(AsyncCallback callback)
        {
            if (_serverUri == null)
            {
                throw new NullReferenceException($"{nameof(_serverUri)} is Null. If not using Unity Game Services, use BeginGetConnectorHandle(Uri server, AsyncCallback callback)");
            }

            return BeginGetConnectorHandle(_serverUri, callback);
        }

        internal IAsyncResult BeginGetConnectorHandle(Uri server, AsyncCallback callback)
        {
            CheckInitialized();

            var result = new AsyncResult<string>(callback);
            if (!string.IsNullOrEmpty(_connectorHandle))
            {
                result.SetCompletedSynchronously(_connectorHandle);
                return result;
            }

            _pendingConnectorCreateRequests.Enqueue(result);
            if (_pendingConnectorCreateRequests.Count > 1)
            {
                return result;
            }

            var request = new vx_req_connector_create_t();
            var response = new vx_resp_connector_create_t();
            request.acct_mgmt_server = server.ToString();
            string connectorHandle = $"C{_nextHandle++}";
            request.connector_handle = connectorHandle;
            VxClient.Instance.BeginIssueRequest(request, ar =>
            {
                try
                {
                    response = VxClient.Instance.EndIssueRequest(ar);
                }
                catch (Exception e)
                {
                    VivoxDebug.Instance.VxExceptionMessage($"{request.GetType().Name} failed: {e}");
                    _connectorHandle = null;
                    while (_pendingConnectorCreateRequests.Count > 0)
                    {
                        ((AsyncResult<string>)(_pendingConnectorCreateRequests).Dequeue()).SetComplete(e);
                    }
                    if (VivoxDebug.Instance.throwInternalExcepetions)
                    {
                        throw;
                    }
                    return;
                }
                _connectorHandle = connectorHandle;
                if(!string.IsNullOrEmpty(response.default_realm))
                {
                    defaultRealm = response.default_realm;
                }
                while (_pendingConnectorCreateRequests.Count > 0)
                {
                    ((AsyncResult<string>)(_pendingConnectorCreateRequests).Dequeue()).SetComplete(_connectorHandle);
                }
            });
            return result;
        }

        internal string EndGetConnectorHandle(IAsyncResult result)
        {
            return ((AsyncResult<string>)result).Result;
        }

        internal void RemoveLoginSession(AccountId accountId)
        {
            _loginSessions.Remove(accountId);
        }

        internal void AddLoginSession(AccountId accountId, LoginSession session)
        {
            if(!_loginSessions.ContainsKey(accountId))
            {
                _loginSessions[accountId] = session;
            }
        }

        /// <summary>
        /// Uninitialize this Client instance.
        /// Note: If this Client instance is not initialized, then this does nothing.
        /// </summary>
        public void Uninitialize()
        {
            if (Initialized && !VxClient.PlatformNotSupported)
            {
                VxClient.Instance.Stop();
                TTSShutdown();
                _inputDevices.Clear();
                _outputDevices.Clear();
                _loginSessions.Clear();
                _connectorHandle = null;
            }
        }

        public static void Cleanup()
        {
            if (VxClient.PlatformNotSupported) return;
            VxClient.Instance.Cleanup();
            
            VivoxCoreInstance.Uninitialize();
        }

        /// <summary>
        /// The internal version of the low-level vivoxsdk library.
        /// </summary>
        public static string InternalVersion => VxClient.GetVersion();

        /// <summary>
        /// Gets the LoginSession object for the provided accountId, and creates one if necessary.
        /// </summary>
        /// <param name="accountId">The AccountId.</param>
        /// <returns>The login session for the accountId.</returns>
        /// <exception cref="ArgumentNullException">Thrown when accountId is null or empty.</exception>
        /// <remarks>If a new LoginSession is created, then LoginSessions.AfterKeyAdded is raised.</remarks>
        public ILoginSession GetLoginSession(AccountId accountId)
        {
            if (AccountId.IsNullOrEmpty(accountId))
                throw new ArgumentNullException(nameof(accountId));

            CheckInitialized();
            if (_loginSessions.ContainsKey(accountId))
            {
                return _loginSessions[accountId];
            }
            var loginSession = new LoginSession(this, accountId);
            _loginSessions[accountId] = loginSession;
            return loginSession;
        }

        /// <summary>
        /// Specifies whether the client is initialized: True if initialized, false if uninitialized. 
        /// Note: The state of this is managed by the Core SDK; the wrapper is forwarding the information.
        /// </summary>
        public bool Initialized
        {
            get
            {
                if (VxClient.PlatformNotSupported) return false;
                return Convert.ToBoolean(VivoxCoreInstancePINVOKE.vx_is_initialized());
            }
        }

        /// <summary>
        /// All of the Login sessions associated with this Client instance.
        /// </summary>
        public IReadOnlyDictionary<AccountId, ILoginSession> LoginSessions => _loginSessions;
        /// <summary>
        /// The audio input devices associated with this Client instance.
        /// </summary>
        public IAudioDevices AudioInputDevices => _inputDevices;
        /// <summary>
        /// The audio output devices associated with this Client instance.
        /// </summary>
        public IAudioDevices AudioOutputDevices => _outputDevices;

        /// <summary>
        /// Indicates whether Vivox's software echo cancellation feature is enabled.
        /// Note: This is completely independent of any hardware-provided acoustic echo cancellation that might be available for a device.
        /// </summary>
        public bool IsAudioEchoCancellationEnabled
        {
            get
            {
                if (VxClient.PlatformNotSupported) return false;
                return VivoxCoreInstance.IsAudioEchoCancellationEnabled();
            }
        }

        /// <summary>
        /// Turn Vivox's audio echo cancellation feature on or off.
        /// </summary>
        /// <param name="onOff">True for on, False for off.</param>
        public void SetAudioEchoCancellation(bool onOff)
        {
            CheckInitialized();

            if (IsAudioEchoCancellationEnabled != onOff)
            {
                VivoxCoreInstance.vx_set_vivox_aec_enabled(Convert.ToInt32(onOff));
            }
        }

        void IDisposable.Dispose()
        {
            Uninitialize();
        }

        internal static string GetRandomUserId(string prefix)
        {
            return Helper.GetRandomUserId(prefix);
        }

        internal static string GetRandomChannelUri(string prefix, string realm)
        {
            return Helper.GetRandomChannelUri(prefix, realm);
        }

        public static void Run(LoopDone done)
        {
            MessagePump.Instance.RunUntil(done);
        }

        public static bool Run(WaitHandle handle, TimeSpan until)
        {
            DateTime then = DateTime.Now + until;
            MessagePump.Instance.RunUntil(() => MessagePump.IsDone(handle, then));
            if (handle != null) return handle.WaitOne(0);
            return false;
        }

        /// <summary>
        /// Process all asynchronous messages. 
        /// This must be called periodically by the application at a frequency of no less than every 100ms.
        /// </summary>
        public static void RunOnce()
        {
            MessagePump.Instance.RunUntil(() => MessagePump.IsDone(null, DateTime.Now));
        }

        internal bool TTSInitialize()
        {
            if (!_ttsIsInitialized && !VxClient.PlatformNotSupported)
            {
                // NB: When there is more than one tts_engine type available, we will need to make a public TTSInitialize() method.
                vx_tts_status status = VivoxCoreInstance.vx_tts_initialize(vx_tts_engine_type.tts_engine_vivox_default, out _ttsManagerId);
                if (vx_tts_status.tts_status_success == status)
                    _ttsIsInitialized = true;
            }
            return _ttsIsInitialized;
        }

        internal void TTSShutdown()
        {
            if (_ttsIsInitialized)
            {
                var status = VivoxCoreInstance.vx_tts_shutdown();
                foreach (ILoginSession session in _loginSessions)
                {
                    ((TextToSpeech)session.TTS).CleanupTTS();
                }
                _ttsIsInitialized = false;
            }
        }
    }
}
