using DialogSystem.Model;

namespace DialogSystem.Scripts
{
    /// <summary>
    /// 角色脚本
    /// 通过这个脚本维护角色名字和GameObject之间的映射
    /// </summary>
    public class DialogCharacter : UnityEngine.MonoBehaviour
    {
        public CharacterChooser CharacterChooser;

        private void OnEnable()
        {
            DialogSystem.RefreshCharacterObj(CharacterChooser.CharacterName, transform.gameObject);
        }

    }
}