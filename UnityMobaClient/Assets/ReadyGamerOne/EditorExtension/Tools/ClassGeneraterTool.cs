using System.Collections.Generic;
using System.IO;
using ReadyGamerOne.Global;
#if UNITY_EDITOR
using UnityEditor;
using Directory = UnityEngine.Windows.Directory;
using FileUtil = ReadyGamerOne.Utility.FileUtil;
#endif
using UnityEngine;

namespace ReadyGamerOne.EditorExtension
{
    
//#pragma warning disable CS0414

    public class ClassGeneraterTool
#if UNITY_EDITOR
        : IEditorTools
#endif
    {
#if UNITY_EDITOR

        static string Title = "自动类生成";

        private static string generateDir;
        private static string sourDir;
        private static string nameSpace;
        private static bool HasParentClass = true;
        private static string parentClass;
        private static bool IsGenericParent = false;
        private static bool IsGenericParamSelf = true;
        private static string GenericParam;

        static void OnToolsGUI(string rootNs, string viewNs, string constNs, string dataNs, string autoDir,
            string scriptDir)
        {
            EditorGUILayout.LabelField("生成目录", generateDir);
            if (GUILayout.Button("设置生成目录"))
                generateDir = EditorUtility.OpenFolderPanel("选择生成目录", Application.dataPath, "");
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("资源目录", sourDir);
            if (GUILayout.Button("设置资源目录"))
                sourDir = EditorUtility.OpenFolderPanel("选择资源目录", Application.dataPath, "");
            EditorGUILayout.Space();
            
            nameSpace = EditorGUILayout.TextField("命名空间", nameSpace);

            HasParentClass = EditorGUILayout.Toggle("是否具有父类", HasParentClass);
            if (HasParentClass)
            {
                parentClass = EditorGUILayout.TextField("父类名", parentClass);
                IsGenericParent = EditorGUILayout.Toggle("父类是否是泛型类", IsGenericParent);
                if (IsGenericParent)
                {
                    IsGenericParamSelf = EditorGUILayout.Toggle("泛型参数是不是本类名", IsGenericParamSelf);
                    if (!IsGenericParamSelf)
                        GenericParam = EditorGUILayout.TextField("泛型参数", GenericParam);
                }
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("开始生成", GUILayout.Height(3 * EditorGUIUtility.singleLineHeight)))
            {
                if (!Directory.Exists(generateDir))
                {
                    Debug.LogError("生成目录不存在：" + generateDir);
                    return;
                }

                if (!Directory.Exists(sourDir))
                {
                    Debug.LogError("资源目录不存在：" + sourDir);
                    return;
                }

                if (string.IsNullOrEmpty(nameSpace))
                {
                    Debug.LogError("命名空间不能为空");
                    return;
                }

                if (HasParentClass)
                {
                    if (string.IsNullOrEmpty(parentClass))
                    {
                        Debug.LogError("父类不能为空");
                        return;
                    }
                    else
                    {
                        if (IsGenericParent && !IsGenericParamSelf && string.IsNullOrEmpty(GenericParam))
                        {
                            Debug.LogError("泛型参数不能为空");
                            return;
                        }
                    }
                }
                
                Create();
                
            }


        }


        private static void Create()
        {
            List<string> fileNames = new List<string>();
            
            FileUtil.SearchDirectory(sourDir, fileInfo =>
            {
                if (fileInfo.FullName.EndsWith(".meta"))
                    return;
                var pureName = Path.GetFileNameWithoutExtension(fileInfo.FullName);
                if (fileNames.Contains(pureName))
                {
                    Debug.LogError("发现同名文件:"+fileNames);
                    return;
                }
                fileNames.Add(pureName);
            },true);

            foreach (var file in fileNames)
            {
                string parent = null;
                if (HasParentClass)
                {
                    parent = parentClass;
                    if (IsGenericParent)
                    {
                        if (IsGenericParamSelf)
                            parent += "<" + file + ">";
                        else
                            parent += "<" + GenericParam + ">";
                    }
                }
                FileUtil.CreateClassFile(file,nameSpace,generateDir,parent,partical:true);
            }

            AssetDatabase.Refresh();

            Debug.Log("生成完成");
        }

#endif
    }
}