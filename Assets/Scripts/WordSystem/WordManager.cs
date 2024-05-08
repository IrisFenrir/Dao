using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Dao.WordSystem
{
    public class WordManager : Singleton<WordManager>
    {
        private Dictionary<string, Word> m_words = new();

        public void Import(string path)
        {
            StreamReader reader = new StreamReader(path);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var data = line.Trim().Split(',');
                string id = data[0];
                string imageName = data[1];
                string standardTranslation = data[2];
                Word word = new Word(id, standardTranslation, imageName);
                m_words.Add(id, word);
            }
        }

        public Word GetWord(string name)
        {
            return m_words[name];
        }

        public void GetAnswer(string name)
        {
            m_words[name].GetAnswer();
        }
    }
}
