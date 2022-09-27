namespace VivoxUnity
{
    /// <summary>
    /// Either an audio input device (microphone) or audio output device (speaker hardware or headphones).
    /// </summary>
    public interface IAudioDevice : IKeyedItemNotifyPropertyChanged<string>
    {
        /// <summary>
        /// A user-displayable device name.
        /// </summary>
        string Name { get; }
    }
}
