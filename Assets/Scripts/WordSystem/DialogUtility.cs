using System.Collections.Generic;

namespace Dao.WordSystem
{
    public class DialogUtility
    {
        private static Dictionary<string, IDialog> m_dialogs = new();

        public static void Import(string path)
        {
            var dataArray = SaveHelper.Load(path).Array;
            foreach (var data in dataArray)
            {
                string id = data["id"];
                IDialog dialog = LoadDialog(data["sentence"].Array);
                m_dialogs.Add(id, dialog);
            }
        }

        public static IDialog GetDialog(string id)
        {
            return m_dialogs[id];
        }

        private static IDialog LoadDialog(List<Json> sentenceData)
        {
            if (sentenceData.Count == 0)
                return null;
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
                else if (type == "Select")
                {
                    var options = sentenceData[i]["options"].Array;
                    string[] optionContent = new string[options.Count];
                    IDialog[] optionDialog = new IDialog[options.Count];
                    for (int j = 0; j < options.Count; j++)
                    {
                        var optionData = options[j];
                        optionContent[j] = optionData["option"];
                        optionDialog[j] = LoadDialog(optionData["sentence"].Array);
                    }
                    SelectDialog dialog = new SelectDialog(optionContent);
                    for (int j = 0; j < options.Count; j++)
                    {
                        dialog.Next[j] = optionDialog[j];
                    }
                    dialogs[i] = dialog;
                }
            }
            for (int i = 0; i < sentenceData.Count - 1; i++)
            {
                dialogs[i].Next[0] = dialogs[i + 1];
            }
            return dialogs[0];
        }

        public static SelectDialog SearchSelectDialog(IDialog dialog)
        {
            while (dialog is SelectDialog == false)
            {
                dialog = dialog.Next[0];
            }
            return dialog as SelectDialog;

        }
    }
}
