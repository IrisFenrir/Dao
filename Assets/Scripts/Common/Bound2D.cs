using UnityEngine;

namespace Dao
{
    public class Bound2D : MonoBehaviour
    {
        public Vector2 center;
        public float width;
        public float height;
        public Color Color = new Color(177f / 255, 253f / 255, 89f / 255);

        public Rect Rect
        {
            get
            {
                Vector3 centerPos = transform.position;
                Vector3 leftTop = centerPos - Vector3.right * width * center.x + Vector3.up * height * (1 - center.y);
                return new Rect(leftTop.x, leftTop.y, width, height);
            }
        }

        private void OnDrawGizmos()
        {
            Vector3 centerPos = transform.position;
            Vector3 leftTop = centerPos - Vector3.right * width * center.x + Vector3.up * height * (1 - center.y);
            Vector3 rightTop = leftTop + Vector3.right * width;
            Vector3 rightBottom = rightTop - Vector3.up * height;
            Vector3 leftBottom = leftTop - Vector3.up * height;

            Gizmos.color = Color;
            Gizmos.DrawLine(leftTop, rightTop);
            Gizmos.DrawLine(rightTop, rightBottom);
            Gizmos.DrawLine(rightBottom, leftBottom);
            Gizmos.DrawLine(leftBottom, leftTop);
        }
    }
}
