using System.Collections.Generic;
using System.Linq;
using Es.InkPainter;
using ReadyGamerOne.Utility;
using UnityEngine;

namespace PurificationPioneer.Script
{
    public static class InkCanvasMgr
    {
        /// <summary>
        /// 计算给定几种颜色所占据的比率
        /// </summary>
        /// <param name="targetColors"></param>
        /// <returns></returns>
        public static Dictionary<Color, float> CalculateColor(HashSet<Color> targetColors)
        {
            var ans = new Dictionary<Color, float>();
            foreach (var inkCanvas in Object.FindObjectsOfType<InkCanvas>())
            {
                var weight = inkCanvas.transform.lossyScale.x;
                foreach (var inkCanvasPaintData in inkCanvas.PaintDatas)
                {
                    var mainTex = inkCanvasPaintData.mainTexture as RenderTexture;
                    var texture2D = TextureUtil.CreateTexture2D(mainTex);

                    for (var x = 0; x < texture2D.width; x++)
                    {
                        for (var y = 0; y < texture2D.height; y++)
                        {
                            var color = texture2D.GetPixel(x, y);
                            
                            if(!targetColors.Contains(color))
                                continue;
                            
                            if (ans.ContainsKey(color))
                            {
                                ans[color] += weight;
                            }
                            else
                            {
                                ans.Add(color, weight);
                            }
                        }
                    }
                    
                }
            }

            var all = 
                ans.Sum(kv => kv.Value);

            foreach (var color in targetColors)
            {
                ans[color] /= all;
            }
            
            return ans;
        }
    }
}