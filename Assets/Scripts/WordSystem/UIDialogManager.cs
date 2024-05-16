using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dao.WordSystem
{
    public class UIDialogManager : Singleton<UIDialogManager>
    {
        public bool Enable { get; private set; }
        public IDialog Current { get; private set; }

        private GameObject m_root;
        private bool m_isPlaying;

        private GameObject m_blackBackground;

        public UIDialogManager()
        {
            m_root = FindUtility.Find("Canvas/DialogPanel");
            m_blackBackground = FindUtility.Find("DialogBackground");

            FindUtility.Find("WorldCanvas/SecretSentence/Ciphertext/Ma/Next").GetComponent<Button>().onClick.AddListener(() =>
            {
                Next();
            });
        }

        public void Show(bool showBlack = true)
        {
            m_root.SetActive(true);
            Enable = true;
            if (showBlack)
                m_blackBackground.SetActive(true);
        }

        public void Close()
        {
            m_root.SetActive(false);
            Enable = false;
            m_blackBackground.SetActive(false);
        }

        public async void StartDialog(IDialog dialog, bool showBlack = true)
        {
            if (!Enable)
                Show(showBlack);
            Current = dialog;
            Current.onStart?.Invoke();
            m_isPlaying = true;
            await Current.Show();
            m_isPlaying = false;
            Current.onStop?.Invoke();
        }

        public void Next()
        {
            if (!Enable || m_isPlaying) return;
            if (Current is NormalDialog || Current is CiphertextDialog)
            {
                if (Current.Next[0] is SelectDialog select)
                {
                    select.Show();
                }
                else
                {
                    Current.Close();
                    var next = Current.Next[0];
                    if (next == null)
                    {
                        Close();
                    }
                    else
                    {
                        StartDialog(next);
                    }
                }
            }
        }

        public void Next(SelectDialog select, int index)
        {
            Current.Close();
            select.Close();
            var next = select.Next[index];
            if (next == null)
            {
                Close();
            }
            else
            {
                StartDialog(next);
            }
        }

        public void Update(float deltaTime)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Next();
            }
        }
    }
}
