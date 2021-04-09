using System;
using System.Collections;
using System.Collections.Generic;
using DialogSystem.Model;
using DialogSystem.ScriptObject;
using ReadyGamerOne.Common;
using ReadyGamerOne.Script;
using UnityEngine;

namespace DialogSystem.Scripts
{
    /// <summary>
    /// 对话系统
    /// </summary>
    public class DialogSystem : MonoBehaviour
    {  
        #region Static

        #region Const_Messages

        /// <summary>
        /// 用于DialogSystem内部
        /// <DialogUnitInfo>资源</DialogUnitInfo>
        /// </summary>
        public static readonly string ExternWord = "__ew";
        
        /// <summary>
        /// 用于DialogSystem内部
        /// <string>assetName</string>
        /// </summary>
        public static readonly string EndThisDialogUnit = "__etdu";

        /// <summary>
        /// 用于DialogSystem内部
        /// </summary>
        public static readonly string CanGoNext = "__cgn";

        /// <summary>
        /// 开始对话
        /// <string>对话单元名字</string>
        /// </summary>
        public static readonly string BeginDialog = "__bdg";

        /// <summary>
        /// Choose单元的分支不会返回
        /// <string>AssetName</string>
        /// </summary>
        public static readonly string ChooseNotBack = "__cnb";

        #region 监听多资源消息

        private static Dictionary<string, List<string>> messageCache = new Dictionary<string, List<string>>();
        private static void AddAssetMessage(string message,string assetName)
        {
            if (!messageCache.ContainsKey(message))
            {
                messageCache.Add(message,new List<string>());
            }

            if (!messageCache[message].Contains(assetName))
                messageCache[message].Add(assetName);
        }
        public static bool GetAssetMessage(string message, string assetName,bool reset=true)
        {
            if (!messageCache.ContainsKey(message))
            {
                return false;
            }

            if (!messageCache[message].Contains(assetName))
                return false;

            if(reset)
                messageCache[message].Remove(assetName);
            return true;
        }        

        #endregion
        

        #endregion

        #region 维护Character名字和GameObject的映射
        
        // 所有DialogCharacter需要在全局窗口中添加好，并且，每个角色都需要挂上DialogCharacter脚本用于维护和更新其位置信息
        // 用于在角色头顶产生对话气泡  
        private static Dictionary<string, GameObject> nameToGameObject = new Dictionary<string, GameObject>();
        /// <summary>
        /// 根据角色名字获取角色GameObject
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static GameObject GetCharacterObj(string name)
        {
            if (nameToGameObject.ContainsKey(name) == false)
                throw new Exception("DialogSystem 不包含这个名字：name:" + name);
            return nameToGameObject[name];
        }
        /// <summary>
        /// 更新角色的位置
        /// </summary>
        /// <param name="name"></param>
        /// <param name="obj"></param>
        public static void RefreshCharacterObj(string name, GameObject obj)
        {
            if (nameToGameObject.ContainsKey(name))
            {
                nameToGameObject[name] = obj;
            }
            else
                nameToGameObject.Add(name, obj);
        }
        
        #endregion
        
        #region 开启对话的API

        /// <summary>
        /// 开始谈话实践
        /// </summary>
        public static event Action onStartDialog;

        /// <summary>
        /// 结束谈话事件
        /// </summary>
        public static event Action onEndDialog;


