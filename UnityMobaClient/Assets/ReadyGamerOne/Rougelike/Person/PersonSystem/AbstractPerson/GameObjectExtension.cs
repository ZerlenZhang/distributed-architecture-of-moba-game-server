using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace ReadyGamerOne.Rougelike.Person
{    
    /// <summary>
    /// 拓展GameObject快速获取角色信息的引用
    /// </summary>
    public static class GameObjectExtension
    {
        public static T GetPersonInfo<T>(this GameObject obj)
            where T : AbstractPerson
        {
            if (obj == null)
                return null;
            var id = obj.GetComponent<AbstractPerson.PersonIdentity>();
            if (id == null)
                return null;
            return id.abstractPerson as T;
        } 
        
        public static AbstractPerson GetPersonInfo(this GameObject obj)
        {
            var id = obj.GetComponent<AbstractPerson.PersonIdentity>();
            if (id == null)
                return null;
            return id.abstractPerson;
        }

        public static T GetOrAddComponent<T>(this GameObject self) where T : Component
        {
            var com = self.GetComponent<T>();
            if (!com)
                com = self.AddComponent<T>();
            Assert.IsTrue(com);
            return com;
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