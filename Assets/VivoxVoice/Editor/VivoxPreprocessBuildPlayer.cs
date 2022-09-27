#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build;
#if UNITY_2018_1_OR_NEWER

using UnityEditor.Build.Reporting;
#endif
using Unity.Services.Vivox.Editor;
using UnityEngine;

class VivoxPreprocessBuildPlayer :
#if UNITY_2018_1_OR_NEWER
    IPreprocessBuildWithReport
# else
    IPreprocessBuild
#endif
{
    // Directory of the ChatChannelSample sample.
    private string vivoxSampleDirectory = Application.dataPath + "/Vivox/Examples/ChatChannelSample";
    // Path to our audio directory
    private string vivoxAudioDirectory = Application.dataPath + "/Vivox/Examples/ChatChannelSample/Audio";
    // Path to StreamingAssets
    private string vivoxStreamingAssetsPath = Application.dataPath + "/StreamingAssets/VivoxAssets";
    // Our audio file used
    private string vivoxAudioFile = "VivoxAudioForInjection.wav";

    public int callbackOrder { get { return 0; } }

#if UNITY_2018_1_OR_NEWER
    public void OnPreprocessBuild(BuildReport report)
    {
#if UNITY_EDITOR_OSX && UNITY_IOS
        CheckMicDescription();
#endif
        StreamingAssetsSetup();
        CheckBuildTarget();
#if AUTH_PACKAGE_PRESENT
        EditorGameServiceAnalyticsSender.SendVivoxBuildWithAuthenticationEvent();
#endif
    }
#endif

    public void OnPreprocessBuild(BuildTarget target, string path)
    {
#if UNITY_EDITOR_OSX && UNITY_IOS
        CheckMicDescription();
#endif
        StreamingAssetsSetup();
    }

    private void StreamingAssetsSetup()
    {
        if (Directory.Exists(vivoxAudioDirectory))
        {
            if (!Directory.Exists(vivoxStreamingAssetsPath))
            {
                Directory.CreateDirectory(vivoxStreamingAssetsPath);
                File.Copy(vivoxAudioDirectory + "/" + vivoxAudioFile, vivoxStreamingAssetsPath + "/" + vivoxAudioFile);
            }
            else
            {
                if (!File.Exists(vivoxStreamingAssetsPath + "/" + vivoxAudioFile))
                {
                    File.Copy(vivoxAudioDirectory + "/" + vivoxAudioFile, vivoxStreamingAssetsPath + "/" + vivoxAudioFile);
                }
            }
        }
    }
#if UNITY_EDITOR_OSX && UNITY_IOS
    private void CheckMicDescription()
    {
        if (string.IsNullOrEmpty(PlayerSettings.iOS.microphoneUsageDescription))
        {
            Debug.LogWarning("If this application requests Microphone Access you must add a description to the `Other Settings > Microphone Usage Description` in Player Settings");
        }
    }
#endif

    // If build settings are set to an unusable build, then send out a log error.
    static void CheckBuildTarget()
    {
        // Apple silicon is not available as a build target in 2019.4, and MacOSArchitecture does not exist within OSXStandalone, so only run this chunk on Mac editors later than 2020.2.
#if UNITY_STANDALONE_OSX && UNITY_2020_2_OR_NEWER
        var targetArchitecture = UnityEditor.OSXStandalone.UserBuildSettings.architecture;
#if UNITY_STANDALONE_OSX && UNITY_2022_2_OR_NEWER
        if(targetArchitecture == UnityEditor.Build.OSArchitecture.ARM64 || targetArchitecture == UnityEditor.Build.OSArchitecture.x64ARM64)
#elif UNITY_STANDALONE_OSX && UNITY_2020_2_OR_NEWER
        if(targetArchitecture == UnityEditor.OSXStandalone.MacOSArchitecture.ARM64 || targetArchitecture == UnityEditor.OSXStandalone.MacOSArchitecture.x64ARM64)
#endif
        {
            Debug.LogError("[Vivox]: Currently not compatible with Apple silicon. Go to File > Build Settings > Architecture and set your Unity target platform architecture to Intel 64-bit.");
            return;
        }
#else
        return;
#endif
    }
}
#endif