using System;
using UnityEngine;

namespace Dao.InteractionSystem
{
    public class InteractiveItem : MonoBehaviour
    {
        public string itemName;

        public Action onMouseDown;
        public Action onMouseUp;
        public Action onMouseOver;

        private void OnMouseDown()
        {
            onMouseDown?.Invoke();
        }

        private void OnMouseUp()
        {
            onMouseUp?.Invoke();
        }

        private void OnMouseOver()
        {
            onMouseOver?.Invoke();
        }
    }
}
