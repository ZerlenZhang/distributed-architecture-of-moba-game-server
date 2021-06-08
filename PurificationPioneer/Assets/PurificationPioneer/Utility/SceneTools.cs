using ReadyGamerOne.Global;
using ReadyGamerOne.Utility;
using UnityEditor;
using UnityEngine;

namespace PurificationPioneer
{
    public class SceneTools
#if UNITY_EDITOR
        :IEditorTools
#endif
    {
        
        #pragma warning disable 414
#if UNITY_EDITOR
        private static string Title = "场景工具";
        private static GameObject root;
        private static GameObject cube;
        private static float floorDepth = -2;
        private static Vector2Int floorSize;
        private static void OnToolsGUI(string rootNs, string viewNs, string constNs, string dataNs, string autoDir,
            string scriptDir)
        {
            root = EditorGUILayout.ObjectField("Root", root, typeof(GameObject), true)
                as GameObject;
            cube = EditorGUILayout.ObjectField("Cube", cube, typeof(GameObject), true)
                as GameObject;
            floorDepth = EditorGUILayout.Slider("深度", floorDepth, -10, 10);
            floorSize = EditorGUILayout.Vector2IntField("大小", floorSize);

            if (GUILayout.Button("生成地板",
                GUILayout.Height(2 * EditorGUIUtility.singleLineHeight)))
            {
                if (null == root || cube==null)
                {
                    Debug.LogError("Params is error");
                    return;
                }

                var floorRoot = root.transform.Find("floorRoot");
                if (floorRoot == null)
                {
                    var obj = new GameObject("floorRoot");
                    obj.transform.SetParent(root.transform);
                    obj.transform.localPosition=Vector3.zero;
                    obj.transform.localScale = Vector3.one;
                    floorRoot = obj.transform;
                }
                else
                {
                    floorRoot.DestroyChildrenImmediate();
                }

                var cubeSize = cube.transform.lossyScale;
                var startPos=new Vector3(
                    -(cubeSize.x*floorSize.x/2),
                    floorDepth,
                    -(cubeSize.z*floorSize.y/2));
                
                for (var i = 0; i < floorSize.x; i++)
                {
                    for (var j = 0; j < floorSize.y; j++)
                    {
                        var pos = startPos + new Vector3(
                            i * cubeSize.x,
                            0,
                            j * cubeSize.z);
                        var obj = Object.Instantiate(cube, pos, Quaternion.identity, floorRoot);
                        obj.name = $"Floor_{i}_{j}";
                    }
                }
            }
        }        
#endif
        

    }
}