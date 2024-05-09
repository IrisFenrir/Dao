using Dao.CameraSystem;
using Dao.SceneSystem;
using UnityEngine;

public class GameLoop : MonoBehaviour
{
    public static GameLoop Instance { get; private set; }

    public float cameraMoveSpeed = 60;

    [Header("Room1")]
    public Vector2 doorHandleAngleRange;

    private void Awake()
    {
        Instance ??= this;
    }

    private void Start()
    {
        CameraController.Instance.BindCamera(Camera.main);
        CameraController.Instance.MoveSpeed = cameraMoveSpeed;

        SceneManager.Instance.AddScene("Room1", new Room1());
        SceneManager.Instance.AddScene("Room2", new Room2());
        SceneManager.Instance.AddScene("Room2", new Room3());
        SceneManager.Instance.LoadScene("Room1");
    }

    private void Update()
    {
        SceneManager.Instance.Update(Time.deltaTime);
        CameraController.Instance.Update(Time.deltaTime);

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    SceneManager.Instance.LoadScene("Room2");
        //}
    }
}

