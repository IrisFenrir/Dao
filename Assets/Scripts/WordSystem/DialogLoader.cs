using System.Collections.Generic;

namespace Dao.WordSystem
{
    public class DialogLoader : Singleton<DialogLoader>
    {
        private Dictionary<string, IDialog> m_dialogs = new();

        public void Import(string path)
        {
            var dataArray = SaveHelper.Load(path).Array;
            foreach (var data in dataArray)
            {
                string id = data["id"];
                IDialog dialog = LoadDialog(data["sentence"].Array);
                m_dialogs.Add(id, dialog);
            }
        }

        public IDialog GetDialog(string id)
        {
            return m_dialogs[id];
        }

        private IDialog LoadDialog(List<Json> sentenceData)
        {
            IDialog[] dialogs = new IDialog[sentenceData.Count];
            for (int i = 0; i < sentenceData.Count; i++)
            {
                string type = sentenceData[i]["type"];
                if (type == "Normal")
                {
                    dialogs[i] = new NormalDialog(sentenceData[i]["content"]);
                }
                else if (type == "Ciphertext")
                {
                    List<string> wordID = new List<string>();
                    foreach (var item in sentenceData[i]["content"].Array)
                    {
                        wordID.Add(item);
                    }
                    string translation = sentenceData[i]["translation"];
                    Sentence sentence = new Sentence(wordID, translation);
                    dialogs[i] = new CiphertextDialog(sentence);
                }
            }
            for (int i = 0; i < sentenceData.Count - 1; i++)
            {
                dialogs[i].Next[0] = dialogs[i + 1];
            }
            return dialogs[0];
        }
    }
}
