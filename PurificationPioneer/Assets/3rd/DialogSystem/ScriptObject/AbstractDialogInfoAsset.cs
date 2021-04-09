using System;
using System.Collections.Generic;
using DialogSystem.Model;
using ReadyGamerOne.EditorExtension;
using UnityEngine;
#if UNITY_EDITOR
    
using UnityEngine.Windows;
using UnityEditor;
#endif
namespace DialogSystem.ScriptObject
{
    public abstract class AbstractDialogInfoAsset:ScriptableObject
    {
       
        #region DataStructures


        public List<DialogUnitInfo> DialogUnits = new List<DialogUnitInfo>();

        [SerializeField] private DialogVarAsset varAsset;
        [SerializeField] private DialogCharacterAsset characterAsset;

        public DialogVarAsset VarAsset
        {
            get
            {
                if(varAsset==null)
                    varAsset=DialogVarAsset.Instance;
                return varAsset;
            }
        }

        public DialogCharacterAsset CharacterAsset
        {
            get
            {
                if(characterAsset==null)
                    characterAsset=DialogCharacterAsset.Instance;
                return characterAsset;
            }
        }
        
        #endregion

        #region Abstract_Properties

        

        /// <summary>
        /// 如何下一步？比如有的游戏里对话时，”点击“继续对话
        /// </summary>
        public abstract Func<bool> CanGoToNext { get; }

        /// <summary>
        /// 是否进行了交互，比如一些游戏中的”操作“按键，用于和场景物体交互
        /// </summary>
        public virtual Func<bool> IfInteract => null;
        
        /// <summary>
        /// 说话者名字类，Word，Choose单元会用到，说话者的名字枚举值来自于这个类的公有静态字段
        /// </summary>
        public abstract Action<DialogUnitInfo> CreateChooseUi { get; }
    
        public abstract Action<DialogUnitInfo> CreateWordUI { get; }

        public virtual Action<DialogUnitInfo> CreateNarratorUI => null;
        

        #endregion
        
        #region Virtual_Properties

        /// <summary>
        /// 消息类型，Message单元会使用，消息类型来自这个类公有静态字段
        /// </summary>
        public virtual Type MessageType => null;

        /// <summary>
        /// Panel类型，重写此属性，DialogSystem中就会有Panel操作功能
        /// </summary>
        public virtual Type PanelType => null;

        /// <summary>
        /// Scene类型，重写此属性，DialogSystem中就会有Scene操作功能
        /// </summary>
        public virtual Type SceneType => null;
        
        /// <summary>
        /// 控制主角说话时是否可以行动
        /// </summary>
        public virtual Action<bool> SetPlayerMovable => null;

        /// <summary>
        /// Dialog类型Word单元对应的UI预制体名，提供多种路径，就可以在编辑器可视化选择使用哪个预制体
        /// </summary>
       public virtual string[] DialogWordUiKeys=>null;

        public virtual string[] CaptionWordUiKeys => null;

        /// <summary>
        /// Narrator单元对应的UI预制体名
        /// </summary>
        public virtual string NarratorUiKeys => null;
        
        /// <summary>
        /// Choose单元对应的UI预制体名
        /// </summary>
        public virtual string ChooseUiKeys=> null;

        #endregion

        #region PublicFields

        [Header("对话时能否移动")] 
        public bool canMoveOnTalking = true;
        [Header("设置打字机效果几秒之内结束")]
        public float wordFinishTime = 0.3f;
        [Header("是否受Time.timeScale影响")]
        public bool affectedByTimeScale = true;
        [Header("是否刷新StringChooser变量")]
        public bool refreshStringChooser = false;
        

        #endregion

        #region Used_For_Editor
        public int GetIntOffset(int index)
        {
            var intOffset = 0;
            for (int i = 0; i < index; i++)
                switch (DialogUnits[i].UnitType)
                {
                    case UnitType.If:
                        intOffset++;
                        break;
                    case UnitType.EndIf:
                        intOffset--;
                        break;
                }

            if (DialogUnits[index].UnitType == UnitType.Else ||
                DialogUnits[index].UnitType == UnitType.EndIf)
                intOffset--;

            return intOffset;
        }

        public int GetID()
        {
            var index = 0;
            while (true)
            {
                var ok = true;
                foreach (var unit in DialogUnits)
                {
                    if (index == unit.id)
                    {
                        ok = false;
                        break;
                    }
                }

                if (ok)
                    break;
                index++;
            }

            return index;
        }


        public Dictionary<string, string> GetValueStrings(ArgType type)
        {
            var list = new Dictionary<string, string>();
            switch (type)
            {
                case ArgType.Bool:
                    foreach (var VARIABLE in VarAsset.varInfos)
                    {
                        if (string.IsNullOrEmpty(VARIABLE.VarName))
                            continue;
                        if (list.ContainsKey(VARIABLE.VarName))
                            continue;
                        if (VARIABLE.ArgChooser.argType == ArgType.Bool)
                        {
                            list.Add(VARIABLE.VarName, VARIABLE.ArgChooser.BoolArg.ToString());
                        }
                    }

                    break;
                case ArgType.Int:
                    foreach (var VARIABLE in VarAsset.varInfos)
                    {
                        if (string.IsNullOrEmpty(VARIABLE.VarName))
                            continue;
                        if (list.ContainsKey(VARIABLE.VarName))
                            continue;
                        if (VARIABLE.ArgChooser.argType == ArgType.Int)
                        {
                            list.Add(VARIABLE.VarName, VARIABLE.ArgChooser.IntArg.ToString());
                        }
                    }

                    break;
                case ArgType.Float:
                    foreach (var VARIABLE in VarAsset.varInfos)
                    {
                        if (string.IsNullOrEmpty(VARIABLE.VarName))
                            continue;
                        if (list.ContainsKey(VARIABLE.VarName))
                            continue;
                        if (VARIABLE.ArgChooser.argType == ArgType.Float)
                        {
                            list.Add(VARIABLE.VarName, VARIABLE.ArgChooser.FloatArg.ToString());
                        }
                    }

                    break;
                case ArgType.String:
                    foreach (var VARIABLE in VarAsset.varInfos)
                    {
                        if (string.IsNullOrEmpty(VARIABLE.VarName))
                            continue;
                        if (list.ContainsKey(VARIABLE.VarName))
                            continue;
                        if (VARIABLE.ArgChooser.argType == ArgType.String)
                        {
                            list.Add(VARIABLE.VarName, VARIABLE.ArgChooser.StringArg);
                        }
                    }

                    break;
            }

            return list;
        }

        public Dictionary<string, ArgChooser> GetValueInfos()
        {
            var dic = new Dictionary<string, ArgChooser>();
            foreach (var VARIABLE in VarAsset.varInfos)
            {
                if(string.IsNullOrEmpty(VARIABLE.VarName))
                    continue;
                dic.Add(VARIABLE.VarName, VARIABLE.ArgChooser);
            }

            return dic;
        }

        #endregion
    }
}