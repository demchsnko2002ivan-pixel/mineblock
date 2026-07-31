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
            if (item != null && ic != null)
            if (item == otherIc.item)
            {
                Debug.Log("Item Stacked");
                ic.AddItemStack(otherIc);
                InventoryManager.Instance.RemoveContainer(otherIc.container);
                Destroy(otherIc.gameObject);
                InventoryManager.Instance.ListItems();
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
    }
    public void SetItemController(ItemController controller)
    {
        ic = controller;
        item = controller.item;
    }
    public void PlaceItem(RectTransform rt)
    {
        rt.SetParent(transform);
        rt.localPosition = Vector3.zero;
    }
}