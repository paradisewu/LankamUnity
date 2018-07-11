using UnityEngine;
using UnityEditor;
using System.IO;

public class AssetDatabaseDemo : MonoBehaviour
{

    [MenuItem("AssetDatabaseDemo/BuildAssetbundle")]
    public static void BuildAssetBundle()
    {
        string url = Application.dataPath + "/StreamingAssets";
        DirectoryInfo mydir = new DirectoryInfo(url);
        if (!mydir.Exists)
        {
            Directory.CreateDirectory(url);
        }
        BuildPipeline.BuildAssetBundles(url, 0, EditorUserBuildSettings.activeBuildTarget);
        AssetDatabase.Refresh();
    }

    [MenuItem("AssetDatabaseDemo/SetAssetsBundleName")]
    public static void BuildAssetBundles()
    {
        Object[] objs = Selection.GetFiltered(typeof(Object), SelectionMode.Deep);
        string url;
        for (int i = 0; i < objs.Length; i++)
        {
            url = AssetDatabase.GetAssetPath(objs[i]);
            AssetImporter import = AssetImporter.GetAtPath(url);
            import.assetBundleName = objs[i].name + ".assetbundle";
        }
    }
}
