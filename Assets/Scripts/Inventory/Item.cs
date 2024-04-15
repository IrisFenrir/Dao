using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class item : MonoBehaviour
{
    public ItemName itemName;
    public bool beUse;
    public int num;

    public void ItemClicked()
    {
        // add to bag
        InventoryManager.Instance.AddItem(itemName);
        // set inwentory state to unseeable in scenes
        this.gameObject.SetActive(false);
    }

    public void UseItem()
    {
        num -= 1;
        if (num == 0)
            beUse = false;
    }
}
