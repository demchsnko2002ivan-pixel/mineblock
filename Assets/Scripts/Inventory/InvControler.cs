using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvControler : MonoBehaviour
{
    [SerializeField]
    private GameObject inventory;
    private bool _inventory;
    void Start()
    {
        _inventory = false;
        inventory.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (_inventory)
            {
                _inventory = false;
                inventory.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                PricelFollow.Instance.Disable();
                Cursor.visible = false;
                PlayerAttackHandler.Instance.EnableDisableAttacking(true);
                MouseLook.Instance.EnableRotate(true);
            }
            else
            {
                // InventoryManager.Instance.ListItems();
                _inventory = true;
                inventory.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                PricelFollow.Instance.Enable();
                Cursor.visible = false;
                PlayerAttackHandler.Instance.EnableDisableAttacking(false);
                MouseLook.Instance.EnableRotate(false);
            }
        }
    }
}
