using System;
using ReadyGamerOne.Common;
using ReadyGamerOne.EditorExtension;
using UnityEngine;

namespace DialogSystem.Model
{
    [Serializable]
    public class MessageTrigger
    {
#pragma warning disable 649
        public int argCount;

        [SerializeField] private ValueChooser arg1;
        [SerializeField] private ValueChooser arg2;
        
        public StringChooser messageToTick;

        public void TickMessage()
        {
            switch (argCount)
            {
                case 0:
                    CEventCenter.BroadMessage(messageToTick.StringValue);
                    break;
                case 1:
                    switch (arg1.ArgType)
                    {
                        case ArgType.Float:
                            CEventCenter.BroadMessage(messageToTick.StringValue,arg1.FloatValue);
                            break;
                        case ArgType.Int:
                            CEventCenter.BroadMessage(messageToTick.StringValue,arg1.IntValue);
                            break;
                        case ArgType.String:
                            CEventCenter.BroadMessage(messageToTick.StringValue,arg1.StrValue);
                            break;
                        case ArgType.Bool:
                            CEventCenter.BroadMessage(messageToTick.StringValue,arg1.BoolValue);
                            break;
                        case ArgType.Vector3:
                            CEventCenter.BroadMessage(messageToTick.StringValue, arg1.Vector3Value);
                            break;
                    }
                    break;
                case 2:
                    switch (arg1.ArgType)
                    {
                        case ArgType.Float:
                            switch (arg2.ArgType)
                            {
                                case ArgType.Float:
                                    CEventCenter.BroadMessage(messageToTick.StringValue,arg1.FloatValue, arg2.FloatValue);
                                    break;
                                case ArgType.Int:
                                    CEventCenter.BroadMessage(messageToTick.StringValue,arg1.FloatValue,arg2.IntValue);
                                    break;
                                case ArgType.String:
                                    CEventCenter.BroadMessage(messageToTick.StringValue,arg1.FloatValue,arg2.StrValue);
                                    break;
                                case ArgType.Bool:
                                    CEventCenter.BroadMessage(messageToTick.StringValue,arg1.FloatValue,arg2.BoolValue);
                                    break;
                                
                                case ArgType.Vector3:
                                    CEventCenter.BroadMessage(messageToTick.StringValue, arg1.FloatValue,arg2.Vector3Value);
                                    break;
                            }
                            break;
                        case ArgType.Int:
                            switch (arg2.ArgType)
                            {
                                case ArgType.Float:
                                    CEventCenter.BroadMessage(messageToTick.StringValue,arg1.IntValue, arg2.FloatValue);
                                    break;
                                case ArgType.Int:
                                    CEventCenter.BroadMessage(messageToTick.StringValue,arg1.IntValue,arg2.IntValue);
                                    break;
                                case ArgType.String:
                                    CEventCenter.BroadMessage(messageToTick.StringValue,arg1.IntValue,arg2.StrValue);
                                    break;
                                case ArgType.Bool:
                                    CEventCenter.BroadMessage(messageToTick.StringValue,arg1.IntValue,arg2.BoolValue);
                                    break;
                                case ArgType.Vector3:
                                    CEventCenter.BroadMessage(messageToTick.StringValue, arg1.IntValue,arg2.Vector3Value);
                                    break;
                            }
                            break;
                        case ArgType.String:
                            switch (arg2.ArgType)
                            {
                                case ArgType.Float:
                                    CEventCenter.BroadMessage(messageToTick.StringValue,arg1.StrValue, arg2.FloatValue);
                                    break;
                                case ArgType.Int:
                                    CEventCenter.BroadMessage(messageToTick.StringValue,arg1.StrValue,arg2.IntValue);
                                    break;
                                case ArgType.String:
                                    CEventCenter.BroadMessage(messageToTick.StringValue,arg1.StrValue,arg2.StrValue);
                                    break;
                                case ArgType.Bool:
                                    CEventCenter.BroadMessage(messageToTick.StringValue,arg1.StrValue,arg2.BoolValue);
                                    break;
                                case ArgType.Vector3:
                                    CEventCenter.BroadMessage(messageToTick.StringValue, arg1.StrValue,arg2.Vector3Value);
                                    break;
                            }
                            break;
                        case ArgType.Bool :
                            switch (arg2.ArgType)
                            {
                                case ArgType.Float:
                                    CEventCenter.BroadMessage(messageToTick.StringValue,arg1.BoolValue, arg2.FloatValue);
                                    break;
                                case ArgType.Int:
                                    CEventCenter.BroadMessage(messageToTick.StringValue,arg1.BoolValue,arg2.IntValue);
                                    break;
                                case ArgType.String:
                                    CEventCenter.BroadMessage(messageToTick.StringValue,arg1.BoolValue,arg2.StrValue);
                                    break;
                                case ArgType.Bool:
                                    CEventCenter.BroadMessage(messageToTick.StringValue,arg1.BoolValue,arg2.BoolValue);
                                    break;
                                case ArgType.Vector3:
                                    CEventCenter.BroadMessage(messageToTick.StringValue, arg1.BoolValue,arg2.Vector3Value);
                                    break;
                            }
                            break;
                        case ArgType.Vector3:
                            switch (arg2.ArgType)
                            {
                                case ArgType.Float:
                                    CEventCenter.BroadMessage(messageToTick.StringValue,arg1.Vector3Value, arg2.FloatValue);
                                    break;
                                case ArgType.Int:
                                    CEventCenter.BroadMessage(messageToTick.StringValue,arg1.Vector3Value,arg2.IntValue);
                                    break;
                                case ArgType.String:
                                    CEventCenter.BroadMessage(messageToTick.StringValue,arg1.Vector3Value,arg2.StrValue);
                                    break;
                                case ArgType.Bool:
                                    CEventCenter.BroadMessage(messageToTick.StringValue,arg1.Vector3Value,arg2.BoolValue);
                                    break;
                                
                                case ArgType.Vector3:
                                    CEventCenter.BroadMessage(messageToTick.StringValue, arg1.Vector3Value,arg2.Vector3Value);
                                    break;
                            }
                            break;
                    }
                    break;
            }
        }
#pragma warning restore 649
    }

}

