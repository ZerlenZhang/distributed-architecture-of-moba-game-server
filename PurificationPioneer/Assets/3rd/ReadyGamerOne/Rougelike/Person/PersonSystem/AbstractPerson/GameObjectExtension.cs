using UnityEngine;

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
            if (id) 
                return id.abstractPerson;

            if (!obj.transform.parent) 
                return null;
            
            return obj.GetComponentInParent<AbstractPerson.PersonIdentity>()?.abstractPerson;
        }
    }
}