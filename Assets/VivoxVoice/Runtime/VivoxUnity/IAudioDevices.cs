using System;
using System.ComponentModel;

namespace VivoxUnity
{

    /// <summary>
    /// An interface to enumerate and manage audio devices.
    /// </summary>
    public interface IAudioDevices : INotifyPropertyChanged
    {
        /// <summary>
        /// Call BeginSetActiveDevice() with this device to use whatever the operating system uses for the device.
        /// </summary>
        IAudioDevice SystemDevice { get; }
        /// <summary>
        /// The available devices on this system. 
        /// </summary>
        /// <remarks>
        /// Note: Call <see cref="IAudioDevices.BeginRefresh"/> before accessing this property or values could be stale.
        /// </remarks>
        IReadOnlyDictionary<string, IAudioDevice> AvailableDevices { get; }
        /// <summary>
        /// Call this to set the active audio device. This takes effect immediately for all open sessions.
        /// </summary>
        /// <param name="device">The active device.</param>
        /// <param name="callback">Called upon completion.</param>
        /// <returns>An IAsyncResult.</returns>
        IAsyncResult BeginSetActiveDevice(IAudioDevice device, AsyncCallback callback);
        /// <summary>
        /// Call this to pick up failures from the BeginSetActiveDevice() asynchronous method.
        /// </summary>
        /// <param name="result">The value returned from BeginSetActiveDevice().</param>
        void EndSetActiveDevice(IAsyncResult result);
        /// <summary>
        /// The active audio device.
        /// </summary>
        IAudioDevice ActiveDevice { get; }
        /// <summary>
        /// The effective system device. 
        /// </summary>
        /// <remarks>
        /// <para>If the active device is set to SystemDevice or CommunicationDevice, then the effective device shows the actual device used.</para>
        /// <para>Note: When the value for this property changes, a PropertyChanged event fires.</para>
        /// </remarks>
        IAudioDevice EffectiveDevice { get; }

        /// <summary>
        /// AudioGain for the device.
        /// </summary>
        /// <remarks>
        /// This is a value between -50 and 50. Positive values make the audio louder, and negative values make the audio quieter.
        /// 0 leaves the value unchanged (default). This applies to all active audio sessions.
        /// </remarks>
        int VolumeAdjustment { get; set; }
        /// <summary>
        /// Indicate whether audio is muted for this device.
        /// </summary>
        /// <remarks>
        /// Set to true to stop the audio device from capturing or rendering audio. 
        /// The default is false.
        /// </remarks>
        bool Muted { get; set; }
        /// <summary>
        /// The audio energy level of this device, normalized to a value between 0 and 1
        /// </summary>
        /// <remarks>
        /// Useful in creating VU meters for testing out the audio levels before login or while logged in
        /// </remarks>
        double DeviceEnergy { get; }
        /// <summary>
        /// Refresh the list of available devices. 
        /// </summary>
        /// <param name="cb">The function to call when the operation completes.</param>
        /// <returns>An IAsyncResult.</returns>
        /// <remarks>
        /// Call BeginRefresh before accessing the <see cref="IAudioDevices.ActiveDevice"/>, <see cref="IAudioDevices.EffectiveDevice"/>, and <see cref="IAudioDevices.AvailableDevices"/> properties.
        /// Note: It can take up to 200 milliseconds before the list of devices refreshes.
        /// </remarks>
        IAsyncResult BeginRefresh(AsyncCallback cb);
        /// <summary>
        /// Call this to pick up failures from the BeginRefresh() asynchronous method.
        /// </summary>
        /// <param name="result">The result returned from BeginRefresh.</param>
        void EndRefresh(IAsyncResult result);

    }
}
