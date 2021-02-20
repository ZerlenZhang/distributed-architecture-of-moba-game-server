using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace PurificationPioneer.Script
{
    [CustomEditor(typeof(MyCube))]
    public class MyCubeEditor : Editor
    {
        private MyCube _cubemc;
        private SerializedProperty _isTriggerProp;
        private SerializedProperty _cubeMaterialProp;
        private SerializedProperty _cubePhysicsMaterialProp;

        private void OnEnable()
        {
            _cubemc=target as MyCube;
            _isTriggerProp = serializedObject.FindProperty("_isTrigger");
            _cubeMaterialProp = serializedObject.FindProperty("_cubeMaterial");
            _cubePhysicsMaterialProp = serializedObject.FindProperty("_cubePhysicsMaterial");
            CheckCube();
        }

        private void CheckCube()
        {
            var parentTrans = _cubemc.transform;
            var changed = false;
            if (!_cubemc.Forward)
            {
                changed = true;
                
                //create forward
                var obj = GameObject.CreatePrimitive(PrimitiveType.Quad);
                obj.name = $"{_cubemc.name}_forward";
                var trans = obj.transform;
                trans.SetParent(parentTrans);
                trans.localPosition = Vector3.forward * (parentTrans.localScale.z / 2);
                trans.rotation = Quaternion.Euler(180, 0, 0);
                trans.localScale = Vector3.one;

                _cubemc.Forward = obj;
            }

            if (!_cubemc.Back)
            {
                changed = true;
                
                //create back
                var obj = GameObject.CreatePrimitive(PrimitiveType.Quad);
                obj.name = $"{_cubemc.name}_back";
                var trans = obj.transform;
                trans.SetParent(parentTrans);
                trans.localPosition = Vector3.back * (parentTrans.localScale.z / 2);
                trans.rotation = Quaternion.identity;
                trans.localScale = Vector3.one;

                _cubemc.Back = obj;
            }

            if (!_cubemc.Top)
            {
                changed = true;
                
                //create top
                var obj = GameObject.CreatePrimitive(PrimitiveType.Quad);
                obj.name = $"{_cubemc.name}_top";
                var trans = obj.transform;
                trans.SetParent(parentTrans);
                trans.localPosition = Vector3.up * (parentTrans.localScale.y / 2);
                trans.rotation = Quaternion.Euler(90, 0, 0);;
                trans.localScale = Vector3.one;

                _cubemc.Top = obj;
            }
            
            if (!_cubemc.Bottom)
            {
                changed = true;
                
                //create bottom
                var obj = GameObject.CreatePrimitive(PrimitiveType.Quad);
                obj.name = $"{_cubemc.name}_bottom";
                var trans = obj.transform;
                trans.SetParent(parentTrans);
                trans.localPosition = Vector3.down * (parentTrans.localScale.y / 2);
                trans.rotation = Quaternion.Euler(-90, 0, 0);;
                trans.localScale = Vector3.one;

                _cubemc.Bottom = obj;
            }
            
            if (!_cubemc.Left)
            {
                changed = true;
                
                //create left
                var obj = GameObject.CreatePrimitive(PrimitiveType.Quad);
                obj.name = $"{_cubemc.name}_left";
                var trans = obj.transform;
                trans.SetParent(parentTrans);
                trans.localPosition = Vector3.left * (parentTrans.localScale.x / 2);
                trans.rotation = Quaternion.Euler(0, 90, 0);;
                trans.localScale = Vector3.one;

                _cubemc.Left = obj;
            }

            if (!_cubemc.Right)
            {
                changed = true;
                
                //create right
                var obj = GameObject.CreatePrimitive(PrimitiveType.Quad);
                obj.name = $"{_cubemc.name}_right";
                var trans = obj.transform;
                trans.SetParent(parentTrans);
                trans.localPosition = Vector3.right * (parentTrans.localScale.x / 2);
                trans.rotation = Quaternion.Euler(0, -90, 0);;
                trans.localScale = Vector3.one;

                _cubemc.Right = obj;
            }

            if (UpdateProperties())
            {
                changed = true;
            }
            
            if (changed && !Application.isPlaying)
            {
                changed = false;
                EditorUtility.SetDirty(_cubemc.gameObject);
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }
        }

        private bool UpdateProperties()
        {
            var changed = false;
            var layer = LayerMask.NameToLayer("Paintable");
            foreach (var child in _cubemc.faces)
            {
                if (SetProperties(child))
                {
                    changed = true;
                }
            }
            

            if (_cubemc.gameObject.layer != layer)
            {
                changed = true;
                _cubemc.gameObject.layer = layer;
            }

            return changed;
        }
        
        private bool SetProperties(GameObject child)
        {
            var changed = false;
            var layer = LayerMask.NameToLayer("Paintable");
            var meshRenderer = child.GetComponent<MeshRenderer>();
            Assert.IsTrue(meshRenderer);
            var meshCollider = child.GetComponent<MeshCollider>();
            Assert.IsTrue(meshCollider);
            if (child.layer != layer)
            {
                changed = true;
                child.layer = layer;
            }

            if (_cubeMaterialProp.objectReferenceValue && 
                meshRenderer.sharedMaterial != _cubeMaterialProp.objectReferenceValue)
            {
                changed = true;
                meshRenderer.sharedMaterial=_cubeMaterialProp.objectReferenceValue as Material;
            }

            if (_cubePhysicsMaterialProp.objectReferenceValue &&
                meshCollider.sharedMaterial!=_cubePhysicsMaterialProp.objectReferenceValue)
            {
                changed = true;
                meshCollider.sharedMaterial=_cubePhysicsMaterialProp.objectReferenceValue as PhysicMaterial;
            }

            if (_isTriggerProp.boolValue != meshCollider.convex
                || _isTriggerProp.boolValue != meshCollider.isTrigger)
            {
                changed = true;
                if (_isTriggerProp.boolValue)
                {
                    meshCollider.convex = _isTriggerProp.boolValue;
                    meshCollider.isTrigger = _isTriggerProp.boolValue;
                }
                else
                {
                    meshCollider.isTrigger = _isTriggerProp.boolValue;
                    meshCollider.convex = _isTriggerProp.boolValue;
                }
            }

            return changed;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUI.BeginChangeCheck();
            
            EditorGUILayout.PropertyField(_isTriggerProp);
            EditorGUILayout.PropertyField(_cubeMaterialProp);
            EditorGUILayout.PropertyField(_cubePhysicsMaterialProp);

            if (EditorGUI.EndChangeCheck())
            {
                if (UpdateProperties())
                {
                    EditorUtility.SetDirty(_cubemc.gameObject);
                    EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                }
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}