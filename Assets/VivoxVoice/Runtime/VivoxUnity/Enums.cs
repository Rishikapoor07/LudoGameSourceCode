using System.ComponentModel;

namespace VivoxUnity
{
    /// <summary>
    /// The state of the Login session.
    /// </summary>
    [DefaultValue(LoggedOut)]
    public enum LoginState
    {
        /// <summary>
        /// The Login session is signed out.
        /// </summary>
        LoggedOut = vx_login_state_change_state.login_state_logged_out,
        /// <summary>
        /// The Login session is signed in.
        /// </summary>
        LoggedIn = vx_login_state_change_state.login_state_logged_in,
        /// <summary>
        /// The Login session is in the process of signing in.
        /// </summary>
        LoggingIn = vx_login_state_change_state.login_state_logging_in,
        /// <summary>
        /// The Login session is in the process of signing out.
        /// </summary>
        LoggingOut = vx_login_state_change_state.login_state_logging_out
    }

    /// <summary>
    /// Determine how to handle incoming subscriptions.
    /// </summary>
    public enum SubscriptionMode
    {
        /// <summary>
        /// Automatically accept all incoming subscription requests.
        /// </summary>
        Accept = vx_buddy_management_mode.mode_auto_accept,
        /// <summary>
        /// Automatically block all incoming subscription requests.
        /// </summary>
        Block = vx_buddy_management_mode.mode_block,
        /// <summary>
        /// Defer incoming subscription request handling to the application. 
        /// In this scenario, the IncomingSubscriptionRequests collection raises the AfterItemAdded event.
        /// </summary>
        Defer = vx_buddy_management_mode.mode_application,
    }

    /// <summary>
    /// The online status of the user.
    /// </summary>
    public enum PresenceStatus
    {
        /// <summary>
        /// Generally available
        /// </summary>
        Available = vx_buddy_presence_state.buddy_presence_online,
        /// <summary>
        /// Do Not Disturb
        /// </summary>
        DoNotDisturb = vx_buddy_presence_state.buddy_presence_busy,
        /// <summary>
        /// Away
        /// </summary>
        Away = vx_buddy_presence_state.buddy_presence_away,
        /// <summary>
        /// Currently in a call
        /// </summary>
        InACall = vx_buddy_presence_state.buddy_presence_onthephone,
        /// <summary>
        /// Not available (offline)
        /// </summary>
        Unavailable = vx_buddy_presence_state.buddy_presence_offline,
        /// <summary>
        /// Available to chat
        /// </summary>
        Chat = vx_buddy_presence_state.buddy_presence_chat,
        /// <summary>
        /// Away for an extended period of time
        /// </summary>
        ExtendedAway = vx_buddy_presence_state.buddy_presence_extended_away
    }

    /// <summary>
    /// Determine how often the SDK sends participant property events while in a channel.
    /// </summary>
    /// <remarks>
    /// <para>By default, participant property events send on participant state change (for example, when a user starts talking, stops talking, is muted, or is unmuted). If set to a per second rate, messages send at that rate if there has been a change since the last update message. This is always true unless the participant is muted through the SDK, which causes no audio energy or state changes.</para>
    /// <para>Caution: Setting this to a non-default value increases user and server traffic. This should only be done if a real-time visual representation of audio values are needed (for example, a graphic VAD indicator). For a static VAD indicator, the default setting is correct.</para>
    /// </remarks>
    [DefaultValue(StateChange)]
    public enum ParticipantPropertyUpdateFrequency
    {
        /// <summary>
        /// On participant state change (the default setting)
        /// </summary>
        StateChange = 100,
        /// <summary>
        /// Never
        /// </summary>
        Update00Hz = 0,
        /// <summary>
        /// 1 time per second
        /// </summary>
        Update01Hz = 50,
        /// <summary>
        /// 5 times per second
        /// </summary>
        Update05Hz = 10,
        /// <summary>
        /// 10 times per second
        /// </summary>
        Update10Hz = 5
    }

    /// <summary>
    /// The type of channel.
    /// </summary>
    public enum ChannelType
    {
        /// <summary>
        /// A typical conferencing channel.
        /// </summary>
        NonPositional,
        /// <summary>
        /// A conferencing channel where user voices are rendered with 3D positional effects.
        /// </summary>
        Positional,
        /// <summary>
        /// A conferencing channel where the user's text and audio is echoed back to the user.
        /// </summary>
        Echo
    }

    /// <summary>
    /// Define the policy of where microphone audio and injected audio get broadcast to.
    /// </summary>
    public enum TransmissionMode
    {
        /// <summary>
        /// Adopts a policy of transmission into no channels.
        /// </summary>
        None,
        /// <summary>
        /// Adopts a policy of transmission into one channel at a time.
        /// </summary>
        Single,
        /// <summary>
        /// Adopts a policy of transmission into all channels simultaneously.
        /// </summary>
        All
    }

    /// <summary>
    /// The state of any resource with connection semantics (media and text state).
    /// </summary>
    public enum ConnectionState
    {
        /// <summary>
        /// The resource is disconnected.
        /// </summary>
        Disconnected,
        /// <summary>
        /// The resource is in the process of connecting.
        /// </summary>
        Connecting,
        /// <summary>
        /// The resource is connected.
        /// </summary>
        Connected,
        /// <summary>
        /// The resource is in the process of disconnecting.
        /// </summary>
        Disconnecting
    }

