using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VivoxUnity
{
    class LoginOptions
    {
        /// <summary>
        /// The DisplayName used by the LoginSession.
        /// </summary>
        string displayName;

        /// <summary>
        /// Whether or not to automatically switch transmission to newly connected channels. If true and transmission mode is set to single, connecting to a channel will switch the currently transmitting channel to the new channel.
        /// </summary>
        bool alwaysTransmitToNewChannels;

        /// <summary>
        /// Whether or not to enable Text-to-Speech for this account - disabling it will prevent Text-to-Speech messages from being sent or received.
        /// </summary>
        bool enableTTS;

        /// <summary>
        /// A list of Account identifiers to be blocked immediately upon login.
        /// </summary>
        List<string> blockedUserList;
    }
}
