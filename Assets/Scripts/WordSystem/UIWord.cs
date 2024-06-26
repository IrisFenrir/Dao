﻿using UnityEngine;
using UnityEngine.UI;

namespace Dao.WordSystem
{
    public class UIWord
    {
        public Word word { get; private set; }

        private GameObject m_root;

        public UIWord(Word word, GameObject gameObject, GameObject text)
        {
            this.word = word;
            m_root = gameObject;
            //var responder = m_root.AddComponent<Responder>();
            if (!m_root.TryGetComponent<Responder>(out var responder))
                responder = m_root.AddComponent<Responder>();
            responder.onMouseEnter = () =>
            {
                // 显示文字
                text.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, -6);
                text.GetComponentInChildren<Text>().text = word.GetTranslation();
                text.SetActive(true);
            };
            responder.onMouseExit = () =>
            {
                // 隐藏文字
                text.SetActive(false);
            };
            responder.onMouseDown = () =>
            {
                UIDictionary.Instance.Show();
            };
        }
    }
}
