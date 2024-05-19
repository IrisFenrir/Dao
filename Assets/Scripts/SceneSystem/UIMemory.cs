using Dao.WordSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Dao.SceneSystem
{
    public class UIMemory : Singleton<UIMemory>
    {
        public bool Enable { get; private set; }

        private GameObject m_root;

        private string m_currentScene;
        private Vector3 m_playerPos;
        private Vector3 m_playerScale;

        public UIMemory()
        {
            m_root = FindUtility.Find("Canvas/Memory");

            m_root.transform.Find("Close").GetComponent<Button>().onClick.AddListener(CloseMemory);
            var panel = m_root.transform.Find("Panel");
            FindUtility.Find("Close", panel).GetComponent<Button>().onClick.AddListener(Close);
            FindUtility.Find("EntryRoom_Mather", panel).GetComponent<Button>().onClick.AddListener(Memory_EntryRoom_Mather);
        }

        private void CloseMemory()
        {
            CiphertextDialog.Reset();
            // 打开UI
            m_root.transform.Find("Panel").gameObject.SetActive(true);
            // 关闭 关闭按钮
            m_root.transform.Find("Close").gameObject.SetActive(false);
            // 设置主角位置
            var player = FindUtility.Find("Player").transform;
            player.position = m_playerPos;
            player.localScale = m_playerScale;
            player.gameObject.SetActive(true);
            // 加载场景
            SceneManager.Instance.LoadScene(m_currentScene);
            UIDialogManager.Instance.MemoryMode = false;
        }

        private void Memory_EntryRoom_Mather()
        {
            // 记录当前信息
            var player = FindUtility.Find("Player").transform;
            m_currentScene = SceneManager.Instance.Current.name;
            m_playerPos = player.position;
            m_playerScale = player.localScale;

            // 加载引导场景
            var entryRoom = FindUtility.Find("Environments/EntryRoom");
            GameObject background = FindUtility.Find("Background", entryRoom.transform);
            GameObject openDoor = FindUtility.Find("OpenDoor", entryRoom.transform);
            openDoor.SetActive(false);
            background.SetActive(true);
            SceneManager.Instance.GetScene<EntryRoom>("EntryRoom").MemoryMode = true;
            SceneManager.Instance.LoadScene("EntryRoom");
            // 设置妈妈和主角位置
            var mather = FindUtility.Find("MatherModel");
            player.gameObject.SetActive(false);
            mather.SetActive(true);
            mather.transform.position = new Vector3(3.6f, -0.41f, 0);
            // 关闭交互
            var responders = FindUtility.Find("Environments/EntryRoom/Scene/Background/Responders");
            responders.SetActive(false);
            // 开启对话
            UIDialogManager.Instance.MemoryMode = true;
            var dialog1 = DialogUtility.GetDialog("EntryRoom-Mather-First");
            var pos = FindUtility.Find("MatherModel/DialogPos1").transform.position;
            FindUtility.Find("WorldCanvas/CiphertDialog").transform.position = pos;
            UIDialogManager.Instance.StartDialog(dialog1, false);
            // 关闭UI
            m_root.transform.Find("Panel").gameObject.SetActive(false);
            // 显示关闭按钮
            m_root.transform.Find("Close").gameObject.SetActive(true);
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
    }
}
