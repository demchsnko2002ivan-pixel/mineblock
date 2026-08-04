using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [SerializeField]
    private List<ItemContainer> items = new List<ItemContainer>();

    [SerializeField]
    private Transform itemContent;

    [SerializeField]
    private GameObject itemPrefab;

    public ItemController[] inventoryItems;
    public Slot[] inventorySlots;
    public Dictionary<Item, int> itemDictionary = new Dictionary<Item, int>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        inventorySlots = itemContent.GetComponentsInChildren<Slot>();
    }

    public void Add(Item item)
    {
        // ALWAYS create a new container instance with a count of 1.
        // This keeps items separate so they can occupy their own slots for future dragging/stacking.
        items.Add(new ItemContainer { currentItem = item, count = 1 });

        // Dictionary still tracks the TOTAL global count of this item type
        if (itemDictionary.ContainsKey(item) == false)
        {
            itemDictionary.Add(item, 1);
        }
        else
        {
            itemDictionary[item]++;
        }

        ListItems();
        Debug.Log(item.itemName);
    }

    public int GetItemStack(Item item)
    {
        if (itemDictionary.ContainsKey(item))
        {
            return itemDictionary[item];
        }
        else
        {
            return 0;
        }
    }

    public void Remove(Item item)
    {
        // Find the first slot/container containing this item type and decrement it
        ItemContainer existingContainer = items.Find(c => c.currentItem == item);
        if (existingContainer != null)
        {
            existingContainer.count--;
            if (existingContainer.count <= 0)
            {
                items.Remove(existingContainer);
            }
        }

        item.stack--;

        if (itemDictionary.ContainsKey(item))
        {
            if (itemDictionary[item] > 1)
            {
                itemDictionary[item]--;
            }
            else
            {
                itemDictionary.Remove(item);
            }
        }
    }
    public void RemoveContainer(ItemContainer container, bool decrementDictionary = true)
    {
        if (decrementDictionary && container != null && container.currentItem != null)
        {
            if (itemDictionary.ContainsKey(container.currentItem))
            {
                if (itemDictionary[container.currentItem] > 1)
                {
                    itemDictionary[container.currentItem] -= container.count;
                }
                if (itemDictionary[container.currentItem] <= 0)
                {
                    itemDictionary.Remove(container.currentItem);
                }
            }
        }

        if (container != null)
        {
            items.Remove(container);
        }
    }

    public void RemoveItemIcon(Item item)
    {
        // Removes the first matching container instance found
        ItemContainer existingContainer = items.Find(c => c.currentItem == item);
        if (existingContainer != null)
        {
            items.Remove(existingContainer);
        }
    }

    public void ListItems()
    {
        foreach (Slot slot in inventorySlots)
        {
            slot.Clear();
        }

        foreach (var container in items)
        {
            Item item = container.currentItem;

            GameObject obj = Instantiate(itemPrefab, itemContent);
            var itemName = obj.transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
            var itemStack = obj.transform.Find("Stack").GetComponent<TextMeshProUGUI>();
            var itemIcon = obj.transform.Find("Icon").GetComponent<Image>();
            var itemController = obj.GetComponent<ItemController>();

            itemName.text = item.itemName;
            itemIcon.sprite = item.icon;

            // Read the stack count directly from this specific container
            itemController.stack = container.count;
            itemStack.text = "" + itemController.stack;

            itemController.prefab = item.prefab;
            itemController.item = item;
            itemController.AddItem(item);
            itemController.SetContainer(container);

            Debug.Log(item.itemName + " " + container.count);

            var slot = GetEmptySlot();
            if (slot != null)
            {
                slot.SetItemController(itemController);
                obj.transform.SetParent(slot.transform);
                obj.transform.localPosition = Vector3.zero;
            }
        }
        SetInventoryItems();
    }

    public Slot GetEmptySlot()
    {
        foreach (Slot slot in inventorySlots)
        {
            if (slot.transform.childCount == 0)
            {
                return slot;
            }
        }
        return null;
    }

    public void SetInventoryItems()
    {
        inventoryItems = itemContent.GetComponentsInChildren<ItemController>();
    }

    public void CraftItem(CraftRecipe recipe)
    {
        foreach (Ingredient ingredient in recipe.ingredients)
        {
            for (int i = 0; i < ingredient.amount; i++)
            {
                Remove(ingredient.item);
            }
        }
        Add(recipe.resultItem);
        ListItems();
    }

    public bool Contains(Item item, int stack)
    {
        if (itemDictionary.ContainsKey(item) && itemDictionary[item] >= stack)
        {
            return true;
        }
        return false;
    }
}

[System.Serializable]
public class ItemContainer
{
    // Changed to standard public fields so Unity can serialize and show them in the Inspector
    public Item currentItem;
    public int count { get; set; } = 1;
}