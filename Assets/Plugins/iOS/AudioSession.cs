using System.Runtime.InteropServices;

public static class AudioSession
{
#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern void _SetAudioAmbient();
    [DllImport("__Internal")]
    private static extern bool _SetAudioPlayback();
    [DllImport("__Internal")]
    private static extern bool _SetAudioPlayAndRecord();
    [DllImport("__Internal")]
    private static extern bool _IsAmbientSet();
 
    public static void SetAudioAmbient()
    {
        _SetAudioAmbient();
    }
 
    public static void SetAudioPlayback()
    {
        _SetAudioPlayback();
    }
 
    public static void SetAudioPlayAndRecord()
    {
        _SetAudioPlayAndRecord();
    }
 
    public static bool IsAmbientSet()
    {
        return _IsAmbientSet();
    }
#endif
}