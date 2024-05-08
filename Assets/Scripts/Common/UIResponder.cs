using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dao
{
    public class UIResponder : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public Action onMouseClick;
        public Action onMouseEnter;
        public Action onMouseExit;

        public void OnPointerClick(PointerEventData eventData)
        {
            onMouseClick?.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            onMouseEnter?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onMouseExit?.Invoke();
        }
    }
}
