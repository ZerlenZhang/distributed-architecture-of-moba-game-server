using System.Collections.Generic;
using DialogSystem.Model;
using ReadyGamerOne.Common;


namespace DialogSystem.ScriptObject
{
    [ScriptableSingletonInfo("DialogVars")]
    public class DialogVarAsset:ScriptableSingleton<DialogVarAsset>
    {
        public List<VarUnitInfo> varInfos = new List<VarUnitInfo>();

        public VarUnitInfo GetVarWithName(string name)
        {
            foreach (var VARIABLE in varInfos)
            {
                if (VARIABLE.VarName == name)
                    return VARIABLE;
            }

            return null;
        }
    }
}