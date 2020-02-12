using UnityEngine;

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
    }
}