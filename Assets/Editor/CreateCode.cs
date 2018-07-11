using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System;
using UnityEngine.UI;
public enum EWriteState
{
    None,
    WRITE
}
public class CreateCode {
    private static List<FileInfo> fileInfo = new List<FileInfo>();
    private static bool isFileExist = false;
    private static List<Transform> childs = new List<Transform>();
	[MenuItem("Tools/自动生成UI代码")]
    public static void AutoCreateCode()
    {
        FileStream myFs;
        fileInfo.Clear();
        GameObject[] selects = Selection.gameObjects;
        string filePath = Application.dataPath+"/../../Haier/Assets/Scripts/";
        GetAllFile(filePath);
        foreach(GameObject go in selects)
        {
            StreamWriter writer;
            string path = filePath;
            isFileExist = false;
            string fileContent;
            foreach (FileInfo nextFile in fileInfo)
            {
                if (nextFile.Name.Equals(go.name+".cs"))
                {
                    isFileExist = true;
                    path = nextFile.DirectoryName+"/"+nextFile.Name;
                    
                    break;
                }
            }
            if (!isFileExist)
            {
                myFs = new FileStream(path + go.name+".cs", FileMode.Create);
                fileContent = CreateContent(go,myFs);
                writer = new StreamWriter(myFs);
            }
            else
            {
                myFs = new FileStream(path, FileMode.Open,FileAccess.ReadWrite);
                fileContent = AppendContent(go,myFs,path);
                writer = new StreamWriter(path, false);
            }
            writer.Write(fileContent);
            writer.Close();
            myFs.Close();
        }
        Debug.Log("生成代码成功");
    }
    
    //public static void BuildAssetBundles()
    //{
    //    UnityEngine.Object[] objs = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Deep);
    //    string url;
    //    for (int i = 0; i < objs.Length; i++)
    //    {
    //        url = AssetDatabase.GetAssetPath(objs[i]);
    //        AssetImporter import = AssetImporter.GetAtPath(url);
    //        import.assetBundleName = objs[i].name + ".assetbundle";
    //    }
    //}

    private static void GetAllFile(string path)
    {
        DirectoryInfo dInfo = new DirectoryInfo(path);
        foreach(FileInfo fInfo in dInfo.GetFiles())
        {
            fileInfo.Add(fInfo);
        }
        foreach(DirectoryInfo dir in dInfo.GetDirectories())
        {
            GetAllFile(dir.FullName);
        }
    }

    private static string CreateContent(GameObject go,FileStream myFs)
    {
        childs.Clear();
        GetChildComponts(go);
        StringBuilder content = new StringBuilder();
        
        content.Append("using UnityEngine;\n");
        content.Append("using UnityEngine.UI;\n\n");
        content.Append("public class " + go.name + ": MonoBehaviour{\n\n");
        content.Append("\t#region " + go.name+"\n");
        
        foreach(Transform t in childs)
        {
            Component[] components = t.GetComponents<Component>();
            content.Append("\t" + "public " + getType(components).Name + " " + t.name + ";\n");
        }
        content.Append("\t#endregion\n");
        content.Append("}");
        return content.ToString();
    }

    private static string AppendContent(GameObject go,FileStream stream,string path)
    {
        stream.Seek(0,SeekOrigin.Begin);
        StringBuilder content = new StringBuilder();
        EWriteState state = EWriteState.None;
        childs.Clear();
        GetChildComponts(go);
        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
        string line;
        while((line = reader.ReadLine()) != null)
        {
            if (line.Equals("\t#region "+go.name))
            {
                state = EWriteState.WRITE;
                content.Append("\t#region " + go.name+"\n");
                foreach (Transform t in childs)
                {
                    Component[] components = t.GetComponents<Component>();
                    content.Append("\t" + "public " + getType(components).Name + " " + t.name + ";\n");
                }
            }
            else if (line.Contains("endregion"))
            {
                state = EWriteState.None;
            }
            if(state == EWriteState.None)
            {
                content.Append(line+"\n");
            }
        }
        reader.Close();
        return content.ToString();
    }

    private static void GetChildComponts(GameObject go)
    {
        for (int j = 0; j < go.transform.childCount; j++)
        {
            Transform temp = go.transform.GetChild(j);
            if (temp.name.StartsWith("b_"))
            {
                childs.Add(temp);
            }
            if (temp.childCount > 0)
            {
                GetChildComponts(temp.gameObject);
            }
        }
    }

    private static Type getType(Component[] components)
    {
        int x = int.MaxValue;
        Type c = null;
        foreach (Component com in components)
        {
            if (com is InputField)
            {
                if (x > 0)
                {
                    x = 0;
                    c = typeof(InputField);
                }
            }
            else if (com is ScrollRect)
            {
                if (x > 1)
                {
                    x = 1;
                    c = typeof(ScrollRect);
                }
            }
            else if (com is Scrollbar)
            {
                if (x > 2)
                {
                    x = 2;
                    c = typeof(Scrollbar);
                }
            }
            else if (com is Slider)
            {
                if (x > 3)
                {
                    x = 3;
                    c = typeof(Slider);
                }
            }
            else if (com is Button)
            {
                if (x > 4)
                {
                    x = 4;
                    c = typeof(Button);
                }
            }
            else if (com is Toggle)
            {
                if (x > 5)
                {
                    x = 5;
                    c = typeof(Toggle);
                }
            }
            else if (com is RawImage)
            {
                if (x > 6)
                {
                    x = 6;
                    c = typeof(RawImage);
                }
            }
            else if (com is Image)
            {
                if (x > 7)
                {
                    x = 7;
                    c = typeof(Image);
                }
            }
            else if (com is Text)
            {
                if (x > 8)
                {
                    x = 8;
                    c = typeof(Text);
                }
            }
        }
        if (c == null)
            return typeof(Transform);
        return c;
    }
}
