using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class InvItemDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private ScrollRect parentScrollRect;
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        canvas = GetComponentInParent<Canvas>();
        parentScrollRect = GetComponentInParent<ScrollRect>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
        originalParent = transform.parent;
        transform.SetParent(canvas.transform);
        if (parentScrollRect != null)
        {
            parentScrollRect.enabled = false;
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        if (parentScrollRect != null)
        {
            parentScrollRect.enabled = true;
        }
        ResetPosition();
        if (transform.parent == canvas.transform)
        {
            transform.SetParent(originalParent);
            rectTransform.anchoredPosition = Vector2.zero;
        }
        var objectList = eventData.hovered;
        foreach (GameObject obj in objectList)
        {
            Debug.Log(obj.name);
        }
        //Debug.Log("Object Count " + objectList.Count);
        if (objectList.Count == 0)
        {
            GetComponent<ItemController>().DropItem();
        }
    }
    private void ResetPosition()
    {
        rectTransform.localPosition = Vector3.zero;
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
    }
}
