using UnityEngine;

namespace ReadyGamerOne.Utility
{
    public static class ColorExtension
    {
        public static bool EqualsTo(this Color self, Color other,float tolerance=0.01f)
        {
            return Mathf.Abs(self.r - other.r) <= tolerance
                   && Mathf.Abs(self.g - other.g) <= tolerance
                   && Mathf.Abs(self.b - other.b) <= tolerance
                   && Mathf.Abs(self.a - other.a) <= tolerance;
        }
    }
}