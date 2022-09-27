using System;

namespace VivoxUnity
{
    /// <summary>
    /// Used during initialization to set up the SDK with a custom configuration.
    /// </summary>
    public class VivoxConfig
    {
        private vx_sdk_config_t vx_sdk_config = new vx_sdk_config_t();
        
        /// <summary>
        /// Internally used for mapping to the Core SDK type.
        /// </summary>
        public vx_sdk_config_t ToVx_Sdk_Config()
        {
            return vx_sdk_config;
        }

        /// <summary>
        /// The number of threads used for encoding/decoding audio. Must be 1 for client SDKs.
        /// </summary>
        public int CodecThreads
        {
            get { return vx_sdk_config.num_codec_threads; }
            set { vx_sdk_config.num_codec_threads = value; }
        }

        /// <summary>
        /// The number of threads used for voice processing. Must be 1 for client SDKs.
        /// </summary>
        public int VoiceThreads
        {
            get { return vx_sdk_config.num_voice_threads; }
            set { vx_sdk_config.num_voice_threads = value; }
        }

        /// <summary>
        /// The number of threads used for web requests. Must be 1 for client SDKs.
        /// </summary>
        public int WebThreads
        {
            get { return vx_sdk_config.num_web_threads; }
            set { vx_sdk_config.num_web_threads = value; }
        }

        /// <summary>
        /// The render source maximum queue depth.
        /// </summary>
        public int RenderSourceQueueDepthMax
        {
            get { return vx_sdk_config.render_source_queue_depth_max; }
            set { vx_sdk_config.render_source_queue_depth_max = value; }
        }

        /// <summary>
        /// The render source initial buffer count.
        /// </summary>
        public int RenderSourceInitialBufferCount
        {
            get { return vx_sdk_config.render_source_initial_buffer_count; }
            set { vx_sdk_config.render_source_initial_buffer_count = value; }
        }

        /// <summary>
        /// The upstream jitter frame count.
        /// </summary>
        public int UpstreamJitterFrameCount
        {
            get { return vx_sdk_config.upstream_jitter_frame_count; }
            set { vx_sdk_config.upstream_jitter_frame_count = value; }
        }

        /// <summary>
        /// The maximum number of logins per user.
        /// </summary>
        public int MaxLoginsPerUser
        {
            get { return vx_sdk_config.max_logins_per_user; }
            set { vx_sdk_config.max_logins_per_user = value; }
        }

        /// <summary>
        /// The initial log level.
        /// Severity level of logs: -1 = no logging, 0 = errors only, 1 = warnings, 2 = info, 3 = debug, 4 = trace, 5 = log all
        /// </summary>
        public vx_log_level InitialLogLevel
        {
            get { return vx_sdk_config.initial_log_level; }
            set { vx_sdk_config.initial_log_level = value; }
        }

        /// <summary>
        /// Disable audio device polling by using a timer.
        /// </summary>
        public bool DisableDevicePolling
        {
            get { return Convert.ToBoolean(vx_sdk_config.disable_device_polling); }
            set { vx_sdk_config.disable_device_polling = Convert.ToInt32(value); }
        }

        /// <summary>
        /// Used for diagnostic purposes only.
        /// </summary>
        public bool ForceCaptureSilence
        {
            get { return Convert.ToBoolean(vx_sdk_config.force_capture_silence); }
            set { vx_sdk_config.force_capture_silence = Convert.ToInt32(value); }
        }

        /// <summary>
        /// Enable advanced automatic settings for audio levels.
        /// </summary>
        public bool EnableAdvancedAutoLevels
        {
            get { return Convert.ToBoolean(vx_sdk_config.enable_advanced_auto_levels); }
            set { vx_sdk_config.enable_advanced_auto_levels = Convert.ToInt32(value); }
        }

        /// <summary>
        /// The number of 20 millisecond buffers for the capture device.
        /// </summary>
        public int CaptureDeviceBufferSizeIntervals
        {
            get { return vx_sdk_config.capture_device_buffer_size_intervals; }
            set { vx_sdk_config.capture_device_buffer_size_intervals = value; }
        }

