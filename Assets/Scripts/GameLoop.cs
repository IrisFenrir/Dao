using UnityEngine;

public class GameLoop : MonoBehaviour
{
    public static GameLoop Instance { get; private set; }

    public float cameraMoveSpeed = 3;
    public float cameraMoveAreaWidth = 50;
    public Vector2 cameraMoveRange;

    [Header("UI")]
    public GameObject puzzle1_boxPanel;
    public Sprite switchON;
    public Sprite switchOFF;
    public GameObject notePanel;
    public GameObject wordItemPrefab;

    [Header("Word")]
    public Sprite onWord;
    public Sprite offWord;

    [Header("Scene")]
    public GameObject scene1;
    public GameObject scene2;

    private InteractManager m_interactManager;

    private void Awake()
    {
        Instance ??= this;

        //m_cameraController = new CameraController(Camera.main);
        CameraController.Instance.Init(Camera.main);
        CameraController.Instance.MoveSpeed = cameraMoveSpeed;
        CameraController.Instance.MoveAreaWidth = 50;
        CameraController.Instance.MoveRange = cameraMoveRange;

        UIManager.Instance.AddPanel("Puzzle1_Box", new UIPanel(puzzle1_boxPanel) { name = "Puzzle1_Box" });
        UINote note = new UINote(notePanel, wordItemPrefab);
        UIManager.Instance.AddPanel("Note", note);

        WordManager.Instance.AddWord("Open", "¿ª", onWord);
        WordManager.Instance.AddWord("Close", "¹Ø", offWord);
        m_interactManager = new InteractManager();

        //WordManager.Instance.AddFoundWord("Open");
        //WordManager.Instance.AddFoundWord("Close");

        //UIManager.Instance.OpenPanel("Note");
        scene1.SetActive(true);
        scene2.SetActive(false);
    }

    private void Update()
    {
        CameraController.Instance.Update();

        m_interactManager.Update();
    }

    public void ChangeScene()
    {
        scene1.SetActive(false);
        scene2.SetActive(true);
        CameraController.Instance.Enable = true;
    }
}
