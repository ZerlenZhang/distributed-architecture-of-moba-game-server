
using System;

namespace ReadyGamerOne.Rougelike.Person
{
    /// <summary>
    /// 可以制造和承受伤害
    /// </summary>
    /// <typeparam name="PersonType"></typeparam>
    public interface ITakeDamageablePerson<PersonType>
        where PersonType:AbstractPerson
    {
        event Action<AbstractPerson, int> onCauseDamage;
        event Action<AbstractPerson, int> onTakeDamage;
        void OnTakeDamage(PersonType takeDamageFrom,int damage);
        void OnCauseDamage(PersonType causeDamageTo,int damage);

    }
}