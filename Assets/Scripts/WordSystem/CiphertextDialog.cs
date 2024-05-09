using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Dao.WordSystem
{
    public class CiphertextDialog : IDialog
    {
        private GameObject m_root;
        private Sentence m_sentence;

        public CiphertextDialog(Sentence sentence)
        {
            Next = new IDialog[1];
            m_root = FindUtility.Find("SecretSentence");
            m_sentence = sentence;
        }

        public override void Close()
        {
            m_root.SetActive(false);
            FindUtility.Find("Translation", m_root.transform).SetActive(false);
            var ciphertext = FindUtility.Find("Ciphertext", m_root.transform);
            for (int i = 0; i < ciphertext.transform.childCount; i++)
            {
                var child = ciphertext.transform.GetChild(i).gameObject;
                child.transform.GetChild(0).gameObject.SetActive(false);
                child.SetActive(false);
            }
            ciphertext.SetActive(false);
        }

        public override async Task Show()
        {
            m_root.SetActive(true);

            if (m_sentence.CanShowTranslation())
            {
                var translation = FindUtility.Find("Translation", m_root.transform);
                translation.GetComponent<Text>().text = m_sentence.translation;
                translation.SetActive(true);
            }
            
            var ciphertext = FindUtility.Find("Ciphertext", m_root.transform).transform;
            ciphertext.gameObject.SetActive(true);
            var count = m_sentence.WordCount();
            for (int i = 0; i < count; i++)
            {
                var child = ciphertext.GetChild(i);
                child.GetComponent<Image>().sprite = m_sentence.GetWordSprite(i);
                if (!m_sentence.CanShowTranslation())
                {
                    child.GetChild(0).GetComponent<Text>().text = m_sentence.GetWordTranslation(i);
                    child.GetChild(0).gameObject.SetActive(true);
                }
                child.gameObject.SetActive(true);
                await Task.Delay(200);
            }
        }
    }
}
