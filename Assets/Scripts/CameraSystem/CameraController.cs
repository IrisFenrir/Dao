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

        public void BindCamera(Camera camera)
        {
            m_camera = camera;
        }

        public void SetPosition(Vector3 position)
        {
            m_camera.transform.position = position;
        }

        public void Update(float deltaTime)
        {
            if (!Enable) return;

            Vector3 mousePos = Input.mousePosition;
            if (mousePos.x < BoundWidth && m_camera.transform.position.x > MoveRange.x)
            {
                m_camera.transform.Translate(Vector3.left * MoveSpeed * deltaTime);
            }
            else if (mousePos.x > Screen.width - BoundWidth && m_camera.transform.position.x < MoveRange.y)
            {
                m_camera.transform.Translate(Vector3.right * MoveSpeed * deltaTime);
            }
        }
    }
}
