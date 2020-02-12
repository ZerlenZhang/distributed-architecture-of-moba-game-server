using UnityEngine;

namespace ReadyGamerOne.Utility
{
    public static class BoxCollider2DExtension
    {
        public static void GetCorners(this BoxCollider2D collider2D,out Vector3[] corners)
        {
            var offset = collider2D.offset;
            var size = collider2D.size;
            var scale = collider2D.transform.localScale;
            var startPos = collider2D.transform.position;
    
            var c = startPos + new Vector3(scale.x * offset.x, scale.y * offset.y);
    
            var height = size.y * scale.y;
            var width = size.x * scale.x;
    
            var lt = c + new Vector3(-width / 2, height / 2);

            corners = new[]
            {
                lt + new Vector3(0, -height, 0),
                lt,
                lt + new Vector3(width, 0, 0),
                lt + new Vector3(width, -height, 0)
            };
        }
    }
}