using System.Collections.Generic;
using UnityEngine;

namespace Dao.WordSystem
{
    public class UIDialogManager : Singleton<UIDialogManager>
    {
        public bool Enable { get; private set; }
        public IDialog Current { get; private set; }

        private GameObject m_root;
        private bool m_isPlaying;

        public UIDialogManager()
        {
            m_root = FindUtility.Find("Canvas/DialogPanel");
        }

        public void Show()
        {
            m_root.SetActive(true);
            Enable = true;
        }

        public void Close()
        {
            m_root.SetActive(false);
            Enable = false;
        }

        public async void StartDialog(IDialog dialog)
        {
            if (!Enable)
                Show();
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

        public void Update(float deltaTime)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Next();
            }
        }
    }
}
