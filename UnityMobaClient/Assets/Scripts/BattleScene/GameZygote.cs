using Moba.Global;
using UnityEngine;

namespace Moba.Script
{
    public class GameZygote : UnityEngine.MonoBehaviour
    {
        public GameObject[] characters;
        public Transform playerEntry;

        private void Start()
        {
            //实例化角色
            var player = Instantiate(characters[NetInfo.usex]);
            player.transform.position = playerEntry.position;
            
            //添加角色控制
            var ctrl = player.AddComponent<CharacterCtrl>();
            ctrl.isGhost = false;
        }
    }
}