        /// <summary>
        /// 运行DialogInfoAsset资源
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static IEnumerator RunDialog(AbstractDialogInfoAsset info,Action endCall=null)
        {
            #region 对话开始事件
            
            
            if(!IsRunningAll)
                onStartDialog?.Invoke();
            if (DialogSettings.Instance.ShowDialogStackInfo)
            {
                var text = "对话开始-";
                for (var i = 0; i < stackSize; i++)
                    text += "-";
                print(text+info.name);                
            }


            #endregion
            



            stackSize++;
            
            var DialogUnits = info.DialogUnits;

            var ifConditions=new Stack<bool>();
            var ifTrue = false;

            var currentIndex = 0;

            if (!info.canMoveOnTalking)
                info.SetPlayerMovable(false);
            
            
            
            while (currentIndex<DialogUnits.Count)
            {

                var dialogUnitInfo = DialogUnits[currentIndex];
                
//                Debug.Log(dialogUnitInfo.UnitType);
//                Debug.Log(info.name+":"+ dialogUnitInfo.id);
                
                switch (dialogUnitInfo.UnitType)
                {
                    case UnitType.Word:
//                        Debug.Log("DialogSystem_RunDialog_Switch_UnitType.Word_Start");
                        yield return dialogUnitInfo.RunWordUnit();
//                        Debug.Log("DialogSystem_RunDialog_Switch_UnitType.Word_end");
                        break;
                    case UnitType.Choose:
                        yield return dialogUnitInfo.RunChooseUnit();
                        
                        if (GetAssetMessage(ChooseNotBack,info.name))
                        {            
                            endCall?.Invoke();
                            EndRunningDialog("End by ChooseNotBack",info.name);
                            yield break;
                        }
                        break;
                    
                    case UnitType.Narrator:
                        yield return dialogUnitInfo.RunNarratorUnit();
                        break;

                    case UnitType.ExWord:
                        yield return dialogUnitInfo.RunExWordUnit();
                        break;
                    

                    case UnitType.If:

                        ifTrue = dialogUnitInfo.Complare();
                        ifConditions.Push(ifTrue);


                        if (!ifTrue)//如果不满足IF条件，IF到Else或者EndIF之间不执行
                        {
                            //寻找Else
                            var id = FindUnitFromIndex(info, currentIndex, UnitType.Else);
                            if (id != -1)
                            {
                                //找到Else
                                currentIndex = id - 1;
                            }
                            else//没找到Else，查找EndIF
                            {
                                id = FindUnitFromIndex(info, currentIndex, UnitType.EndIf);

                                if (-1 == id)
                                {            
                                    endCall?.Invoke();
                                    EndRunningDialog("End by 在IF处， if 条件不满足，同时没有EndIf",info.name);
                                    yield break;
                                }
                                else
                                {
                                    //找到EndIf
                                    currentIndex = id - 1;
                                }
                            }
                        }
                        
                        break;
                    case UnitType.Else:

                        if (ifConditions.Peek())//如果满足条件，Else到EndIF不执行
                        {
                            //寻找下一个EndIF
                            var id = FindUnitFromIndex(info, currentIndex, UnitType.EndIf);
                            if (-1 == id)
                            {
                                //没找到
                                endCall?.Invoke();
                                EndRunningDialog("End by 在Else处， if 条件满足，同时没有EndIf",info.name);
                                yield break;
                            }
                            else
                            {
                                //找到了
                                currentIndex = id - 1;
                            }
                        }
                        
                        break;
                    case UnitType.EndIf:
                        ifConditions.Pop();
                        break;
                    case UnitType.Message:
                        dialogUnitInfo.RunMessageUnit();
                        break;
                    case UnitType.SetVar:
                        dialogUnitInfo.RunSetVarUnit();
                        break;
                    case UnitType.End:
                        endCall?.Invoke();
                        EndRunningDialog("End By UnitType.End",info.name);
                        yield break;
                    case UnitType.Skip:
                        currentIndex += dialogUnitInfo.skipNum;
                        break;
                    case UnitType.Event:
                        dialogUnitInfo.RunEventUnit();
                        break;
                    case UnitType.Print:
                        dialogUnitInfo.RunPrintUnit();
                        break;
                    case UnitType.VarInfo:
                        dialogUnitInfo.ShowVarInfo();
                        break;
                    case UnitType.Wait:
                        yield return dialogUnitInfo.WaitForTime();
                        break;
                    case UnitType.Jump:
                        yield return dialogUnitInfo.RunJumpUnit();
                        if (GetAssetMessage(ChooseNotBack,info.name))
                        {            
                            endCall?.Invoke();
                            EndRunningDialog("End by JumpNotBack",info.name);
                            yield break;
                        }
                        
                        break;
                    case UnitType.Shining:
                        yield return dialogUnitInfo.Shining();
                        break;
                    case UnitType.FadeIn:
                        yield return dialogUnitInfo.FadeIn();
                        break;
                    case UnitType.FadeOut:
                        yield return dialogUnitInfo.FadeOut();
                        break;
                    case UnitType.Panel:
//                        Debug.Log("DialogSystem_RunDialog_Switch_UnitType_Panel");
                        dialogUnitInfo.RunPanelUnit();
                        break;
                    case UnitType.Scene:
                        yield return dialogUnitInfo.RunSceneUnit();
//                        Debug.Log("DialogSystem_RunDialog_Switch_UnitType.Scene_end");
                        break;
                    case UnitType.Progress:
                        dialogUnitInfo.RunProgressUnit();
                        break;
                    case UnitType.ProcessInfo:
                        dialogUnitInfo.RunProgressInfoUnit();
                        break;
                    case UnitType.Camera:
                        yield return dialogUnitInfo.RunCameraUnit();
                        break;
                }

                currentIndex++;
            }

            if (!info.canMoveOnTalking)
                info.SetPlayerMovable(true);
            endCall?.Invoke();
            EndRunningDialog("End by normalEnd",info.name);

        }

        /// <summary>
        /// 顺次运行DialogSystem所有DialogInfoAsset资源
        /// </summary>
        /// <param name="dialogSystem"></param>
        /// <returns></returns>
        public static IEnumerator RunAllDialog(DialogSystem dialogSystem,bool await=false)
        {
            IsRunningAll = true;
            onStartDialog?.Invoke();
            
            foreach (var asset in dialogSystem.DialogInfoAssets)
            {
                if(asset==null)
                    continue;
                if (await)
                    yield return RunDialogAwait(asset);
                else
                    yield return RunDialog(asset);
            }
            
            onEndDialog?.Invoke();

            IsRunningAll = false;
        }        
        
