using System;
using UnityEngine;

namespace ReadyGamerOne.Rougelike.Person
{
    public static class AttackPersonExtension
    {
        /// <summary>
        /// 尝试攻击另一单位
        /// </summary>
        /// <param name="self"></param>
        /// <param name="other"></param>
        /// <param name="damage"></param>
        /// <returns>有效攻击返回True，否则返回False</returns>
        public static bool TryAttack(this AbstractPerson self, AbstractPerson other, int damage)
        {
            if (damage == 0)
                return false;
            
            var selfAtk = self as ITakeDamageablePerson<AbstractPerson>;
            var otherAtk = other as ITakeDamageablePerson<AbstractPerson>;

            if(null==self)
                throw new Exception("自身角色为空？");

            if (null == other)
                return false;

            if (!self.IsAlive || !other.IsAlive)
                return false;
            
            if (null == selfAtk || null == otherAtk)
                return false;

            selfAtk.OnCauseDamage(other, damage);
            otherAtk.OnTakeDamage(self, damage);

            return true;
        }

        /// <summary>
        /// 尝试攻击另一单位，damage就是攻击力
        /// </summary>
        /// <param name="self"></param>
        /// <param name="other"></param>
        /// <param name="damage"></param>
        /// <returns>有效攻击返回True，否则返回False</returns>
        public static bool TryAttackSimple(this AbstractPerson self, AbstractPerson other)
        {
            var selfAtk = self as ITakeDamageablePerson<AbstractPerson>;
            var otherAtk = other as ITakeDamageablePerson<AbstractPerson>;

            if(null==self)
                throw new Exception("自身角色为空？");

            if (null == other)
                return false;

            if (!self.IsAlive || !other.IsAlive)
                return false;
            
            if (null == selfAtk || null == otherAtk)
                return false;

            selfAtk.OnCauseDamage(other, self.Attack);
            otherAtk.OnTakeDamage(self, self.Attack);

            return true;
        }

        /// <summary>
        /// 尝试攻击另一单位，需要IFightablePerson<T>接口
        /// </summary>
        /// <param name="self"></param>
        /// <param name="other"></param>
        /// <param name="damage"></param>
        /// <returns>有效攻击返回True，否则返回False</returns>
        public static bool TryAttack(this IPerson self, IPerson other,int damage)
        {
            return self.gameObject.TryAttackAsPerson(other.gameObject,damage);
        }
        
        /// <summary>
        /// 尝试攻击另一单位，damage就是攻击力，需要IFightablePerson<T>接口
        /// </summary>
        /// <param name="self"></param>
        /// <param name="other"></param>
        /// <param name="damage"></param>
        /// <returns>有效攻击返回True，否则返回False</returns>
        public static bool TryAttackSimple(this IPerson self, IPerson other)
        {
            return self.gameObject.TryAttackSimpleAsPerson(other.gameObject);
        }
        
        /// <summary>
        /// 尝试用一个GameObject对另一单位造成伤害
        /// </summary>
        /// <param name="self"></param>
        /// <param name="other"></param>
        /// <param name="damage"></param>
        /// <returns></returns>
        public static bool TryAttackAsPerson(this GameObject self, GameObject other, int damage)
        {
            if (damage == 0)
                return false;
            
            if (self == null || other == null)
                return false;

            var selfPerson = self.GetPersonInfo();
            var otherPerson = other.GetPersonInfo();
            return selfPerson.TryAttack(otherPerson,damage);
        }

        /// <summary>
        /// 尝试用一个GameObject对另一单位造成伤害，伤害就是攻击力
        /// </summary>
        /// <param name="self"></param>
        /// <param name="other"></param>
        /// <param name="damage"></param>
        /// <returns></returns>
        public static bool TryAttackSimpleAsPerson(this GameObject self, GameObject other)
        {
            if (self == null || other == null)
                return false;

            var selfPerson = self.GetPersonInfo();
            var otherPerson = other.GetPersonInfo();

            return selfPerson.TryAttackSimple(otherPerson);
        }
    }
}