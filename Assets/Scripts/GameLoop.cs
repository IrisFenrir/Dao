using Dao.CameraSystem;
using Dao.SceneSystem;
using Dao.WordSystem;
using UnityEngine;

public class GameLoop : MonoBehaviour
{
    public static GameLoop Instance { get; private set; }

    public float cameraMoveSpeed = 60;

    [Header("UI")]
    public GameObject sentenceWordItem;
    public GameObject dictionaryPage;
    public GameObject dictionaryWordItem;
    public GameObject wordContextPage;
    public GameObject wordContextItem;
    public GameObject uiTranslation;

    [Header("EntryRoom")]
    public Vector2 doorHandleAngleRange;

    [Header("LivingRoom")]
    public Sprite boxSwitchOn;
    public Sprite boxSwitchOff;

    private void Awake()
    {
        Instance ??= this;
    }

    private void Start()
    {
        GetComponent<Test>().value++;

        CameraController.Instance.BindCamera(Camera.main);
        CameraController.Instance.MoveSpeed = cameraMoveSpeed;

        string dialogDataPath = Application.streamingAssetsPath + "/Data/DialogData.json";

        WordManager.Instance.Import(Application.streamingAssetsPath + "/Data/Ciphertext.csv");
        DialogUtility.Import(Application.streamingAssetsPath + "/Data/Sentence.json");
        //DialogManager.Instance.Build(ciphertextPath, dialogDataPath);

        //UITranslationManager.Instance.SetPrefab(uiTranslation);

        SceneManager.Instance.AddScene("EntryRoom", new EntryRoom());
        SceneManager.Instance.AddScene("LivingRoom", new LivingRoom());
        SceneManager.Instance.AddScene("Kitchen", new Kitchen());
        SceneManager.Instance.LoadScene("Kitchen");

        
    }

    private void Update()
    {
        SceneManager.Instance.Update(Time.deltaTime);
        CameraController.Instance.Update(Time.deltaTime);
        UIDialogManager.Instance.Update(Time.deltaTime);
        //DialogManager.Instance.Update(Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Y))
        {
            SceneManager.Instance.GetScene<LivingRoom>("LivingRoom").OpenMedicalCase();
        }
    }
}

