using UnityEngine;

namespace Dao.InventorySystem
{
    public abstract class Item
    {
        public string name;
        public GameObject gameObject;
        
        public virtual void Use() { }
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
    }
}
