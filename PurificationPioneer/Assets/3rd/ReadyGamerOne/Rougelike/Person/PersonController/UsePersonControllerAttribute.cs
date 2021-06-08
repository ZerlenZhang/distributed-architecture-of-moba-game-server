using System;

namespace ReadyGamerOne.Rougelike.Person
{
    [AttributeUsage(AttributeTargets.Class)]
    public class UsePersonControllerAttribute:System.Attribute
    {
        public Type controllerType;

        public UsePersonControllerAttribute(Type controllerType)
        {
            this.controllerType = controllerType;
        }
    }
}