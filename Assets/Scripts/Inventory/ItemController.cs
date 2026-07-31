using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemController : MonoBehaviour
{
    public Item item;
    public GameObject prefab;
    public int stack { get; set; } = 1;
    public ItemContainer container;
    public void RemoveItem()
    {
        Instantiate(prefab, InventoryManager.Instance.transform.position, Quaternion.identity);
        InventoryManager.Instance.RemoveContainer(container);
        Destroy(gameObject);
    }
    public void DropItem()
    {
        if (prefab == null)
        {
            Debug.Log("Missing prefab");
            return;
        }
        GameObject newOb = Instantiate(prefab, PlayerController.Instance.transform.position + PlayerController.Instance.transform.forward + Vector3.up, Quaternion.identity);
        InventoryManager.Instance.RemoveContainer(container);
        Quaternion q = newOb.transform.rotation;
        q.eulerAngles = new Vector3(45f, 0f, 45f);
        newOb.transform.rotation = q;
        Destroy(gameObject);
    }
    public void SetContainer(ItemContainer container)
    {
        this.container = container;
    }
    public void AddItem(Item newItem)
    {
        if (newItem != null)
        {
            item = newItem;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void AddItemStack(ItemController ic)
    {
        if (ic.container != null && container != null)
        {
            container.count += ic.container.count;
        }
        else
        {
            Debug.LogError("Container = null");
        }
    }
    public void Select(bool selected)
    {

    }
}
