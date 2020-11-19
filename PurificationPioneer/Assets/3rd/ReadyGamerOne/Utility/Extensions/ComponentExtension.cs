using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace ReadyGamerOne.Utility
{
    public static class ComponentExtension
    {
        public static T GetComponent<T>(this Component self, string transformPath)
            where T : Component
        {
            if (!self)
                return null;
            var temp = self.transform.Find(transformPath);
            if (!temp)
            {
                throw new Exception($"{self.gameObject.name} 获取组建失败：" + transformPath);
            }
            return temp.GetComponent<T>();
        }
        public static T[] GetComponents<T>(this Component self, string transformPath)
            where T : Component
        {
            if (!self)
                return null;
            var temp = self.transform.Find(transformPath);
            if (!temp)
            {
                throw new Exception($"{self.gameObject.name} 获取组建失败：" + transformPath);
            }
            return temp.GetComponents<T>();
        }
        public static T GetComponentInChildren<T>(this Component self, string transformPath)
            where T : Component
        {
            if (!self)
                return null;
            var temp = self.transform.Find(transformPath);
            if (!temp)
            {
                throw new Exception($"{self.gameObject.name} 获取组建失败：" + transformPath);
            }
            return temp.GetComponentInChildren<T>();
        }
        public static T[] GetComponentsInChildren<T>(this Component self, string transformPath)
            where T : Component
        {
            if (!self)
                return null;
            var temp = self.transform.Find(transformPath);
            if (!temp)
            {
                throw new Exception($"{self.gameObject.name} 获取组建失败：" + transformPath);
            }
            return temp.GetComponentsInChildren<T>();
        }
        
        
        public static T GetOrAddComponent<T>(this Component self)
            where T:Component
        {
            var ans = self.GetComponent<T>();
            if (!ans)
                ans = self.gameObject.AddComponent<T>();
            return ans;
        }     

        public static Component GetOrAddComponent(this Component self, Type componentType)
        {
            var com = self.GetComponent(componentType);
            if (!com)
                com = self.gameObject.AddComponent(componentType);
            Assert.IsTrue(com);
            return com;
        }
    }
}