    /// <summary>
    /// The state of Vivox when an unexpected network disconnection occurs. Use this to display the status of the disconnect to the user, or if there is a recovery failure, to reset the game state or retry the overall Vivox connection.
    /// </summary>
    public enum ConnectionRecoveryState
    {
        /// <summary>
        /// Vivox has yet to connect to the service for the first time.
        /// This is the default state.
        /// </summary>
        Disconnected = vx_connection_state.connection_state_disconnected,
        /// <summary>
        /// Vivox has successfully connected to the service for the first time.
        /// </summary>
        Connected = vx_connection_state.connection_state_connected,
        /// <summary>
        /// Vivox is in the process of recovering the connection with the service.
        /// </summary>
        Recovering = vx_connection_state.connection_state_recovering,
        /// <summary>
        /// Vivox has failed to reestablish the connection with the service.
        /// </summary>
        FailedToRecover = vx_connection_state.connection_state_failed_to_recover,
        /// <summary>
        /// Vivox has successfully recovered the connection with the service.
        /// </summary>
        Recovered = vx_connection_state.connection_state_recovered
    }

    /// <summary>
    /// The distance model for a positional channel, which determines the algorithm to use when computing attenuation.
    /// </summary>
    public enum AudioFadeModel
    {
        /// <summary>
        /// No distance-based attenuation is applied. All speakers are rendered as if they are in the same position as the listener.
        /// </summary>
        None = 0,
        /// <summary>
        /// Fades voice quickly at first, buts slows down as you get further from conversational distance.
        /// </summary>
        InverseByDistance = 1,
        /// <summary>
        /// Fades voice slowly at first, but speeds up as you get further from conversational distance.
        /// </summary>
        LinearByDistance = 2,
        /// <summary>
        /// Voice within conversational distance is louder, but fades quickly beyond it.
        /// </summary>
        ExponentialByDistance = 3
    }

    /// <summary>
    /// Configure a set of supported audio codecs.
    /// </summary>
    [DefaultValue(Opus40)]
    public enum MediaCodecType
    {
        /// <summary>
        /// PCMU
        /// </summary>
        PCMU = vx_codec.vx_codec_pcmu,
        /// <summary>
        /// Siren7, 16kHz, 32kbps
        /// </summary>
        Siren7 = vx_codec.vx_codec_siren7,
        /// <summary>
        /// Siren14, 32kHz, 32kbps
        /// </summary>
        Siren14 = vx_codec.vx_codec_siren14,
        /// <summary>
        /// Opus, 48kHz, 8kbps
        /// </summary>
        Opus8 = vx_codec.vx_codec_opus8,
        /// <summary>
        /// Opus, 48kHz, 40kbps -- recommended Opus default
        /// </summary>
        Opus40 = vx_codec.vx_codec_opus40,
        /// <summary>
        /// Opus, 48kHz, 57kbps -- proposed; pending research
        /// </summary>
        Opus57 = vx_codec.vx_codec_opus57,
        /// <summary>
        /// Opus, 48kHz, 72kbps -- proposed; pending research
        /// </summary>
        Opus72 = vx_codec.vx_codec_opus72
    }

    /// <summary>
    /// A unified selection of output streams and mechanisms for text-to-speech (TTS) injection.
    /// </summary>
    public enum TTSDestination
    {
        /// <summary>
        /// Immediately send to participants in connected sessions (according to the transmission policy). Mixes new messages with any other ongoing messages.
        /// </summary>
        RemoteTransmission = vx_tts_destination.tts_dest_remote_transmission,
        /// <summary>
        /// Immediately play back locally on a render device (for example, speaker hardware). Mixes new messages with any other ongoing messages.
        /// </summary>
        LocalPlayback = vx_tts_destination.tts_dest_local_playback,
        /// <summary>
        /// Immediately play back locally on a render device and send to participants in connected sessions (according to the transmission policy). Mixes new messages with any other ongoing messages.
        /// </summary>
        RemoteTransmissionWithLocalPlayback = vx_tts_destination.tts_dest_remote_transmission_with_local_playback,
        /// <summary>
        /// Send to participants in connected sessions, or enqueue if there is already an ongoing message playing in this destination.
        /// </summary>
        QueuedRemoteTransmission = vx_tts_destination.tts_dest_queued_remote_transmission,
        /// <summary>
        /// Play back locally on a render device (for example, speaker hardware), or enqueue if there is already an ongoing message playing in this destination.
        /// </summary>
        QueuedLocalPlayback = vx_tts_destination.tts_dest_queued_local_playback,
        /// <summary>
        /// Play back locally on a render device and send to participants in connected sessions. Enqueue if there is already an ongoing message playing in this destination.
        /// </summary>
        QueuedRemoteTransmissionWithLocalPlayback = vx_tts_destination.tts_dest_queued_remote_transmission_with_local_playback,
        /// <summary>
        /// Immediately play back locally on a render device (for example, speaker hardware). Replaces the currently playing message in this destination.
        /// </summary>
        ScreenReader = vx_tts_destination.tts_dest_screen_reader
    }

    /// <summary>
    /// The state of the text-to-speech (TTS) message.
    /// </summary>
    public enum TTSMessageState
    {
        /// <summary>
        /// The message is not yet in the TTS subsystem.
        /// </summary>
        NotEnqueued,
        /// <summary>
        /// The message is waiting to be played in its destination.
        /// </summary>
        Enqueued,
        /// <summary>
        /// The message is currently being played in its destination.
        /// </summary>
        Playing
    }
}
