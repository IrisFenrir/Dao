using System.Collections.Generic;
using UnityEngine;

namespace Dao.WordSystem
{
    public class Sentence
    {
        public string translation { get; private set; }

        private List<Word> m_words;

        public Sentence(List<string> wordsID, string translation)
        {
            this.translation = translation;
            m_words = new();
            foreach (var id in wordsID)
            {
                m_words.Add(WordManager.Instance.GetWord(id));
            }
        }

        public bool CanShowTranslation()
        {
            return m_words.TrueForAll(word => word.getAnswer);
        }

        public int WordCount()
        {
            return m_words.Count;
        }

        public Sprite GetWordSprite(int index)
        {
            return m_words[index].image;
        }

        public string GetWordTranslation(int index)
        {
            return m_words[index].GetTranslation();
        }

        public void RecordWords()
        {
            m_words.ForEach(w => UIDictionary.Instance.AddWord(w));
        }
    }
}
