using System;
using UnityEngine;

public class InteractiveItem : MonoBehaviour
{
    public string itemName;

    public Action MouseEnterAction { get; set; }
    public Action MouseExitAction { get; set; }
    public Action MouseDownAction { get; set; }

    private void OnMouseEnter()
    {
        
    }

    private void OnMouseExit()
    {
        
    }


    private void OnMouseDown()
    {
        MouseDownAction?.Invoke();
    }
}
