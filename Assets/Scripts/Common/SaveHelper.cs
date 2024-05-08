using System.IO;
using System.Text;

namespace Dao
{
    public class SaveHelper
    {
        public static void Save(Json json, string path)
        {
            if (!Directory.GetParent(path).Exists)
            {
                return;
            }
            string jsonText = JsonMapper.JsonToString(json);
            byte[] bytes = Encoding.UTF8.GetBytes(jsonText);
            FileStream stream = File.Create(path);
            stream.Write(bytes);
            stream.Close();
        }

        public static Json Load(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }
            FileStream stream = File.Open(path, FileMode.Open);
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes);
            stream.Close();
            string json = Encoding.UTF8.GetString(bytes);
            return JsonMapper.StringToJson(json);
        }
    }
}
