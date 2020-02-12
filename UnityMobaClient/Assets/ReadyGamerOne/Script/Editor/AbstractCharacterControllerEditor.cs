using UnityEditor;
using UnityEngine;

namespace ReadyGamerOne.Script.Editor
{
    [CustomEditor(typeof(AbstractCharacterController))]
    public class AbstractCharacterControllerEditor:UnityEditor.Editor
    {
        private SerializedProperty moveTypeProp;
        private SerializedProperty worldTypeProp;
        private SerializedProperty inputTypeProp;
        private SerializedProperty dirTypeProp;
        private SerializedProperty spaceTypeProp;

        private SerializedProperty hAxisProp;
        private SerializedProperty vAxisProp;
        private SerializedProperty lKeyProp;
        private SerializedProperty rKeyProp;
        private SerializedProperty uKeyProp;
        private SerializedProperty dKeyProp;
        private bool foldKeys = false;

        private SerializedProperty autoFitProp;

        private SerializedProperty idleAniNameProp;
        private SerializedProperty idleAniIndexProp;
        

        private SerializedProperty enableWalkProp;
        private SerializedProperty walkScalerProp;
        private SerializedProperty walkAniNameProp;
        private SerializedProperty walkAniIndexProp;
        private bool foldWalk=false;
        
        private SerializedProperty enableRunProp;
        private SerializedProperty runScalerProp;
        private SerializedProperty runAniNameProp;
        private SerializedProperty runAniIndexProp;
        private bool foldRun = false;
        
        private SerializedProperty enableDashProp;
        private SerializedProperty dashAniNameProp;
        private SerializedProperty dashAniIndexProp;
        private bool foldDash = false;
        private SerializedProperty dashKeyProp;
        private SerializedProperty dashDistanceProp;
        private SerializedProperty dashArgProp;
        private SerializedProperty lerpTypeProp;

        private SerializedProperty enableSquatProp;
        private SerializedProperty squatKeyProp;
        private SerializedProperty squatAniNameProp;
        private SerializedProperty squatAniIndexProp;
        private bool foldSquat = false;
        
        

        private AbstractCharacterController mb;
        private string[] aniNames;

        private Animator ani;
        private string[] GetAniNames()
        {
            if (aniNames == null)
            {
                var clips = ani.runtimeAnimatorController.animationClips;
                aniNames = new string[clips.Length];
                var index = 0;
                foreach (var clip in clips)
                {
                    aniNames[index++] = clip.name;
                }
            }

            return aniNames;
        }
        private void OnEnable()
        {
            //this.behavior = target as BasicCharacterController;
            mb=target as AbstractCharacterController;

            ani = mb.GetComponent<Animator>();

            this.moveTypeProp = serializedObject.FindProperty("moveType");
            this.worldTypeProp = serializedObject.FindProperty("worldType");
            this.inputTypeProp = serializedObject.FindProperty("inputType");
            this.dirTypeProp = serializedObject.FindProperty("dirType");
            this.spaceTypeProp = serializedObject.FindProperty("spaceType");

            this.hAxisProp = serializedObject.FindProperty("horizontalAxis");
            this.vAxisProp = serializedObject.FindProperty("verticalAxis");
            this.lKeyProp = serializedObject.FindProperty("leftKey");
            this.rKeyProp = serializedObject.FindProperty("rightKey");
            this.uKeyProp = serializedObject.FindProperty("upKey");
            this.dKeyProp = serializedObject.FindProperty("downKey");

            this.autoFitProp = serializedObject.FindProperty("FitDirByLocalScale");

            this.idleAniNameProp = serializedObject.FindProperty("idleAniName");
            this.idleAniIndexProp = serializedObject.FindProperty("idleAniIndex");

            this.enableWalkProp = serializedObject.FindProperty("enableWalk");
            this.walkScalerProp = serializedObject.FindProperty("walkScaler");
            this.walkAniNameProp = serializedObject.FindProperty("walkAniName");
            this.walkAniIndexProp = serializedObject.FindProperty("walkAniIndex");
            
            this.enableRunProp = serializedObject.FindProperty("enableRun");
            this.runScalerProp = serializedObject.FindProperty("runScaler");
            this.runAniNameProp = serializedObject.FindProperty("runAniName");
            this.runAniIndexProp = serializedObject.FindProperty("runAniIndex");
            
            this.enableDashProp = serializedObject.FindProperty("enableDash");
            this.dashAniNameProp = serializedObject.FindProperty("dashAniName");
            this.dashAniIndexProp = serializedObject.FindProperty("dashAniIndex");
            this.dashDistanceProp = serializedObject.FindProperty("dashDistance");
            this.dashKeyProp = serializedObject.FindProperty("dashKey");
            this.dashArgProp = serializedObject.FindProperty("dashTime");
            this.lerpTypeProp = serializedObject.FindProperty("lerpType");

            this.enableSquatProp = serializedObject.FindProperty("enableSquat");
            this.squatKeyProp = serializedObject.FindProperty("squatKey");
            this.squatAniNameProp = serializedObject.FindProperty("squatAniName");
            this.squatAniIndexProp = serializedObject.FindProperty("squatAniIndex");
        }
        
        

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(moveTypeProp);

            if (moveTypeProp.enumValueIndex == 0)//Physical
            {
                EditorGUILayout.PropertyField(worldTypeProp);
            }
            else// Transform
            {
                EditorGUILayout.PropertyField(spaceTypeProp);
            }
            
