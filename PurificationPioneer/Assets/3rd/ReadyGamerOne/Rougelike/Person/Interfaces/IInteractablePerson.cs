using System;

namespace ReadyGamerOne.Rougelike.Person
{
    public interface IInteractablePerson
    {
        bool IfInteract { get; }
        Action<AbstractPerson> OnInteract { get; set; }
    }
}