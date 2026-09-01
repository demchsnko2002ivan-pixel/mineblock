using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Hotbar : MonoBehaviour
{
    public static Hotbar Instance;

    [SerializeField] Slot slot1;
    [SerializeField] Slot slot2;
    [SerializeField] Slot slot3;
    [SerializeField] Slot slot4;
    [SerializeField] Slot slot5;
    [SerializeField] GameObject selectObj;
    [SerializeField] TwoBoneIKConstraint rightIK;
    [SerializeField] TwoBoneIKConstraint leftIK;
    [SerializeField] RigBuilder rigBuilder;
    [SerializeField] Transform toolParent;
    public GameObject activeTool;
    public Slot selectedSlot;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectItem(slot1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectItem(slot2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SelectItem(slot3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SelectItem(slot4);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SelectItem(slot5);
        }
    }

    public void SelectItem(Slot slot)
    {
        selectedSlot = slot;

        if (slot != null && slot.currentTool != null)
        {
            Debug.Log("SelectItem " + slot.currentTool.name);
            ItemController itemController = slot.currentTool.GetComponent<ItemController>();
            if (itemController != null)
            {
                itemController.Select(true);
            }
            DisableSelection(slot);

            Tool tool = null;
            if (itemController != null && itemController.item != null && itemController.item.prefab != null && itemController.item.prefab.transform.TryGetComponent<Tool>(out tool))
            {
                Transform refRight, refLeft;

                if (activeTool != null && activeTool != slot.physicalTool)
                {
                    activeTool.SetActive(false);
                }

                if (slot.physicalTool == null)
                {
                    activeTool = Instantiate(itemController.item.prefab, toolParent);
                    slot.physicalTool = activeTool;
                    foreach (var rb in activeTool.GetComponentsInChildren<Rigidbody>())
                    {
                        rb.isKinematic = true;
                    }
                    foreach (var col in activeTool.GetComponentsInChildren<Collider>())
                    {
                        col.isTrigger = true;
                    }
                    activeTool.transform.localPosition = tool.GetHandPosition();
                    activeTool.transform.localEulerAngles = tool.GetHandRotation();
                    tool = activeTool.GetComponent<Tool>();
                }
                else
                {
                    activeTool = slot.physicalTool;
                    activeTool.SetActive(true);
                    tool = activeTool.GetComponent<Tool>();
                    activeTool.transform.localPosition = tool.GetHandPosition();
                    activeTool.transform.localEulerAngles = tool.GetHandRotation();
                }

                tool.GetReferences(out refLeft, out refRight);
                if (refRight != null && rightIK != null)
                {
                    rightIK.data.target = refRight;
                    rightIK.weight = 1f;
                }
                if (refLeft != null && leftIK != null)
                {
                    leftIK.data.target = refLeft;
                    leftIK.weight = 1f;
                }
            }
            else
            {
                ClearActiveTool();
            }
        }
        else
        {
            ClearActiveTool();
            DisableSelection(slot);
        }
    }

    public bool IsSlotSelected(Slot slot)
    {
        return selectedSlot != null && selectedSlot == slot;
    }

    public void RefreshSelectedSlot()
    {
        if (selectedSlot != null)
        {
            SelectItem(selectedSlot);
        }
    }

    public void ClearActiveTool()
    {
        if (rightIK != null)
        {
            rightIK.weight = 0f;
        }
        if (leftIK != null)
        {
            leftIK.weight = 0f;
        }
        if (activeTool != null)
        {
            activeTool.SetActive(false);
            activeTool = null;
        }
    }

    public void AlignActiveTool()
    {
        if (activeTool != null)
        {
            Tool tool = activeTool.GetComponent<Tool>();
            if (tool != null)
            {
                activeTool.transform.localPosition = tool.GetHandPosition();
                activeTool.transform.localEulerAngles = tool.GetHandRotation();
            }
        }
    }

    private void DisableSelection(Slot activeSlot)
    {
        Slot[] slots = { slot1, slot2, slot3, slot4, slot5 };
        foreach (Slot slot in slots)
        {
            if (slot != null && slot != activeSlot && slot.currentTool != null)
            {
                ItemController itemController = slot.currentTool.GetComponent<ItemController>();
                if (itemController != null)
                {
                    itemController.Select(false);
                }
            }
        }
    }
}