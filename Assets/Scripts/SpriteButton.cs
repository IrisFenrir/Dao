using System;
using UnityEngine;

public class SpriteButton : MonoBehaviour
{
    public Action OnClick { get; set; }
    public Action OnUp { get; set; }
    public bool IsPressing { get; set; }

    public bool pressed;
    public bool released;

    private void OnMouseDown()
    {
        OnClick?.Invoke();
        IsPressing = true;
        pressed = true;
    }

    private void OnMouseUp()
    {
        OnUp?.Invoke();
        IsPressing = false;
        released = true;
    }

    private void LateUpdate()
    {
        pressed = false;
        released = false;
    }
}
