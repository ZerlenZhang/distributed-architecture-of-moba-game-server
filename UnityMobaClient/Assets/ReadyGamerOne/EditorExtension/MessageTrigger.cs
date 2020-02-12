using System;
using ReadyGamerOne.Common;
using UnityEngine;

namespace ReadyGamerOne.EditorExtension
{
//#pragma warning disable CS0649
    [Serializable]
    public class MessageTrigger
    {
        public bool tickMessage;

        public int argCount;

        [SerializeField] private ArgChooser arg1;
        [SerializeField] private ArgChooser arg2;
        
        public StringChooser messageToTick;

        public void TickMessage()
        {
            switch (argCount)
            {
                case 0:
                    CEventCenter.BroadMessage(messageToTick.StringValue);
                    break;
                case 1:
                    switch (arg1.argType)
                    {
                        case ArgType.Float:
                            CEventCenter.BroadMessage(messageToTick.StringValue,arg1.FloatArg);
                            break;
                        case ArgType.Int:
                            CEventCenter.BroadMessage(messageToTick.StringValue,arg1.IntArg);
                            break;
                        case ArgType.String:
                            CEventCenter.BroadMessage(messageToTick.StringValue,arg1.StringArg);
                            break;
                        case ArgType.Bool:
                            CEventCenter.BroadMessage(messageToTick.StringValue,arg1.BoolArg);
                            break;
                    }
                    break;
                case 2:
                    switch (arg1.argType)
                    {
                        case ArgType.Float:
                            switch (arg2.argType)
                            {
                                case ArgType.Float:
                                    CEventCenter.BroadMessage(messageToTick.StringValue,arg1.FloatArg, arg2.FloatArg);
                                    break;
                                case ArgType.Int:
                                    CEventCenter.BroadMessage(messageToTick.StringValue,arg1.FloatArg,arg2.IntArg);
                                    break;
                                case ArgType.String:
                                    CEventCenter.BroadMessage(messageToTick.StringValue,arg1.FloatArg,arg2.StringArg);
                                    break;
                                case ArgType.Bool:
                                    CEventCenter.BroadMessage(messageToTick.StringValue,arg1.FloatArg,arg2.BoolArg);
                                    break;
                            }
                            break;
                        case ArgType.Int:
                            switch (arg2.argType)
                            {
                                case ArgType.Float:
                                    CEventCenter.BroadMessage(messageToTick.StringValue,arg1.IntArg, arg2.FloatArg);
                                    break;
                                case ArgType.Int:
                                    CEventCenter.BroadMessage(messageToTick.StringValue,arg1.IntArg,arg2.IntArg);
                                    break;
                                case ArgType.String:
                                    CEventCenter.BroadMessage(messageToTick.StringValue,arg1.IntArg,arg2.StringArg);
                                    break;
                                case ArgType.Bool:
                                    CEventCenter.BroadMessage(messageToTick.StringValue,arg1.IntArg,arg2.BoolArg);
                                    break;
                            }
                            break;
                        case ArgType.String:
                            switch (arg2.argType)
                            {
                                case ArgType.Float:
                                    CEventCenter.BroadMessage(messageToTick.StringValue,arg1.StringArg, arg2.FloatArg);
                                    break;
                                case ArgType.Int:
                                    CEventCenter.BroadMessage(messageToTick.StringValue,arg1.StringArg,arg2.IntArg);
                                    break;
                                case ArgType.String:
                                    CEventCenter.BroadMessage(messageToTick.StringValue,arg1.StringArg,arg2.StringArg);
                                    break;
                                case ArgType.Bool:
                                    CEventCenter.BroadMessage(messageToTick.StringValue,arg1.StringArg,arg2.BoolArg);
                                    break;
                            }
                            break;
                        case ArgType.Bool :
                            switch (arg2.argType)
                            {
                                case ArgType.Float:
                                    CEventCenter.BroadMessage(messageToTick.StringValue,arg1.BoolArg, arg2.FloatArg);
                                    break;
                                case ArgType.Int:
                                    CEventCenter.BroadMessage(messageToTick.StringValue,arg1.BoolArg,arg2.IntArg);
                                    break;
                                case ArgType.String:
                                    CEventCenter.BroadMessage(messageToTick.StringValue,arg1.BoolArg,arg2.StringArg);
                                    break;
                                case ArgType.Bool:
                                    CEventCenter.BroadMessage(messageToTick.StringValue,arg1.BoolArg,arg2.BoolArg);
                                    break;
                            }
                            break;
                    }
                    break;
            }
        }
    }

}

