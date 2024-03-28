using Dao.CameraSystem;
using Dao.SceneSystem;
using UnityEngine;

public class GameLoop : MonoBehaviour
{
    public float cameraMoveSpeed = 60;

    private void Start()
    {
        CameraController.Instance.BindCamera(Camera.main);
        CameraController.Instance.MoveSpeed = cameraMoveSpeed;

        SceneManager.Instance.AddScene("Room1", new Room1());
        SceneManager.Instance.AddScene("Room2", new Room2());
        SceneManager.Instance.LoadScene("Room1");
    }

    private void Update()
    {
        SceneManager.Instance.Update(Time.deltaTime);
        CameraController.Instance.Update(Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.Instance.LoadScene("Room2");
        }
    }
}

