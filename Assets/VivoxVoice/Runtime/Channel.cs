using VivoxUnity;

namespace Unity.Services.Vivox
{
    public class Channel : ChannelId
    {
        public Channel(string name, ChannelType type = ChannelType.NonPositional, Channel3DProperties properties = null) 
            : base(VivoxService.Instance.Issuer, name, VivoxService.Instance.Domain, type, properties, VivoxService.Instance.EnvironmentId) {}
    }
}