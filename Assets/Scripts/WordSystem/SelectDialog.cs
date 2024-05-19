using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Dao.WordSystem
{
    public class SelectDialog : IDialog
    {
        private GameObject m_root;
        private string[] m_options;
        private Action[] m_actions;
        private HashSet<int> m_disableOptions = new();

        public SelectDialog(string[] options)
        {
            Next = new IDialog[options.Length];
            m_root = FindUtility.Find("Canvas/DialogPanel/Options");
            m_options = options;
            m_actions = new Action[options.Length];
        }

        public void BindSelectAction(int index, Action action)
        {
            m_actions[index] = action;
        }

        public void DisableOption(int index)
        {
            m_disableOptions.Add(index);
        }

        public override void Close()
        {
            for (int i = 0; i < m_root.transform.childCount; i++)
            {
                m_root.transform.GetChild(i).gameObject.SetActive(false);
            }
            m_root.SetActive(false);
        }

        public override async Task Show()
        {
            m_root.SetActive(true);
            for (int i = 0; i < m_options.Length; i++)
            {
                var button = m_root.transform.GetChild(i).GetComponent<Button>();
                if (m_disableOptions.Contains(i))
                {
                    button.interactable = false;
                    button.GetComponent<Text>().text = m_options[i];
                    button.gameObject.SetActive(true);
                }
                else
                {
                    button.interactable = true;
                    // 绑定事件
                    button.onClick.RemoveAllListeners();
                    int index = i;
                    button.onClick.AddListener(() =>
                    {
                        m_actions[index]?.Invoke();
                        UIDialogManager.Instance.Next(this, index);
                    });
                    // 修改选项
                    button.GetComponentInChildren<Text>().text = m_options[i];
                    // 显示选项
                    button.gameObject.SetActive(true);
                }

            }
            await Task.Delay(100);
        }
    }
}
