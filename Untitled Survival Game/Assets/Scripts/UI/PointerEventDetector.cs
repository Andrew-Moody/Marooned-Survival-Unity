using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// A general component for recieving EventSystem pointer callbacks
/// </summary>
public class PointerEventDetector : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler,
	IPointerEnterHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
	public event System.Action<PointerEventData> OnPointerDownEvent;
	public event System.Action<PointerEventData> OnPointerUpEvent;
	public event System.Action<PointerEventData> OnPointerClickEvent;
	public event System.Action<PointerEventData> OnBeginDragEvent;
	public event System.Action<PointerEventData> OnEndDragEvent;
	public event System.Action<PointerEventData> OnDragEvent;
	public event System.Action<PointerEventData> OnPointerEnterEvent;
	public event System.Action<PointerEventData> OnPointerExitEvent;


	private void Awake()
	{
		if (!TryGetComponent(out Graphic graphic))
		{
			Debug.LogError($"PointerEventDetector {gameObject.name} must have a component derived from Graphic to detect pointer events." +
				$"Typically an Image with alpha set to zero if a visual element is not needed");
		}
	}


	public void OnPointerDown(PointerEventData eventData)
	{
		OnPointerDownEvent?.Invoke(eventData);
	}


	public void OnPointerUp(PointerEventData eventData)
	{
		OnPointerUpEvent?.Invoke(eventData);
	}


	public void OnPointerClick(PointerEventData eventData)
	{
		OnPointerClickEvent?.Invoke(eventData);
	}


	public void OnPointerEnter(PointerEventData eventData)
	{
		OnPointerEnterEvent?.Invoke(eventData);
	}


	public void OnPointerExit(PointerEventData eventData)
	{
		OnPointerExitEvent?.Invoke(eventData);
	}



	public void OnBeginDrag(PointerEventData eventData)
	{
		OnBeginDragEvent?.Invoke(eventData);
	}


	public void OnEndDrag(PointerEventData eventData)
	{
		OnEndDragEvent?.Invoke(eventData);
	}


	public void OnDrag(PointerEventData eventData)
	{
		OnDragEvent?.Invoke(eventData);
	}
}
