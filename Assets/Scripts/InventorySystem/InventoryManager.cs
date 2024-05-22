using Dao.CameraSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dao.InventorySystem
{
    public class InventoryManager : Singleton<InventoryManager>
    {
        private Item[] m_items;
        private Queue<int> m_queue = new();
        private int m_index = 1;

        private GameObject m_root;

        public InventoryManager()
        {
            m_items = new Item[11];
            m_root = FindUtility.Find("Inventory");
            m_items[0] = new Diary();

            for (int i = 0; i < 11; i++)
            {
                int index = i;
                m_root.transform.GetChild(index).gameObject.AddComponent<Responder>().onMouseDown = () =>
                {
                    m_items[index]?.Use(index);
                };
            }
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
            Debug.Log(index);
            m_items[index] = item;
            var go = GameObject.Instantiate(item.gameObject).transform;
            go.SetParent(m_root.transform.GetChild(index));
            go.localPosition = Vector3.zero;
            go.gameObject.SetActive(true);
        }

        public void Remove(int index)
        {
            m_queue.Enqueue(index);
            m_items[index] = null;
            GameObject.Destroy(m_root.transform.GetChild(index).GetChild(0).gameObject);
        }

        public bool Contains<T>() where T: Item
        {
            return Array.Find(m_items, i => i != null && i.GetType() == typeof(T)) != null;
        }

        public void Update(float deltaTime)
        {
            Rect screenRect = CameraController.Instance.GetScreenRect();
            m_root.transform.position = new Vector3(screenRect.x + (screenRect.width) / 2, 0, 0);
        }

        public void DisableAllItems()
        {
            for (int i = 0; i < 11; i++)
            {
                m_root.transform.GetChild(i).gameObject.GetComponent<Responder>().enable = false;
            }
        }

        public void EnableAllItems()
        {
            for (int i = 0; i < 11; i++)
            {
                m_root.transform.GetChild(i).gameObject.GetComponent<Responder>().enable = true;
            }
        }
    }
}
