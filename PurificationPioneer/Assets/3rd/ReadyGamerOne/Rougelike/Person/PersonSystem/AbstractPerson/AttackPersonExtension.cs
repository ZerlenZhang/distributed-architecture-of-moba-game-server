using System;
using UnityEngine;

namespace ReadyGamerOne.Rougelike.Person
{
    public static class AttackPersonExtension
    {


        /// <summary>
        /// 尝试攻击另一单位
        /// </summary>
        /// <param name="self">自身角色</param>
        /// <param name="other">攻击目标</param>
        /// <param name="damageScale">攻击缩放，比如技能可能造成200%伤害</param>
        /// <returns>返回是否进行了有效攻击</returns>
        /// <exception cref="Exception">self为空</exception>
        public static bool TryAttack(this AbstractPerson self, AbstractPerson other, float? damageScale=null)
        {
            var ds = damageScale ?? 1.0f;
            
            var selfAtk = self as ITakeDamageablePerson<AbstractPerson,BasicDamage>;
            var otherAtk = other as ITakeDamageablePerson<AbstractPerson,BasicDamage>;

            if(null==self)
                throw new Exception("自身角色为空？");

            if (null == other)
                return false;
            
            if (!self.IsAlive || !other.IsAlive)
                return false;
            
            if (null == selfAtk || null == otherAtk)
                return false;

            var damage = self.CalculateDamage(ds, other);
            
            selfAtk.OnCauseDamage(other,damage);

            return true;
        }
        

        /// <summary>
        /// 尝试攻击另一单位，需要IFightablePerson<T>接口
        /// </summary>
        /// <param name="self"></param>
        /// <param name="other"></param>
        /// <param name="damageScale"></param>
        /// <returns>有效攻击返回True，否则返回False</returns>
        public static bool TryAttack(this IPerson self, IPerson other, float? damageScale=null)
        {
            if (self == null || other == null)
                return false;
            
            return self.gameObject.TryAttack(other.gameObject,damageScale);
        }
        

        /// <summary>
        /// 尝试用一个GameObject对另一单位造成伤害
        /// </summary>
        /// <param name="self"></param>
        /// <param name="other"></param>
        /// <param name="damageScale"></param>
        /// <returns></returns>
        public static bool TryAttack(this GameObject self, GameObject other, float? damageScale=null)
        {
            if (self == null || other == null)
                return false;

            var selfPerson = self.GetPersonInfo();
            var otherPerson = other.GetPersonInfo();

            return selfPerson.TryAttack(otherPerson,damageScale);
        }
    }
}