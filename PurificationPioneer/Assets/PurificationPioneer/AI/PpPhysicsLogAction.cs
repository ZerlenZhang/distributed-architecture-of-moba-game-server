using System.Text;
using BehaviorDesigner.Runtime.Tasks;
using PurificationPioneer.Script;
using PurificationPioneer.Scriptable;
using UnityEngine;

namespace PurificationPioneer.AI
{
    public class PpPhysicsLogAction : Action
    {
        public bool showPhysicsFrameId = false;
        public string text;

#if DebugMode
        public bool controlByGameSetting = true;
#endif

        public override TaskStatus OnUpdate()
        {
#if DebugMode
            if (controlByGameSetting && GameSettings.Instance.EnableAiLog)
            {
                var msg = new StringBuilder();
                msg.Append("[AI]");
                if (showPhysicsFrameId)
                    msg.Append($"[PpPhysicsId-{PpPhysics.physicsFrameId}]");
                msg.Append($"{text}");
                Debug.Log(msg);                
            }
#endif

            return TaskStatus.Success;
        }
    }
}