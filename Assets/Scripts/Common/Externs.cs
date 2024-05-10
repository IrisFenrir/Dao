using UnityEngine;

namespace Dao
{
    public static class Externs
    {
        public static bool Contain(this Bounds bounds, Vector3 pos)
        {
            var leftBottom = bounds.center - Vector3.right * bounds.extents.x - Vector3.up * bounds.extents.y;
            var rightTop = bounds.center + Vector3.right * bounds.extents.x + Vector3.up * bounds.extents.y;
            return pos.x >= leftBottom.x && pos.x <= rightTop.x && pos.y >= leftBottom.y && pos.y <= rightTop.y;
        }
    }
}
