using UnityEngine;

namespace ReadyGamerOne.Utility
{
    public static class TextureUtil
    {
        public static Texture2D CreateTexture2D(RenderTexture rt)
        {
            var before = RenderTexture.active;
            var width = rt.width;
            var height = rt.height;

            var ans = new Texture2D(width, height, TextureFormat.ARGB32, false);

            RenderTexture.active = rt;
            
            ans.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            ans.Apply();

            RenderTexture.active = before;
            
            return ans;
        }
    }
}