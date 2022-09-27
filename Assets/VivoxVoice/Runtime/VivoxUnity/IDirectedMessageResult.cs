namespace VivoxUnity
{
    /// <summary>
    /// The result of a directed message (user-to-user).
    /// </summary>
    public interface IDirectedMessageResult
    {
        /// <summary>
        /// The request ID of the directed message.
        /// </summary>
        string RequestId { get; }
    }
}
