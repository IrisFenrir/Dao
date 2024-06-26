﻿using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Dao.WordSystem
{
    public class CiphertextDialog : IDialog
    {
        private GameObject m_root;
        private Sentence m_sentence;

        public static int index = 0;

        public CiphertextDialog(Sentence sentence)
        {
            Next = new IDialog[1];
            m_root = FindUtility.Find("WorldCanvas/CiphertDialog");
            m_sentence = sentence;
        }

        public override void Close()
        {
            //m_root.SetActive(false);
            //FindUtility.Find("Translation", m_root.transform).SetActive(false);
            //var ciphertext = FindUtility.Find("Ciphertext", m_root.transform);
            //for (int i = 0; i < 6; i++)
            //{
            //    var child = ciphertext.transform.GetChild(i).gameObject;
            //    child.transform.GetChild(0).gameObject.SetActive(false);
            //    child.SetActive(false);
            //}
            //ciphertext.SetActive(false);
        }

        public static void Reset()
        {
            FindUtility.Find("WorldCanvas/CiphertDialog/Mask/Dialogs").GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            index = 0;
        }

        public static void SetPosition(Vector3 pos)
        {
            FindUtility.Find("WorldCanvas/CiphertDialog").transform.position = pos;
        }

        public override async Task Show()
        {
            m_root.SetActive(true);

            // 把句子包含的所有密文记录到字典
            m_sentence.RecordWords();

            var root = FindUtility.Find("Dialogs", m_root.transform).transform;

            // 设置句子内容
            var translation = FindUtility.Find("Translation", root.GetChild(index));
            if (m_sentence.CanShowTranslation())
            {
                translation.GetComponent<Text>().text = m_sentence.translation;
                translation.SetActive(true);
            }
            var ciphertext = FindUtility.Find("Ciphertext", root.GetChild(index)).transform;
            var mama = FindUtility.Find("Ma", root.GetChild(index)).GetComponent<RectTransform>();
            ciphertext.gameObject.SetActive(true);
            var count = m_sentence.WordCount();
            // fillAmount 0.26+0.105 MamaPosX 180.2+70 TranslationWidth 144.37+70
            ciphertext.GetComponent<Image>().fillAmount = 0.26f + (count - 1) * 0.105f;
            mama.anchoredPosition = new Vector2(180.2f + (count - 1) * 70, 48.7f);
            translation.GetComponent<RectTransform>().sizeDelta = new Vector2(144.37f + (count - 1) * 70, 47);
            for (int i = 0; i < count; i++)
            {
                var child = ciphertext.GetChild(i);
                if (!child.TryGetComponent<UIResponder>(out var responder))
                    responder = child.gameObject.AddComponent<UIResponder>();
                responder.onMouseClick = () =>
                {
                    UIDictionary.Instance.Show();
                };

                child.GetComponent<Image>().sprite = m_sentence.GetWordSprite(i);
                if (!m_sentence.CanShowTranslation())
                {
                    child.GetChild(0).GetComponent<Text>().text = m_sentence.GetWordTranslation(i);
                    child.GetChild(0).gameObject.SetActive(true);
                }
                child.gameObject.SetActive(true);
            }
            //Debug.Log("Move");
            // 向上移动
            var targetY = root.GetComponent<RectTransform>().anchoredPosition.y + 0.36f;
            while (targetY - root.GetComponent<RectTransform>().anchoredPosition.y > 0.001f)
            {
                root.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, Mathf.Lerp(root.GetComponent<RectTransform>().anchoredPosition.y, targetY, 0.01f));
                await Task.Yield();
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    break;
#endif
            }

            //Debug.Log("End");
            index++;
        }
    }
}
