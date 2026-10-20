using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField]
    private Item item;
    private Rigidbody rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        if (rb.linearVelocity.magnitude == 0)
        {
            rb.isKinematic = true;
        }
    }
    public void Pickup()
    {
        Debug.Log("Item pickup");
        InventoryManager.Instance.Add(item);
        Destroy(gameObject);
    }
    private void OnMouseDown()
    {
       // Debug.Log("Pick Item");
       // Pickup();
    }
}
