using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace ReadyGamerOne.Utility
{
    public static class GameObjectExtension
    {
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