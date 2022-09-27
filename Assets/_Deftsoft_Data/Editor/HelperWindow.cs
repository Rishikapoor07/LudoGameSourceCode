using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.IO;
using System.Text;

namespace Deftsoft
{
    public class HelperWindow : EditorWindow
    {
        #region SCENE_LOADER

        [MenuItem("[DEFTSOFT]/Load Scene/Loading Scene")]
        public static void LoadGameInit()
        {
            string scenePath = Application.dataPath + "/_Project_Files/Scenes/Loading.unity";
            EditorSceneManager.OpenScene(scenePath);
        }

        [MenuItem("[DEFTSOFT]/Load Scene/UI Scene")]
        public static void LoadGameUI()
        {
            string scenePath = Application.dataPath + "/_Project_Files/Scenes/MainUI.unity";
            EditorSceneManager.OpenScene(scenePath);
        }

        [MenuItem("[DEFTSOFT]/Load Scene/Game Scene")]
        public static void LoadGame()
        {
            string scenePath = Application.dataPath + "/_Project_Files/Scenes/Game.unity";
            EditorSceneManager.OpenScene(scenePath);
        }

        [MenuItem("[DEFTSOFT]/Load Scene/Test Scene")]
        public static void LoadTest()
        {
            string scenePath = Application.dataPath + "/_Deftsoft_Data/_TestData/TestScene.unity";
            EditorSceneManager.OpenScene(scenePath);
        }

        #endregion

        #region OTHER_UTILITIES

        [MenuItem("[DEFTSOFT]/Game Library")]
        public static void HighlightGameLibrary()
        {
            GameLibrary gameLibrary = (GameLibrary)Resources.Load("GameLibrary", typeof(GameLibrary));
            Selection.objects = new Object[] { gameLibrary };
            EditorGUIUtility.PingObject(gameLibrary);
        }

        [MenuItem("[DEFTSOFT]/Generate Debugger UI")]
        public static void LoadDebuggerUI()
        {
            GameObject newObj = Instantiate(Resources.Load("DeveloperHelperUI")) as GameObject;
            newObj.name = "DeveloperHelperUI";
            newObj.transform.SetAsLastSibling();
            Undo.RegisterCreatedObjectUndo(newObj, "Create Deftsoft Developer UI");
        }

        [MenuItem("[DEFTSOFT]/Clear PlayerPref")]
        public static void DeleteAllPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
        }

        #endregion

    }

    public static class Deftsoft_Editor_Utility
    {
        public static void WriteIntoFile(string path, string fileNameWithFormat, string data)
        {
            string filePath = Path.Combine(path, fileNameWithFormat);
            if (File.Exists(filePath)) File.Delete(filePath);

            StreamWriter writer = new StreamWriter(filePath, true);
            writer.Write(data);
            writer.Close();

            //Loading the explorer.
            path = path.Replace(@"/", @"\");
            System.Diagnostics.Process.Start("explorer.exe", "/select," + path);
        }
    }
}