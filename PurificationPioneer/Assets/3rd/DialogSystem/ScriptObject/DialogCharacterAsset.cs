using System.Collections.Generic;
using ReadyGamerOne.Common;


namespace DialogSystem.ScriptObject
{
    [ScriptableSingletonInfo("DialogCharacters")]
    public class DialogCharacterAsset:ScriptableSingleton<DialogCharacterAsset>
    {
        public List<string> characterNames=new List<string>();
    }
}