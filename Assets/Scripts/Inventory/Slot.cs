using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler
{
    public Item item;
    public GameObject currentTool;
    public GameObject physicalTool;
    private ItemController ic;

    public ItemController ItemController
    {
        get
        {
            if (ic == null)
            {
                ic = GetComponentInChildren<ItemController>();
            }
            return ic;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
        {
            return;
        }

        ItemController droppedIc = eventData.pointerDrag.GetComponent<ItemController>();
        if (droppedIc == null)
        {
            return;
        }

        InvItemDrag dragComponent = eventData.pointerDrag.GetComponent<InvItemDrag>();
        Transform sourceParent = dragComponent != null ? dragComponent.originalParent : droppedIc.transform.parent;
        Slot sourceSlot = sourceParent != null ? sourceParent.GetComponent<Slot>() : null;

        // Dropped on the same slot
        if (sourceSlot == this)
        {
            RectTransform draggedRect = droppedIc.GetComponent<RectTransform>();
            draggedRect.SetParent(transform);
            draggedRect.localPosition = Vector3.zero;
            return;
        }

        ItemController existingIc = ItemController;

        // Case 1: Slot already has an item
        if (existingIc != null && existingIc != droppedIc)
        {
            // Case 1A: Stack identical items
            if (existingIc.item != null && droppedIc.item != null && existingIc.item == droppedIc.item)
            {
                Debug.Log("Item Stacked + " + droppedIc.container.count);
                existingIc.AddItemStack(droppedIc);
                InventoryManager.Instance.RemoveContainer(droppedIc.container, false);
                Destroy(droppedIc.gameObject);
                existingIc.UpdateUI();

                if (sourceSlot != null)
                {
                    sourceSlot.ClearSlotData();
                    if (Hotbar.Instance != null && Hotbar.Instance.IsSlotSelected(sourceSlot))
                    {
                        Hotbar.Instance.RefreshSelectedSlot();
                    }
                }
                return;
            }

            // Case 1B: Different items - swap old item to source slot and place new item in this slot
            DestroyPhysicalTool();

            if (sourceSlot != null)
            {
                sourceSlot.DestroyPhysicalTool();

                RectTransform existingRect = existingIc.GetComponent<RectTransform>();
                existingRect.SetParent(sourceSlot.transform);
                existingRect.localPosition = Vector3.zero;
                sourceSlot.SetItemController(existingIc);
            }
            else
            {
                Slot emptySlot = InventoryManager.Instance != null ? InventoryManager.Instance.GetEmptySlot() : null;
                if (emptySlot != null)
                {
                    RectTransform existingRect = existingIc.GetComponent<RectTransform>();
                    existingRect.SetParent(emptySlot.transform);
                    existingRect.localPosition = Vector3.zero;
                    emptySlot.SetItemController(existingIc);
                }
                else
                {
                    existingIc.DropItem();
                }
            }

            RectTransform droppedRect = droppedIc.GetComponent<RectTransform>();
            droppedRect.SetParent(transform);
            droppedRect.localPosition = Vector3.zero;
            SetItemController(droppedIc);

            if (Hotbar.Instance != null)
            {
                if (sourceSlot != null && Hotbar.Instance.IsSlotSelected(sourceSlot))
                {
                    Hotbar.Instance.RefreshSelectedSlot();
                }
                if (Hotbar.Instance.IsSlotSelected(this))
                {
                    Hotbar.Instance.RefreshSelectedSlot();
                }
            }
            return;
        }

        // Case 2: Slot is empty
        DestroyPhysicalTool();

        RectTransform newRect = droppedIc.GetComponent<RectTransform>();
        newRect.SetParent(transform);
        newRect.localPosition = Vector3.zero;
        SetItemController(droppedIc);

        if (sourceSlot != null)
        {
            sourceSlot.ClearSlotData();
            if (Hotbar.Instance != null && Hotbar.Instance.IsSlotSelected(sourceSlot))
            {
                Hotbar.Instance.RefreshSelectedSlot();
            }
        }

        if (Hotbar.Instance != null && Hotbar.Instance.IsSlotSelected(this))
        {
            Hotbar.Instance.RefreshSelectedSlot();
        }
    }

    public void DestroyPhysicalTool()
    {
        if (physicalTool != null)
        {
            if (Hotbar.Instance != null && Hotbar.Instance.activeTool == physicalTool)
            {
                Hotbar.Instance.ClearActiveTool();
            }
            Destroy(physicalTool);
            physicalTool = null;
        }
    }

    public void ClearSlotData()
    {
        DestroyPhysicalTool();
        ic = null;
        item = null;
        currentTool = null;
    }

    public void Clear()
    {
        DestroyPhysicalTool();
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        ic = null;
        item = null;
        currentTool = null;
    }

    public void SetItemController(ItemController controller)
    {
        ic = controller;
        if (controller != null)
        {
            item = controller.item;
            currentTool = controller.gameObject;
        }
        else
        {
            item = null;
            currentTool = null;
        }
    }

    public void PlaceItem(RectTransform rt)
    {
        rt.SetParent(transform);
        rt.localPosition = Vector3.zero;
    }
}