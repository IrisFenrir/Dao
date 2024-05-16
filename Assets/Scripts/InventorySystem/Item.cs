using Dao.SceneSystem;
using Dao.WordSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Dao.InventorySystem
{
    public abstract class Item
    {
        public string name;
        public GameObject gameObject;
        
        public virtual void Use(int index) { }
    }

    public class Tea : Item
    {
        public Tea()
        {
            name = "Tea";
            gameObject = FindUtility.Find("Inventory/Tea");
        }
    }

    public class LivingRoomPaper : Item
    {
        public LivingRoomPaper()
        {
            name = "LivingRoomPaper";
            gameObject = FindUtility.Find("Inventory/LivingRoomPaper");
        }
    }

    public class Piece1 : Item
    {
        public Piece1()
        {
            name = "Piece1";
            gameObject = FindUtility.Find("Inventory/Piece1");
        }
    }

    public class Piece2 : Item
    {
        public Piece2()
        {
            name = "Piece2";
            gameObject = FindUtility.Find("Inventory/Piece2");
        }
    }

    public class Piece3 : Item
    {
        public Piece3()
        {
            name = "Piece3";
            gameObject = FindUtility.Find("Inventory/Piece3");
        }
    }

    public class Piece4 : Item
    {
        public Piece4()
        {
            name = "Piece4";
            gameObject = FindUtility.Find("Inventory/Piece4");
        }
    }

    public class Menu : Item
    {
        public Menu()
        {
            name = "Menu";
            gameObject = FindUtility.Find("Inventory/Menu");
        }
    }

    public class Key : Item
    {
        public Key()
        {
            name = "Key";
            gameObject = FindUtility.Find("Inventory/Key");
        }

        public override void Use(int index)
        {
            var current = SceneManager.Instance.Current.name;
            if (current == "EntryRoom")
            {
                if (FindUtility.Find("Environments/EntryRoom/Scene/NearDoor").activeInHierarchy)
                {
                    FindUtility.Find("Environments/EntryRoom/Scene/NearDoor/MouseHandle/Key").SetActive(true);
                    FindUtility.Find("Environments/EntryRoom/Scene/NearDoor/CloseButton").GetComponent<Responder>().enable = false;
                    InventoryManager.Instance.Remove(index);
                }
            }
        }
    }

    public class Diary : Item
    {
        private GameObject m_open;
        private GameObject m_close;

        public Diary()
        {
            name = "Dairy";

            m_open = FindUtility.Find("Inventory/Item0/Open");
            m_close = FindUtility.Find("Inventory/Item0/Close");
        }

        public override void Use(int index)
        {
            if (UIDictionary.Instance.Enable)
            {
                m_open.SetActive(false);
                m_close.SetActive(true);
                UIDictionary.Instance.Close();
            }
            else
            {
                m_open.SetActive(true);
                m_close.SetActive(false);
                UIDictionary.Instance.Show();
            }
        }
    }
}
