using Dao.CameraSystem;
using Dao.InventorySystem;
using Dao.SceneSystem;
using Dao.WordSystem;
using UnityEngine;
using static Dao.Common.GameUtility;

public class GameLoop : MonoBehaviour
{
    public static GameLoop Instance { get; private set; }

    [Header("Common")]
    public float transitionLerpSpeed = 0.05f;
    public TransitionDirection transitionDirection;

    [Header("EntryRoom")]
    public float handFadeTime = 2f;
    public float handDelayTime = 1.5f;

    [Header("LivingRoom")]
    public Sprite boxSwitchOn;
    public Sprite boxSwitchOff;

    private void Awake()
    {
        Instance ??= this;
    }

    private void Start()
    {
        CameraController.Instance.BindCamera(Camera.main);
        //CameraController.Instance.MoveSpeed = cameraMoveSpeed;

        string dialogDataPath = Application.streamingAssetsPath + "/Data/DialogData.json";

        WordManager.Instance.Import(Application.streamingAssetsPath + "/Data/Ciphertext.csv");
        DialogUtility.Import(Application.streamingAssetsPath + "/Data/Sentence.json");
        //DialogManager.Instance.Build(ciphertextPath, dialogDataPath);

        //UITranslationManager.Instance.SetPrefab(uiTranslation);

        SceneManager.Instance.AddScene("EntryRoom", new EntryRoom());
        SceneManager.Instance.AddScene("LivingRoom", new LivingRoom());
        SceneManager.Instance.AddScene("Kitchen", new Kitchen());
        SceneManager.Instance.AddScene("Bedroom", new Bedroom());
        SceneManager.Instance.LoadScene("EntryRoom");
    }

    private void Update()
    {
        SceneManager.Instance.Update(Time.deltaTime);
        CameraController.Instance.Update(Time.deltaTime);
        UIDialogManager.Instance.Update(Time.deltaTime);
        InventoryManager.Instance.Update(Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Y))
        {
            SceneManager.Instance.GetScene<LivingRoom>("LivingRoom").OpenMedicalCase();
        }

        
    }
}