            EditorGUILayout.PropertyField(dirTypeProp);
            
            EditorGUILayout.PropertyField(inputTypeProp);

            if (foldKeys = EditorGUILayout.Foldout(foldKeys, "键位"))
            {
                switch (inputTypeProp.enumValueIndex)
                {
                    case 0://Input_GetAxis
                        switch (dirTypeProp.enumValueIndex)
                        {
                            case 0:// X
                                EditorGUILayout.PropertyField(hAxisProp);
                                break;
                            case 1:// Y
                                EditorGUILayout.PropertyField(vAxisProp);
                                break;
                            case 2:
                            case 3:
                                EditorGUILayout.PropertyField(hAxisProp);
                                EditorGUILayout.PropertyField(vAxisProp);
                                break;
                        }
                        break;
                    case 1://Input_GetKey
                        switch (dirTypeProp.enumValueIndex)
                        {
                            case 0:// X
                                EditorGUILayout.PropertyField(lKeyProp);
                                EditorGUILayout.PropertyField(rKeyProp);
                                break;
                            case 1:// Y
                                EditorGUILayout.PropertyField(uKeyProp);
                                EditorGUILayout.PropertyField(dKeyProp);
                                break;
                            case 2:
                            case 3:
                                EditorGUILayout.PropertyField(lKeyProp);
                                EditorGUILayout.PropertyField(rKeyProp);
                                EditorGUILayout.PropertyField(uKeyProp);
                                EditorGUILayout.PropertyField(dKeyProp);
                                break;
                        }
                        break;
                }
            }

            if (mb.EnableAni)
            {
                EditorGUILayout.PropertyField(autoFitProp);
                
                idleAniIndexProp.intValue = EditorGUILayout.Popup("Idle动画", idleAniIndexProp.intValue, GetAniNames());
                idleAniNameProp.stringValue = GetAniNames()[idleAniIndexProp.intValue];
            }

            #region Walk

            EditorGUILayout.PropertyField(enableWalkProp);
            if (enableWalkProp.boolValue)
            {
                if (foldWalk = EditorGUILayout.Foldout(foldWalk, "走路状态详情"))
                {    
                    switch (moveTypeProp.enumValueIndex)
                    {
                        case 0:
                            break;
                        case 1://Transform
                            EditorGUILayout.PropertyField(walkScalerProp);
                            break;
                    }
        
                    if (mb.EnableAni)
                    {
                        walkAniIndexProp.intValue= EditorGUILayout.Popup("走路动画", walkAniIndexProp.intValue, GetAniNames());
                        walkAniNameProp.stringValue = GetAniNames()[walkAniIndexProp.intValue];                
                    }
                }
            }            

            #endregion


            #region Run

            EditorGUILayout.PropertyField(enableRunProp);
            if (enableRunProp.boolValue)
            {
                if (foldRun = EditorGUILayout.Foldout(foldRun, "跑步状态详情"))
                {
                    switch (moveTypeProp.enumValueIndex)
                    {
                        case 0:
                            break;
                        case 1://Transform
                            EditorGUILayout.PropertyField(runScalerProp);
                            break;
                    }

                    if (mb.EnableAni)
                    {
                        runAniIndexProp.intValue= EditorGUILayout.Popup("跑步动画",runAniIndexProp.intValue, GetAniNames());
                        runAniNameProp.stringValue = GetAniNames()[runAniIndexProp.intValue];                    
                    }                    
                }


            }            

            #endregion


            #region Dash

            EditorGUILayout.PropertyField(enableDashProp);
            if (enableDashProp.boolValue)
            {
                if (foldDash = EditorGUILayout.Foldout(foldDash, "冲刺状态详情"))
                {
                    EditorGUILayout.PropertyField(dashKeyProp);
                    lerpTypeProp.enumValueIndex =
                        EditorGUILayout.Popup("插值类型", lerpTypeProp.enumValueIndex, new[] {"线性插值", "球形插值"});
                    
                    switch (moveTypeProp.enumValueIndex)
                    {
                        case 1://Transform
                            EditorGUILayout.PropertyField(dashDistanceProp);
                            dashArgProp.floatValue = EditorGUILayout.Slider("DashTime", dashArgProp.floatValue, 0, 3f);
                            break;
                    }
    
                    if (mb.EnableAni)
                    {
                        dashAniIndexProp.intValue= EditorGUILayout.Popup("冲刺动画", dashAniIndexProp.intValue, GetAniNames());
                        dashAniNameProp.stringValue = GetAniNames()[dashAniIndexProp.intValue];                
                    }                
                }
            }            

            #endregion

            #region Squat


            EditorGUILayout.PropertyField(enableSquatProp);
            if (enableSquatProp.boolValue)
            {
                if (foldSquat = EditorGUILayout.Foldout(foldSquat, "下蹲状态详情"))
                {
                    EditorGUILayout.PropertyField(squatKeyProp);
                    if (mb.EnableAni)
                    {
                        squatAniIndexProp.intValue =
                            EditorGUILayout.Popup("下蹲动画", squatAniIndexProp.intValue, GetAniNames());
                        squatAniNameProp.stringValue = GetAniNames()[squatAniIndexProp.intValue];
                    }
                }
            }
            

            #endregion



            serializedObject.ApplyModifiedProperties();

        }
    }
}