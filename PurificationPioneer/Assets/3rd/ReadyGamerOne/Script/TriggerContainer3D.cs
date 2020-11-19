using System;
using System.Collections.Generic;
using ReadyGamerOne.Utility;
using UnityEngine;

namespace ReadyGamerOne.Script
{
    public class TriggerContainer3D:MonoBehaviour
    {
        private LayerMask targetLayer=0;
        private List<Collider> contains = new List<Collider>();
        private Func<Collider, bool> IsOk;

        public void Init(LayerMask targetLayerMask, Func<Collider, bool> condition)
        {
            this.targetLayer = targetLayerMask;
            this.IsOk = condition;
        }


        /// <summary>
        /// 外部回去触发器内部东西
        /// </summary>
        public List<Collider> Content => contains;

        private void OnTriggerEnter(Collider other)
        {
            //判断层级
            if (1 == targetLayer.value.GetNumAtBinary(other.gameObject.layer))
            {
                if (!contains.Contains(other))
                {
                    if (IsOk(other))
                    {
                        Debug.Log("进入：" + other.name+other.GetInstanceID());
                        contains.Add(other);
                    }
                }                
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (contains.Contains(other))
            {
                Debug.Log("移除："+other.name);
                contains.Remove(other);
            }
        }
    }
}