        /// <summary>
        /// 运行对话资源，会等待所有对话结束才会开始
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        public static IEnumerator RunDialogAwait(AbstractDialogInfoAsset info, Action callBack = null)
        {
            while (stackSize != 0)
                yield return null;
            yield return RunDialog(info, callBack);
        }
        
        #endregion
        
        #region Private

        private static int stackSize = 0;         
        private static bool IsRunningAll = false;    
        private static int FindUnitFromIndex(AbstractDialogInfoAsset asset, int index, UnitType type)
         {
             bool skip = false || (type == UnitType.Else || type == UnitType.EndIf);
 
             int ifCount = 0;
             
             var DialogUnits = asset.DialogUnits;
             
             for (var i = index+1; i < DialogUnits.Count; i++)
             {
                 if (skip && DialogUnits[i].UnitType == UnitType.If)
                 {
 //                    print("执行ifCount++");
                     ifCount++;
                 }
                 
                 if (DialogUnits[i].UnitType == type)
                 {
                     if (skip)
                     {
                         if (ifCount <= 0)
                             return i;
                         else
                         {
                             ifCount--;
                         }
                     }
                     else
                         return i;
                 }
             }
 
             return -1;
         }
        
        static DialogSystem()
         {
             //监听选择语句不再返回的消息
             CEventCenter.AddListener<string>(ChooseNotBack,(assetName)=>AddAssetMessage(ChooseNotBack,assetName));
         }    
        /// <summary>
        /// 结束对话调用此函数
        /// </summary>
        /// <param name="endStatement"></param>
        /// <param name="assetName"></param>
        private static void EndRunningDialog(string endStatement, string assetName)
        {
            if(!IsRunningAll)
                onEndDialog?.Invoke();
            
            stackSize--;

            if (DialogSettings.Instance.ShowDialogStackInfo)
            {
                var text = "对话结束-";
                for (var i = 0; i < stackSize; i++)
                    text += "-";
                print($"{text}{assetName}【{endStatement}】");                
            }

        }
        
        #endregion      
        
        #endregion
    
        #region MonoBehavior

        private void OnEnable()
        {
            CEventCenter.AddListener<string>(BeginDialog, OnStartDialogOnMessage);
        }

        private void OnDisable()
        {
            CEventCenter.RemoveListener<string>(BeginDialog,OnStartDialogOnMessage);
        }

        protected virtual void Update()
        {
            if(DialogInfoAssets.Count>0 && DialogInfoAssets[0].CanGoToNext())
                CEventCenter.BroadMessage(Scripts.DialogSystem.CanGoNext);
        }

        #endregion

        #region Public
        
        public List<AbstractDialogInfoAsset> DialogInfoAssets=new List<AbstractDialogInfoAsset>();

        public Func<bool> IfInteract => DialogInfoAssets[0].IfInteract;


        /// <summary>
        /// 根据名字开始某个对话
        /// </summary>
        /// <param name="name">对话资源名，这个名字需要是列表中的一个</param>
        /// <param name="endCall">对话结束时的回调</param>
        /// <param name="await">是否等待正在进行的对话结束</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Coroutine StartDialog(string name,Action endCall=null,bool await=false)
        {
            foreach (var asset in DialogInfoAssets)
            {
                if (asset && asset.name == name)
                    if (await)
                        return MainLoop.Instance.StartCoroutine(RunDialogAwait(asset, endCall));
                    else
                        return MainLoop.Instance.StartCoroutine(RunDialog(asset,endCall));
            }

            throw  new Exception("对话资源名字不对:" + name);
        }

        /// <summary>
        /// 根据Index开启某个对话
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Coroutine StartDialog(int index,Action endCall=null,bool await=false)
        {
            if (index >= DialogInfoAssets.Count)
                throw new Exception("StartDialog超出索引范围：index:" + index);
            if (await)
                return MainLoop.Instance.StartCoroutine(RunDialogAwait(DialogInfoAssets[index], endCall));
            else
                return MainLoop.Instance.StartCoroutine(RunDialog(DialogInfoAssets[index],endCall));
        }

        /// <summary>
        /// 顺次开启所有对话
        /// </summary>
        /// <returns></returns>
        public Coroutine StartDialog(bool await=false)
        {
            return MainLoop.Instance.StartCoroutine(RunAllDialog(this,await));
        }

        /// <summary>
        /// 获取列表所有对话资源名字
        /// </summary>
        /// <returns></returns>
        public List<string> GetAssetNames()
        {
            var names = new List<string>();
            foreach (var asset in DialogInfoAssets)
            {
                //print(asset.name);
                if (asset == null)
                    continue;
                names.Add(asset.name);
            }
            if (names.Count == 0)
                print("没有对话资源");
            return names;
        }
                
        #endregion
        
        #region Native

        private void OnStartDialogOnMessage(string assetName)
        {
            if (GetAssetNames().Contains(name))
                StartDialog(name);
        }
        

        #endregion
        
    }
}