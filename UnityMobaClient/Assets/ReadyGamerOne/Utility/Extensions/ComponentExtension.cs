using UnityEngine;

namespace ReadyGamerOne.Utility
{
    public static class ComponentExtension
    {
        public static T GetComponent<T>(this Component self, string transformPath)
            where T : Component
        {
            if (!self)
                return null;
            return self.transform.Find(transformPath).GetComponent<T>();
        }
        public static T[] GetComponents<T>(this Component self, string transformPath)
            where T : Component
        {
            if (!self)
                return null;
            return self.transform.Find(transformPath).GetComponents<T>();
        }
        public static T GetComponentInChildren<T>(this Component self, string transformPath)
            where T : Component
        {
            if (!self)
                return null;
            return self.transform.Find(transformPath).GetComponentInChildren<T>();
        }
        public static T[] GetComponentsInChildren<T>(this Component self, string transformPath)
            where T : Component
        {
            if (!self)
                return null;
            return self.transform.Find(transformPath).GetComponentsInChildren<T>();
        }
    }
}