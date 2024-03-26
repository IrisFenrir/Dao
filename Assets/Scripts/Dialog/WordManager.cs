using System.Collections.Generic;
using UnityEngine;

public class WordManager : Singleton<WordManager>
{
    private Dictionary<string, Word> m_words = new();

    public List<Word> FoundWords => m_foundWords;
    private List<Word> m_foundWords = new();

    public void AddFoundWord(string wordName)
    {
        if (m_words.TryGetValue(wordName, out Word word))
        {
            m_foundWords.Add(word);
            UINote note = UIManager.Instance.GetPanel("Note") as UINote;
            note.GenerateWordItem(word);
        }
    }

    public void AddFoundWord(Word word)
    {
        if (!m_foundWords.Contains(word))
        {
            m_foundWords.Add(word);
            Debug.Log(word.StandardTranslation);
            UINote note = UIManager.Instance.GetPanel("Note") as UINote;
            note.GenerateWordItem(word);
        }
    }

    public void AddWord(string wordName, string wordTranslation, Sprite gameWordImage = null)
    {
        Word word = new Word() { StandardTranslation = wordTranslation, GameWord = gameWordImage };
        m_words.TryAdd(wordName, word);
    }

    public Word GetWord(string wordName)
    {
        if (m_words.TryGetValue(wordName, out Word word))
            return word;
        return null;
    }
}

