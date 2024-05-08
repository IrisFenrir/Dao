using System;
using UnityEngine;

namespace Dao
{
    public class Responder : MonoBehaviour
    {
        public bool enable = true;

        public Action onMouseDown;
        public Action onMouseUp;
        public Action onMouseOver;
        public Action onMouseEnter;
        public Action onMouseExit;

        private void OnMouseDown()
        {
            if (!enable) return;
            onMouseDown?.Invoke();
        }

        private void OnMouseUp()
        {
            if (!enable) return;
            onMouseUp?.Invoke();
        }

        private void OnMouseOver()
        {
            if (!enable) return;
            onMouseOver?.Invoke();
        }

        private void OnMouseEnter()
        {
            if (!enable) return;
            onMouseEnter?.Invoke();
        }

        private void OnMouseExit()
        {
            if (!enable) return;
            onMouseExit?.Invoke();
        }
    }
}
