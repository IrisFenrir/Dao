using Dao.Common;
using Dao.SceneSystem;
using Dao.WordSystem;
using UnityEngine;

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

        public override void Use(int index)
        {
            var teaUI = FindUtility.Find("Canvas/TeaUI");

            teaUI.SetActive(true);
            UIDictionary.Instance.Show(1);
        }
    }

    public class LivingRoomPaper : Item
    {
        public LivingRoomPaper()
        {
            name = "LivingRoomPaper";
            gameObject = FindUtility.Find("Inventory/LivingRoomPaper");
        }

        public override void Use(int index)
        {
            GameUtility.ShowItem("LivingRoomPaper", 
                "Bird", "Like", "Wind", "Wind", "Front", "Water",
                "Plant", "Like", "Dirt", "Dirt", "Front", "Water",
                "Father", "Mather", "Look", "Kid", "Kid", "Want", "Sugar",
                "Father", "Mather", "Deny", "Look",
                "Fear", "Front", "Want", "Back");
        }
    }

    public class BedroomPaper : Item
    {
        public BedroomPaper()
        {
            name = "BedroomPaper";
            gameObject = FindUtility.Find("Inventory/BedroomPaper");
        }

        public override void Use(int index)
        {
            GameUtility.ShowItem("BedroomPaper",
                "Make", "Food",
                "Father", "Put", "Knife",
                "Mather", "Put", "Fire",
                "Kid", "Put", "Container",
                "I", "I", "Beautiful", "Source");
        }
    }

    public class Piece1 : Item
    {
        public Piece1()
        {
            name = "Piece1";
            gameObject = FindUtility.Find("Inventory/Piece1");
        }

        public override void Use(int index)
        {
            var note = FindUtility.Find("Environments/Bedroom/Scene/Note");

            if (note.activeInHierarchy)
            {
                SceneManager.Instance.GetScene<Bedroom>("Bedroom").MoveNotePiece1();
                InventoryManager.Instance.Remove(index);
            }
            else
            {
                GameUtility.ShowItem("Piece1", "Mather", "Like", "I", "Mather", "Hurt");
            }
        }
    }

    public class Piece2 : Item
    {
        public Piece2()
        {
            name = "Piece2";
            gameObject = FindUtility.Find("Inventory/Piece2");
        }

        public override void Use(int index)
        {
            var note = FindUtility.Find("Environments/Bedroom/Scene/Note");
            if (note.activeInHierarchy)
            {
                SceneManager.Instance.GetScene<Bedroom>("Bedroom").MoveNotePiece2();
                InventoryManager.Instance.Remove(index);
            }
            else
                GameUtility.ShowItem("Piece2", "Water", "Back", "Mather", "I", "Look", "Die", "Front", "Mather");
        }
    }

    public class Piece3 : Item
    {
        public Piece3()
        {
            name = "Piece3";
            gameObject = FindUtility.Find("Inventory/Piece3");
        }

        public override void Use(int index)
        {
            var note = FindUtility.Find("Environments/Bedroom/Scene/Note");
            if (note.activeInHierarchy)
            {
                SceneManager.Instance.GetScene<Bedroom>("Bedroom").MoveNotePiece3();
                InventoryManager.Instance.Remove(index);
            }
            else
                GameUtility.ShowItem("Piece3", "I", "Fear", "I", "Fear", "I", "Fear", "Deny", "Kid", "Deny");
        }
    }

    public class Piece4 : Item
    {
        public Piece4()
        {
            name = "Piece4";
            gameObject = FindUtility.Find("Inventory/Piece4");
        }

        public override void Use(int index)
        {
            var note = FindUtility.Find("Environments/Bedroom/Scene/Note");
            if (note.activeInHierarchy)
            {
                SceneManager.Instance.GetScene<Bedroom>("Bedroom").MoveNotePiece4();
                InventoryManager.Instance.Remove(index);
            }
            else
            {
                GameUtility.ShowItem("Piece4", "Die", "Fear", "Mather", "Die", "Mather", "Like", "Mather");
            }
        }
    }

    public class Menu : Item
    {
        public Menu()
        {
            name = "Menu";
            gameObject = FindUtility.Find("Inventory/Menu");
        }

        public override void Use(int index)
        {
            GameUtility.ShowItem("Menu", 
                "Food", "Make",
                "Open", "Fire", "Source",
                "Big", "Red", "Food",
                "Front", "Water",
                "Big", "Green", "Food",
                "Front", "Salt",
                "Small", "Blue", "Food",
                "Close", "Fire", "Source");
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
            else if (current == "Bedroom")
            {
                FindUtility.Find("Environments/Bedroom/Scene/Background/MouseHandle/Key").SetActive(true);
                InventoryManager.Instance.Remove(index);
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
