using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler
{
    public Item item;
    public GameObject currentTool;
    public GameObject physicalTool;
    private ItemController ic;

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            RectTransform draggedRect = eventData.pointerDrag.GetComponent<RectTransform>();
            draggedRect.SetParent(transform);
            draggedRect.localPosition = Vector3.zero;
            var droppedItem = draggedRect.GetComponent<ItemController>();

            var otherIc = droppedItem;
            if (item != null && ic != null && otherIc != null && ic != otherIc)
            if (item == otherIc.item)
            {
                Debug.Log("Item Stacked + " +otherIc.container.count);
                ic.AddItemStack(otherIc);
                InventoryManager.Instance.RemoveContainer(otherIc.container, false);
                Destroy(otherIc.gameObject);
                ic.UpdateUI();
                return;
            }

            currentTool = droppedItem.gameObject;
        }
    }
    public void Clear()
    {
        var objects = GetComponentsInChildren<Transform>();
        foreach (Transform obj in objects)
        {
            if (obj != transform)
            {
                Destroy(obj.gameObject);
            }
        }
        ic = null;
        item = null;
        currentTool = null;
    }
    public void SetItemController(ItemController controller)
    {
        ic = controller;
        item = controller.item;
        currentTool = controller.gameObject;
    }
    public void PlaceItem(RectTransform rt)
    {
        rt.SetParent(transform);
        rt.localPosition = Vector3.zero;
    }
}