        /// <summary>
        /// The number of 20 millisecond buffers for the render device.
        /// </summary>
        public int RenderDeviceBufferSizeIntervals
        {
            get { return vx_sdk_config.render_device_buffer_size_intervals; }
            set { vx_sdk_config.render_device_buffer_size_intervals = value; }
        }

        /// <summary>
        /// Disable audio ducking.
        /// </summary>
        public bool DisableAudioDucking
        {
            get { return Convert.ToBoolean(vx_sdk_config.disable_audio_ducking); }
            set { vx_sdk_config.disable_audio_ducking = Convert.ToInt32(value); }
        }

        /// <summary>
        /// Default of 1 for most platforms. Caution: Changes to this value must be coordinated with Vivox.
        /// </summary>
        public bool EnableDtx
        {
            get { return Convert.ToBoolean(vx_sdk_config.enable_dtx); }
            set { vx_sdk_config.enable_dtx = Convert.ToInt32(value); }
        }

        /// <summary>
        /// The default codec mask that is used to initialize a connector's configured_codecs.
        /// Codec type <see cref="Codec"/>
        /// </summary>
        public MediaCodecType DefaultCodecsMask
        {
            get { return (MediaCodecType)vx_sdk_config.default_codecs_mask; }
            set { vx_sdk_config.default_codecs_mask = (uint)value; }
        }

        /// <summary>
        /// Enable fast network change detection. By default, this is disabled.
        /// </summary>
        public bool EnableFastNetworkChangeDetection
        {
            get { return Convert.ToBoolean(vx_sdk_config.enable_fast_network_change_detection); }
            set { vx_sdk_config.enable_fast_network_change_detection = Convert.ToInt32(value); }
        }

        /// <summary>
        /// Use operating system-configured proxy settings (Windows only). The default is 0. If the environment variable "VIVOX_USE_OS_PROXY_SETTINGS" is set, then this defaults to 1.
        /// </summary>
        public int UseOsProxySettings
        {
            get { return vx_sdk_config.use_os_proxy_settings; }
            set { vx_sdk_config.use_os_proxy_settings = value; }
        }

        /// <summary>
        /// Enable dynamic voice processing switching. The default value is true.
        /// If enabled, the SDK automatically switches between hardware and software AECs.
        /// To disable this, set the value to 0.
        /// </summary>
        public bool DynamicVoiceProcessingSwitching
        {
            get { return Convert.ToBoolean(vx_sdk_config.dynamic_voice_processing_switching); }
            set { vx_sdk_config.dynamic_voice_processing_switching = Convert.ToInt32(value); }
        }

        /// <summary>
        /// The number of millseconds to wait before disconnecting audio due to RTP timeout at the initial call time. A zero or negative value turns off the guard (this is not recommended).
        /// </summary>
        public int NeverRtpTimeoutMs
        {
            get { return vx_sdk_config.never_rtp_timeout_ms; }
            set { vx_sdk_config.never_rtp_timeout_ms = value; }
        }

        /// <summary>
        /// The number of millseconds to wait before disconnecting audio due to RTP timeout after the call has been established. A zero or negative value turns off the guard (this is not recommended).
        /// </summary>
        public int LostRtpTimeoutMs
        {
            get { return vx_sdk_config.lost_rtp_timeout_ms; }
            set { vx_sdk_config.lost_rtp_timeout_ms = value; }
        }

        /// <summary>
        /// For iOS, set this to true to control the iOS PlayAndRecord category.
        /// If set to false, Vivox sets the proper iOS PlayAndRecord category.
        /// Note: You must set the PlayAndRecord category for simultaneous input/output.
        /// An improper PlayAndRecord category can result in loss of voice communication.  
        /// Defaulting to a speaker plays from speaker hardware instead of the receiver (ear speaker) when headphones are not used.
        /// </summary>
        public bool SkipPrepareForVivox { get; set; }
    }
}
