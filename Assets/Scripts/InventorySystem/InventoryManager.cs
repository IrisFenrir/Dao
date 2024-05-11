using Dao.CameraSystem;
using System.Collections.Generic;
using UnityEngine;

namespace Dao.InventorySystem
{
    public class InventoryManager : Singleton<InventoryManager>
    {
        private Item[] m_items;
        private Queue<int> m_queue = new();
        private int m_index = 0;

        private GameObject m_root;

        public InventoryManager()
        {
            m_items = new Item[10];
            m_root = FindUtility.Find("Inventory");
        }

        public void Show()
        {
            m_root.SetActive(true);
        }

        public void Close()
        {
            m_root.SetActive(false);
        }

        public void AddItem(Item item)
        {
            int index;
            if (m_queue.Count == 0)
            {
                index = m_index++;
            }
            else
            {
                index = m_queue.Dequeue();
            }
            m_items[index] = item;
            var go = Object.Instantiate(item.gameObject).transform;
            go.SetParent(m_root.transform.GetChild(index));
            go.localPosition = Vector3.zero;
            go.gameObject.SetActive(true);
        }

        public void Remove(int index)
        {
            m_queue.Enqueue(index);
            m_items[index] = null;
            Object.Destroy(m_root.transform.GetChild(index).GetChild(0).gameObject);
        }

        public void Update(float deltaTime)
        {
            Rect screenRect = CameraController.Instance.GetScreenRect();
            m_root.transform.position = new Vector3(screenRect.x + (screenRect.width) / 2, 0, 0);
        }
    }
}
