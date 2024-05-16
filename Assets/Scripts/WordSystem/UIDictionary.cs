using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dao.WordSystem
{
    public class UIDictionary : Singleton<UIDictionary>
    {
        public bool Enable => m_root.activeInHierarchy;

        private GameObject m_root;

        private GameObject[] m_pages;

        private int m_pageIndex;

        private List<Word> m_words = new();
        private int m_wordIndex = 0;

        private Word m_currentEditWord;

        public UIDictionary()
        {
            m_root = FindUtility.Find("Inventory/Diary");

            m_pages = new GameObject[3];
            m_pages[0] = FindUtility.Find("Inventory/Diary/Page1");
            m_pages[1] = FindUtility.Find("Inventory/Diary/Page2");
            m_pages[2] = FindUtility.Find("Inventory/Diary/Page3");

            var next = FindUtility.Find("Inventory/Diary/Next");
            var last = FindUtility.Find("Inventory/Diary/Last");
            var close = FindUtility.Find("Inventory/Diary/Close");
            var inputFiled = FindUtility.Find("Canvas/DiaryInput");

            next.AddComponent<Responder>().onMouseDown = () =>
            {
                if (m_pageIndex < 2)
                {
                    m_pageIndex++;
                    m_pages[m_pageIndex - 1].SetActive(false);
                    m_pages[m_pageIndex].SetActive(true);
                    if (m_pageIndex == 2)
                    {
                        next.SetActive(false);
                    }
                    else
                    {
                        next.SetActive(true);
                        last.SetActive(true);
                    }
                }
            };
            last.AddComponent<Responder>().onMouseDown = () =>
            {
                if (m_pageIndex > 0)
                {
                    m_pageIndex--;
                    m_pages[m_pageIndex + 1].SetActive(false);
                    m_pages[m_pageIndex].SetActive(true);
                    if (m_pageIndex == 0)
                    {
                        last.SetActive(false);
                    }
                    else
                    {
                        next.SetActive(true);
                        last.SetActive(true);
                    }
                }
            };
            close.AddComponent<Responder>().onMouseDown = () =>
            {
                Close();
            };

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 18; j++)
                {
                    int index = i * 18 + j;
                    m_pages[i].transform.GetChild(j).gameObject.AddComponent<Responder>().onMouseDown = () =>
                    {
                        m_currentEditWord = m_words[index];
                        inputFiled.SetActive(true);
                        inputFiled.GetComponentInChildren<InputField>().text = m_currentEditWord.GetTranslation();
                        inputFiled.GetComponentInChildren<InputField>().interactable = !m_currentEditWord.getAnswer;
                    };
                }
            }

            inputFiled.GetComponentInChildren<InputField>().onEndEdit.AddListener(str =>
            {
                if (m_currentEditWord != null)
                {
                    m_currentEditWord.customTranslation = str;
                    inputFiled.SetActive(false);
                }
            });
        }

        public void Show()
        {
            m_root.SetActive(true);
        }

        public void Close()
        {
            m_root.SetActive(false);
        }

        public void AddWord(string wordID)
        {
            var word = WordManager.Instance.GetWord(wordID);
            if (m_words.Contains(word))
                return;

            // 根据索引查找Sprite
            var go = m_pages[m_wordIndex / 18].transform.GetChild(m_wordIndex % 18).gameObject;
            go.SetActive(true);
            go.GetComponent<SpriteRenderer>().sprite = word.image;
            m_words.Add(word);

            m_wordIndex++;
        }

        public void AddWord(Word word)
        {
            if (m_words.Contains(word))
                return;

            // 根据索引查找Sprite
            var go = m_pages[m_wordIndex / 18].transform.GetChild(m_wordIndex % 18).gameObject;
            go.SetActive(true);
            go.GetComponent<SpriteRenderer>().sprite = word.image;
            m_words.Add(word);

            m_wordIndex++;
        }
    }
}
