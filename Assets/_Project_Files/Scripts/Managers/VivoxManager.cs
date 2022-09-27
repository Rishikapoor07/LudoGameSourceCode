using UnityEngine;
using VivoxUnity;

public class VivoxManager : VivoxVoiceManager
{
	VivoxVoiceManager vivoxVoiceManager;
	Client _client = new Client();
	public ChannelId _joinedChannelId;
	public IChannelSession _joinedChannelSession;

	private void Awake()
	{
		vivoxVoiceManager = VivoxVoiceManager.Instance;
		_client.Uninitialize();
		_client.Initialize();
		vivoxVoiceManager.OnUserLoggedInEvent += OnVivoxLoggedIn;
		vivoxVoiceManager.OnUserLoggedOutEvent += OnVivoxLoggedOut;
		vivoxVoiceManager.OnParticipantAddedEvent += JoinedChannel;
		vivoxVoiceManager.OnParticipantMute += OnMuteParticipant;
	}

	private void OnApplicationQuit()
	{
		LogOutVivox();
	}

	public void LogInToVivox(string userName)
	{
		if (string.IsNullOrEmpty(userName)) return;
		Debug.Log("Login in to the vivox");
		vivoxVoiceManager.Login(userName);
	}

	public void OnVivoxLoggedIn()
	{
		Debug.Log("Vivox logged in successfully with player name: " + vivoxVoiceManager.LoginSession.LoginSessionId.DisplayName);
		GameInfo.vivoxLogIn = true;
		//HelperUtil.HideLoading();
	}

	public void LogOutVivox()
	{
		if (vivoxVoiceManager.LoginState == LoginState.LoggedIn) vivoxVoiceManager.Logout();
	}

	public void OnVivoxLoggedOut()
	{
		GameInfo.vivoxLogIn = false;
	}

	public void JoinVivoxChannel(string channelName)
	{
		if (!GameInfo.vivoxLogIn)
		{
			LogInToVivox(PlayerDataHolder.PlayerName);

			HelperUtil.CallAfterCondition(() =>
			{
				vivoxVoiceManager.JoinChannel(channelName, ChannelType.NonPositional, ChatCapability.AudioOnly);
			}, () => GameInfo.vivoxLogIn);
		}
		else vivoxVoiceManager.JoinChannel(channelName, ChannelType.NonPositional, ChatCapability.AudioOnly);
	}

	void JoinedChannel(string username, ChannelId channel, IParticipant participant)
	{
		Debug.Log(username + " joined the voice channel with channel id: " + channel);
		if (participant.IsSelf)
		{
			_joinedChannelId = channel;
			_joinedChannelSession = LoginSession.GetChannelSession(_joinedChannelId);
			//HelperUtil.LoadScene(SceneName.Game);   ////Loading game scene after joining the channel
		}
	}

	public void MuteOwnMic(bool isMuted)
	{
		_client.AudioInputDevices.Muted = isMuted;
	}

	public void MuteOtherUser(string username)
	{
		var participants = _joinedChannelSession.Participants;

		if (participants[username].InAudio)
		{
			if (participants[username].LocalMute == false)
			{
				participants[username].LocalMute = true;
			}
			else
			{
				participants[username].LocalMute = false;
			}
		}
		else
		{
			//Tell Player To Try Again
			Debug.Log("Try Again");
		}
	}

	public void OnMuteParticipant(string participantName, ChannelId channel, bool isMuted)
	{
	}
}