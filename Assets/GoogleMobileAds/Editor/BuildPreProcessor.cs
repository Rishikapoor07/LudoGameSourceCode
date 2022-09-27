#if UNITY_EDITOR
using System;
#if UNITY_2018_1_OR_NEWER
using UnityEditor.Build.Reporting;
#endif

#if UNITY_EDITOR
using UnityEditor.Callbacks;
using UnityEditor;
using UnityEditor.Build;

using GoogleMobileAds.Editor;
#endif

#if UNITY_2018_1_OR_NEWER
public class BuildPreProcessor : IPreprocessBuildWithReport
#else
public class BuildPreProcessor : IPreprocessBuild
#endif
{

    public int callbackOrder { get { return 1; } }

#if UNITY_2018_1_OR_NEWER
    public void OnPreprocessBuild(BuildReport report)
#else
    public void OnPreprocessBuild(BuildTarget target, string path)
#endif
    {
#if UNITY_EDITOR
        if (!AssetDatabase.IsValidFolder("Assets/GoogleMobileAds"))
        {
            AssetDatabase.CreateFolder("Assets", "GoogleMobileAds");
        }

        if (AssetDatabase.IsValidFolder("Packages/com.google.ads.mobile"))
        {
            AssetDatabase.CopyAsset("Packages/com.google.ads.mobile/GoogleMobileAds/link.xml", "Assets/GoogleMobileAds/link.xml");
        }
#endif
    }
}

#endif