using Dao;
using System.IO;
using System.Text;
using UnityEngine;

public class DataTest : MonoBehaviour
{
    private void Start()
    {
        //string filePath = Path.Combine(Application.streamingAssetsPath, "Data/DialogData.json");
        string filePath = Application.streamingAssetsPath + "/Data/DialogData.json";
        Debug.Log(filePath);
        FileStream stream = File.Open(filePath, FileMode.Open);
        byte[] bytes = new byte[stream.Length];
        stream.Read(bytes);
        stream.Close();
        string jsonStr = Encoding.UTF8.GetString(bytes);
        Json json = JsonMapper.StringToJson(jsonStr);

        Json scene = json.Array[0];
        Debug.Log((string)scene["Scene"]);
        var dialogs = scene["Dialogs"].Array;
        foreach (Json dialog in dialogs)
        {
            string item = dialog["Item"];
            var sentences = dialog["Sentence"].Array;
            foreach (var sentence in sentences)
            {
                Debug.Log((string)sentence["Content"]);
            }
        }
    }
}
