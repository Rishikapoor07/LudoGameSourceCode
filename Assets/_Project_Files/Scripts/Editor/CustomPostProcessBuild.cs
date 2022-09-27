using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

public class CustomPostProcessBuild
{
    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget buildTarget, string buildPath)
    {
#if (UNITY_IPHONE || UNITY_IOS) && UNITY_EDITOR_OSX && (UNITY_5 || UNITY_5_3_OR_NEWER)
		if (buildTarget != BuildTarget.iOS) return;
        
        string projPath = PBXProject.GetPBXProjectPath(buildPath);
        PBXProject proj = new PBXProject();
 
        proj.ReadFromString(File.ReadAllText(projPath));

#if UNITY_2019_3_OR_NEWER
        var unityFrameworkTarget = proj.GetUnityFrameworkTargetGuid();
        var unityIphoneTarget = proj.GetUnityMainTargetGuid();
#else
        var target = proj.TargetGuidByName(PBXProject.GetUnityTargetName());
#endif

        proj.AddFrameworkToProject(unityFrameworkTarget, "AVFAudio.framework", false);
        proj.AddFrameworkToProject(unityFrameworkTarget, "WebKit.framework", false);
        proj.AddFrameworkToProject(unityFrameworkTarget, "AuthenticationServices.framework", true);

     	proj.AddBuildProperty(unityIphoneTarget, "OTHER_LDFLAGS", "-weak_framework PhotosUI "); 
     	proj.AddBuildProperty(unityIphoneTarget, "OTHER_LDFLAGS", "-framework Photos"); 
     	proj.AddBuildProperty(unityIphoneTarget, "OTHER_LDFLAGS", "-framework MobileCoreServices"); 
     	proj.AddBuildProperty(unityIphoneTarget, "OTHER_LDFLAGS", "-framework ImageIO");

     	File.WriteAllText(projPath, proj.WriteToString());
		string plistPath = Path.Combine(buildPath, "Info.plist");


		PlistDocument plist = new PlistDocument();
		plist.ReadFromString(File.ReadAllText(plistPath));

		PlistElementDict rootDict = plist.root;
			rootDict.SetBoolean("GADIsAdManagerApp", true);
			rootDict.SetBoolean("ITSAppUsesNonExemptEncryption", false);

		File.WriteAllText(plistPath, plist.WriteToString());
#endif
	}
}
