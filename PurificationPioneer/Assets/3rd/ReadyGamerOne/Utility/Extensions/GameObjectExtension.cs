using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace ReadyGamerOne.Utility
{
    public static class GameObjectExtension
    {
        public static int GetUnityDetectLayer(this GameObject self)
        {
            var layer=0;
            for (var i = 0; i < 32; i++)
            {
                var ignore = Physics.GetIgnoreLayerCollision(i, self.layer);
                if(!ignore)
                    layer |= 1 << i;
            }

            return layer;
        }
        public static T GetOrAddComponent<T>(this GameObject self)
            where T:Component
        {
            var ans = self.GetComponent<T>();
            if (!ans)
                ans = self.AddComponent<T>();
            return ans;
        }     

        public static Component GetOrAddComponent(this GameObject self, Type componentType)
        {
            var com = self.GetComponent(componentType);
            if (!com)
                com = self.AddComponent(componentType);
            Assert.IsTrue(com);
            return com;
        }
    }
}