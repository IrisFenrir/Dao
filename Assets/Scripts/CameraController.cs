using UnityEngine;

public class CameraController : Singleton<CameraController>
{
    public bool Enable { get; set; }

    public float MoveSpeed { get; set; } = 1;
    public float MoveAreaWidth { get; set; } = 50;
    public Vector2 MoveRange { get; set; }


    private Camera m_camera;

    public void Init(Camera camera)
    {
        m_camera = camera;
    }

    public void Update()
    {
        if (!Enable) return;

        Vector3 mousePos = Input.mousePosition;

        Vector3 pos = m_camera.transform.position;
        if (mousePos.x < MoveAreaWidth)
        {
            
            if (pos.x > MoveRange.x)
            {
                m_camera.transform.position = new Vector3(Mathf.Max(pos.x - MoveSpeed * Time.deltaTime, MoveRange.x), pos.y, pos.z);
            }
        }
        else if (mousePos.x > Screen.width - MoveAreaWidth)
        {
            if (pos.x < MoveRange.y)
            {
                m_camera.transform.position = new Vector3(Mathf.Min(pos.x + MoveSpeed * Time.deltaTime, MoveRange.y), pos.y, pos.z);
            }
        }
    }
}
