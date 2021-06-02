using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PuzzleGame.UI
{
	public class InteractionDetector : MonoBehaviour, 
		IBeginDragHandler, 
		IDragHandler, 
		IEndDragHandler,
		IPointerDownHandler,
		IPointerUpHandler,
		IPointerClickHandler
	{
		public event Action<InteractionDetector> EventBeginDrag;
		public event Action<InteractionDetector> EventDrag;
		public event Action<InteractionDetector> EventEndDrag;
		public event Action<InteractionDetector> EventPointerDown;
		public event Action<InteractionDetector> EventPointerUp;
		public event Action<InteractionDetector> EventPointerClick;
		
		public void OnBeginDrag(PointerEventData eventData)
		{
			EventBeginDrag?.Invoke(this);
		}

		public void OnDrag(PointerEventData eventData)
		{
			EventDrag?.Invoke(this);
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			EventEndDrag?.Invoke(this);
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			EventPointerDown?.Invoke(this);
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			EventPointerUp?.Invoke(this);
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			EventPointerClick?.Invoke(this);
		}
	}
}
