using UnityEngine;

namespace Dao.CameraSystem
{
    public class CameraController : Singleton<CameraController>
    {
        public float BoundWidth { get; set; } = 50;

        public bool Enable { get; set; }

        public float MoveSpeed { get; set; }
        public Vector2 MoveRange { get; set; }

        private Camera m_camera;

        private Transform m_player;

        public CameraController()
        {
            m_player = FindUtility.Find("Player").transform;
        }

        public void BindCamera(Camera camera)
        {
            m_camera = camera;
        }

        public void SetPosition(Vector3 position)
        {
            m_camera.transform.position = position;
        }

        public Vector3 GetPosition()
        {
            return m_camera.transform.position;
        }

        public Rect GetScreenRect()
        {
            Vector3 leftBottom = m_camera.ScreenToWorldPoint(Vector3.zero);
            Vector3 rightTop = m_camera.ScreenToWorldPoint(Vector3.right * Screen.width + Vector3.up * Screen.height);
            float x = leftBottom.x;
            float y = leftBottom.y;
            float width = rightTop.x - leftBottom.x;
            float height = rightTop.y - leftBottom.y;
            return new Rect(x, y, width, height);
        }

        public void Update(float deltaTime)
        {
            if (!Enable) return;

            //Vector3 mousePos = Input.mousePosition;
            //if (mousePos.x < BoundWidth && m_camera.transform.position.x > MoveRange.x)
            //{
            //    m_camera.transform.Translate(Vector3.left * MoveSpeed * deltaTime);
            //}
            //else if (mousePos.x > Screen.width - BoundWidth && m_camera.transform.position.x < MoveRange.y)
            //{
            //    m_camera.transform.Translate(Vector3.right * MoveSpeed * deltaTime);
            //}

            float target = Mathf.Clamp(m_player.position.x, MoveRange.x, MoveRange.y);
            float current = m_camera.transform.position.x;
            if ((target - current).Abs() > 0.01f)
            {
                m_camera.transform.position = new Vector3(Mathf.Lerp(current, target, 0.01f), 0, -10);
            }
        }
    }
}
