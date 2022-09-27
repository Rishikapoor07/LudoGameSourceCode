namespace VivoxUnity
{
    public interface IAccessTokenServer
    {
        string Issuer { get; }

        string Key { get; }

        System.TimeSpan ExpirationTimeSpan { get; }
    }
}
