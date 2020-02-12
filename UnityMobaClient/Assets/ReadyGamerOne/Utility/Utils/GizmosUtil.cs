using UnityEngine;

namespace ReadyGamerOne.Utility
{
    public static class GizmosUtil
    {
        public static void DrawSign(Vector3 pos,float signalSize=1.0f)
        {
            Gizmos.DrawLine(pos + Vector3.left * signalSize, pos + Vector3.right * signalSize);
            Gizmos.DrawLine(pos + Vector3.up * signalSize, pos + Vector3.down * signalSize);
        }

        public static void DrawRect(Rect rect,Vector3 pos)
        {
            var p = pos + new Vector3(rect.position.x, rect.position.y, 0);
            var h = rect.height;
            var w = rect.width;

            Gizmos.DrawLine(p, p + new Vector3(w, 0, 0));
            Gizmos.DrawLine(p, p + new Vector3(0, h, 0));           
            Gizmos.DrawLine(p+new Vector3(w,h,0), p + new Vector3(w, 0, 0));
            Gizmos.DrawLine(p+new Vector3(w,h,0), p + new Vector3(0, h, 0));
        }
        
        public static void DrawRect(Rect rect)=>DrawRect(rect,Vector3.zero);
        
        public static void DrawBoxCollider2D(BoxCollider2D collider2D)
        {
            if (collider2D == null)
            {
                Debug.LogError("BoxCollider2D 为空");
                return;
            }
            
            var offset = collider2D.offset;
            var size = collider2D.size;
            var scale = collider2D.transform.localScale;
            var startPos = collider2D.transform.position;
    
            var c = startPos + new Vector3(scale.x * offset.x, scale.y * offset.y);
    
            var height = size.y * scale.y;
            var width = size.x * scale.x;
    
            var lt = c + new Vector3(-width / 2, height / 2);
            Gizmos.DrawLine(lt, lt + new Vector3(width, 0, 0));
            Gizmos.DrawLine(lt + new Vector3(0, -height, 0), lt + new Vector3(width, -height, 0));
            Gizmos.DrawLine(lt, lt + new Vector3(0, -height, 0));
            Gizmos.DrawLine(lt + new Vector3(width, 0, 0), lt + new Vector3(width, -height, 0));
        }
    }
}