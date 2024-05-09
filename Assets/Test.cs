using Dao.WordSystem;
using UnityEngine;
using System.Collections.Generic;

public class Test : MonoBehaviour
{
    public int value = 0;

    private void Start()
    {
        //NormalDialog dialog = new NormalDialog("°¡°¡°¡°¡°¡°¡");
        //WordManager.Instance.Import(Application.streamingAssetsPath + "/Data/Ciphertext.csv");
        //Sentence sentence = new Sentence(new List<string> { "I", "You", "He", "Water" }, "¹¾¹¾¹¾");
        //CiphertextDialog dialog1 = new CiphertextDialog(sentence);
        //dialog.Next[0] = dialog1;
        //UIDialogManager.Instance.StartDialog(dialog);
        WordManager.Instance.Import(Application.streamingAssetsPath + "/Data/Ciphertext.csv");
        DialogUtility.Import(Application.streamingAssetsPath + "/Data/Sentence.json");
        UIDialogManager.Instance.StartDialog(DialogUtility.GetDialog("LivingRoom-NearDoor"));
    }

    private void Update()
    {
        UIDialogManager.Instance.Update(Time.deltaTime);
    }
}
