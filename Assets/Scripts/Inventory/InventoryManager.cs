using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    public ItemDataList_SO itemData;

    [SerializeField]private List<ItemName> itemList = new List<ItemName>();
    public void AddItem(ItemName itemName)
    {
        // not in list
        if(!itemList.Contains(itemName))
        {
            itemList.Add(itemName);
            // UIœ‘ æ
            EventHandler.CallUpdateUIEvent(itemData.GetItemDetails(itemName), itemList.Count - 1);
        }
    }

    public void DeleteItem(ItemName itemName)
    {
        // not in list
        if (itemList.Contains(itemName))
        {
            itemList.Remove(itemName);
        }
    }

    public ItemDetails FindItem(ItemName itemName)
    {
        // not in list
        if (itemList.Contains(itemName))
        {
            return itemData.GetItemDetails(itemName);
        }
        else
            return null;
    }
}
