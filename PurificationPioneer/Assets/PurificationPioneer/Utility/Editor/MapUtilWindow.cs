using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace PurificationPioneer.Utility.Editor
{
    public class MapUtilWindow : EditorWindow
    {
        [MenuItem("净化先锋/MapUtil")]
        private static void ShowWindow()
        {
            var window = GetWindow<MapUtilWindow>();
            window.titleContent = new GUIContent("地图制作工具");
            window.Show();
        }
        
        private enum BuildType
        {
            Cube,
            Plane
        }
        
        private string[] titles = {"Create", "Settings"};
        private int selectIndex;
        

        //settings
        private float errorNumber = 0.01f;
        private GameObject myCube;
        private GameObject myPlane;
        private float minTileSize=0.2f;
        
        //build
        private BuildType _buildType;
        private string objName="DefaultName";
        private Vector3 createPos;
        
        private void OnEnable()
        {
            isBuildTypeFresh = true;
        }

        private void OnGUI()
        {
            selectIndex = GUILayout.Toolbar(selectIndex, titles);
            switch (selectIndex)
            {
                case 0://Create
                    objName = EditorGUILayout.TextField("创建物体名", objName);
                    createPos = EditorGUILayout.Vector3Field("创建位置", createPos);
                    
                    EditorGUI.BeginChangeCheck();
                    _buildType = (BuildType)EditorGUILayout.EnumPopup(_buildType);
                    if (EditorGUI.EndChangeCheck())
                    {
                        isBuildTypeFresh = true;
                    }

                    if (BuildMethodDic.TryGetValue(_buildType, out var onGui))
                    {
                        onGui();
                    }
                    else
                    {
                        Debug.LogError($"Unexpected BuildType:{_buildType}");
                        return;
                    }
                    
                    break;
                
                case 1://Settings
                    errorNumber = Mathf.Abs(EditorGUILayout.FloatField("两个float差距小于这个数视为相等", errorNumber));
                    myCube = EditorGUILayout.ObjectField("MyCube", myCube, typeof(GameObject), false) as GameObject;
                    myPlane=EditorGUILayout.ObjectField("MyPlane",myPlane,typeof(GameObject),false) as GameObject;
                    minTileSize = EditorGUILayout.FloatField("最小单元缩放", minTileSize);                
                    break;
            }
        }

        #region BuildTypeGUI

        private Dictionary<BuildType, Action> _buildMethodDic;
        private Dictionary<BuildType, Action> BuildMethodDic
        {
            get
            {
                if (null == _buildMethodDic)
                {
                    _buildMethodDic = new Dictionary<BuildType, Action>();
                    _buildMethodDic.Add(BuildType.Cube,OnCreateCubeGUI);
                    _buildMethodDic.Add(BuildType.Plane,OnCreatePlaneGUI);
                }

                return _buildMethodDic;
            }
        }
        
        //common
        private float tempDivider, xC, yC, zC;
        private bool isBuildTypeFresh = false;
        //cube
        private Vector3 expectedSizeV3;
        private void OnCreateCubeGUI()
        {
            EditorGUI.BeginChangeCheck();
            expectedSizeV3 = EditorGUILayout.Vector3Field("期望大小", expectedSizeV3);
            if (isBuildTypeFresh || EditorGUI.EndChangeCheck())
            {
                isBuildTypeFresh = false;
                var expectedSize = expectedSizeV3;
                tempDivider =
                    GreatestCommonDivisor(expectedSize.x, GreatestCommonDivisor(expectedSize.y, expectedSize.z));
                if (tempDivider > minTileSize)
                {
                    xC = Mathf.RoundToInt(expectedSize.x / tempDivider);
                    yC = Mathf.RoundToInt(expectedSize.y / tempDivider);
                    zC = Mathf.RoundToInt(expectedSize.z / tempDivider);
                }
            }

            if (tempDivider > minTileSize)
            {
                EditorGUILayout.HelpBox($"[Divider-{tempDivider}] 阵列[ {xC}:{yC}:{zC} ]", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox($"[Divider-{tempDivider}] 期望大小最大公因数不合理", MessageType.Error);
            }

            if (GUILayout.Button("创建Cube"))
            {
                if (expectedSizeV3.x < errorNumber || expectedSizeV3.y < errorNumber ||
                    expectedSizeV3.z < errorNumber)
                {
                    Debug.LogError("期望大小非法");
                    return;
                }

                if (!CreateCube(expectedSizeV3, createPos, objName))
                {
                    return;
                }
            }
        }
        //plane
        private Vector2 expectedSizeV2;
        private void OnCreatePlaneGUI()
        {
            EditorGUI.BeginChangeCheck();
            expectedSizeV2 = EditorGUILayout.Vector2Field("期望大小", expectedSizeV2);
            if (isBuildTypeFresh || EditorGUI.EndChangeCheck())
            {
                isBuildTypeFresh = false;
                var expectedSize = expectedSizeV2;
                tempDivider = GreatestCommonDivisor(expectedSize.x, expectedSize.y);

                if (tempDivider > minTileSize)
                {
                    xC = Mathf.RoundToInt(expectedSize.x / tempDivider);
                    yC = Mathf.RoundToInt(expectedSize.y / tempDivider);
                }
            }

            if (tempDivider > minTileSize)
            {
                EditorGUILayout.HelpBox($"[Divider-{tempDivider}] 阵列[ {xC}:{yC} ]",MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox($"[Divider-{tempDivider}] 期望大小最大公因数不合理",MessageType.Error);
            }
                            
            if (GUILayout.Button("创建Plane"))
            {
                if (expectedSizeV2.x < errorNumber || expectedSizeV2.y < errorNumber)
                {
                    Debug.LogError("期望大小非法");
                    return;
                }
                if (!CreatePlane(expectedSizeV2, createPos, objName))
                {
                    return;
                }
            }
        }
        

        #endregion


        /// <summary>
        /// 创建平面
        /// </summary>
        /// <param name="expectedSize"></param>
        /// <param name="position"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool CreatePlane(Vector2 expectedSize, Vector3? position = null, string name = null)
        {
            if (!myPlane)
            {
                Debug.LogError($"MyPlane预制体为空");
                return false;
            }
            
            var objName = string.IsNullOrEmpty(name) ? "NewPlane" : name;
            var centerPos = position ?? Vector3.zero;

            var divider = GreatestCommonDivisor(expectedSize.x, expectedSize.y);        
            if (divider < minTileSize)
            {
                Debug.LogError($"期望大小的最大公因数是{divider}, 小于最小缩放：{minTileSize}");
                return false;
            }
            var xCount = Mathf.RoundToInt(expectedSize.x / divider);
            var yCount = Mathf.RoundToInt(expectedSize.y / divider);

            var newObj = new GameObject(objName)
            {
                isStatic = myPlane.isStatic, 
                layer = myPlane.layer
            };
            newObj.transform.position = centerPos;

            CreateObjGrid(myPlane, xCount, yCount, divider,
                new Vector3(centerPos.x - expectedSize.x / 2, centerPos.y - expectedSize.y / 2, 0),
                Quaternion.identity,
                (rowIndex, colIndex) => new Vector3(colIndex * divider, rowIndex * divider, 0),
                "Body",
                newObj.transform);
            
            MarkDirty(newObj);
            Selection.activeInstanceID = newObj.GetInstanceID();
            return true;
        }

        /// <summary>
        /// 创建长方体
        /// </summary>
        /// <param name="expectedSize"></param>
        /// <param name="position"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool CreateCube(Vector3 expectedSize,Vector3? position=null, string name=null)
        {
            if (!myPlane)
            {
                Debug.LogError($"MyPlane预制体为空");
                return false;
            }
            
            var objName = string.IsNullOrEmpty(name) ? "NewCube" : name;
            var centerPos = position ?? Vector3.zero;
            
            var divider = GreatestCommonDivisor(expectedSize.x, GreatestCommonDivisor(expectedSize.y, expectedSize.z));

            if (divider < minTileSize)
            {
                Debug.LogError($"期望大小的最大公因数是{divider}, 小于最小缩放：{minTileSize}");
                return false;
            }
            
            var xCount = Mathf.RoundToInt(expectedSize.x / divider);
            var yCount = Mathf.RoundToInt(expectedSize.y / divider);
            var zCount = Mathf.RoundToInt(expectedSize.z / divider);
            
            var newObj = new GameObject(objName);
            newObj.isStatic = myPlane.isStatic;
            newObj.layer = myPlane.layer;

            #region Body

            //forward
            CreateObjGrid(myPlane, xCount, yCount, divider,
                new Vector3(centerPos.x - expectedSize.x / 2f, centerPos.y - expectedSize.y / 2f, centerPos.z + expectedSize.z / 2f),
                Quaternion.Euler(0, 180, 0),
                (rowIndex, colIndex) => new Vector3(colIndex * divider, rowIndex * divider, 0),
                "Forward",
                newObj.transform);
            //back
            CreateObjGrid(myPlane, xCount, yCount, divider,
                new Vector3(centerPos.x - expectedSize.x / 2f, centerPos.y - expectedSize.y / 2f, centerPos.z - expectedSize.z / 2f),
                Quaternion.Euler(0, 0, 0),
                (rowIndex, colIndex) => new Vector3(colIndex * divider, rowIndex * divider, 0),
                "Back",
                newObj.transform);
            //left
            CreateObjGrid(myPlane, zCount, yCount, divider,
                new Vector3(centerPos.x - expectedSize.x / 2f, centerPos.y - expectedSize.y / 2f, centerPos.z + expectedSize.z / 2f),
                Quaternion.Euler(0, 90, 0),
                (rowIndex, colIndex) => new Vector3(0, rowIndex * divider,-colIndex * divider),
                "Left",
                newObj.transform);
            //right
            CreateObjGrid(myPlane, zCount, yCount, divider,
                new Vector3(centerPos.x + expectedSize.x / 2f, centerPos.y - expectedSize.y / 2f, centerPos.z + expectedSize.z / 2f),
                Quaternion.Euler(0, -90, 0),
                (rowIndex, colIndex) => new Vector3(0, rowIndex * divider,-colIndex * divider),
                "Right",
                newObj.transform);
            //top
            CreateObjGrid(myPlane, xCount, zCount, divider,
                new Vector3(centerPos.x - expectedSize.x / 2f, centerPos.y + expectedSize.y / 2f, centerPos.z - expectedSize.z / 2f),
                Quaternion.Euler(90, 0, 0),
                (rowIndex, colIndex) => new Vector3(colIndex * divider,0, rowIndex * divider),
                "Top",
                newObj.transform);
            //bottom
            CreateObjGrid(myPlane, xCount, zCount, divider,
                new Vector3(centerPos.x - expectedSize.x / 2f, centerPos.y - expectedSize.y / 2f, centerPos.z - expectedSize.z / 2f),
                Quaternion.Euler(-90, 0, 0),
                (rowIndex, colIndex) => new Vector3(colIndex * divider,0, rowIndex * divider),
                "Bottom",
                newObj.transform);            

            #endregion

            newObj.transform.position = centerPos;
            MarkDirty(newObj);
            Selection.activeInstanceID = newObj.GetInstanceID();
            return true;
        }

        /// <summary>
        /// 创建物体阵列
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="colCount"></param>
        /// <param name="rowCount"></param>
        /// <param name="localScale"></param>
        /// <param name="startPos"></param>
        /// <param name="rotation"></param>
        /// <param name="calculateOffset"></param>
        /// <param name="nameStart"></param>
        /// <param name="parent"></param>
        private void CreateObjGrid(GameObject prefab, int colCount, int rowCount, float localScale, Vector3 startPos, Quaternion rotation, Func<int,int,Vector3> calculateOffset, string nameStart, Transform parent = null)
        {
            //修正自身中心位置偏移
            startPos += calculateOffset(1, 1)/2;
            
            for (var i = 0; i < rowCount; i++)
            {
                for (var j = 0; j < colCount; j++)
                {
                    var pos = startPos + calculateOffset(i, j);
                    var cube = Instantiate(prefab, pos, rotation);
                    cube.transform.localScale = Vector3.one * localScale;
                    cube.transform.SetParent(parent);
                    cube.name = $"[{nameStart}]{prefab.name}[{i},{j}]";
                }
            }
        }


        /// <summary>
        /// 求最大公因数
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private float GreatestCommonDivisor(float a, float b)
        {
            if (a <= errorNumber || b <= errorNumber)
            {
                return 0;
            }
            var max = Mathf.Max(a, b);
            var min = Mathf.Min(a, b);
            while (max - min > errorNumber)
            {
                var ans = max - min;
                max = Mathf.Max(min, ans);
                min = Mathf.Min(min, ans);
            }

            return min;
        }
        private int GreatestCommonDivisor(int a, int b)
        {
            var max = Mathf.Max(a, b);
            var min = Mathf.Min(a, b);
            while (max != min)
            {
                var ans = max - min;
                max = Mathf.Max(min, ans);
                min = Mathf.Min(min, ans);
            }

            return min;
        }


        /// <summary>
        /// 标记修改
        /// </summary>
        /// <param name="obj"></param>
        private void MarkDirty(GameObject obj)
        {
            EditorUtility.SetDirty(obj);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
    }
}