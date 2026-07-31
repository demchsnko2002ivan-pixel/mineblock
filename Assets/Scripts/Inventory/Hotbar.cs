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
    private void SelectItem(Slot slot)
    {
        if (slot.currentTool != null)
        {
            Debug.Log("SelectItem" + slot.currentTool.name);
            slot.currentTool.GetComponent<ItemController>().Select(true);
            DisableSelection(slot);
            Tool tool;
            if (slot.currentTool.GetComponent<ItemController>().item.prefab.transform.TryGetComponent<Tool>(out tool))
            {
                Transform refRight, refLeft;

                if (slot.physicalTool == null)
                {
                    activeTool = Instantiate(slot.currentTool.GetComponent<ItemController>().item.prefab, toolParent);
                    slot.physicalTool = activeTool;
                    activeTool.GetComponent<Rigidbody>().isKinematic = true;
                    activeTool.GetComponent<Collider>().isTrigger = true;
                    activeTool.transform.localPosition = tool.GetHandPosition();
                    activeTool.transform.localEulerAngles = tool.GetHandRotation();
                    tool = activeTool.GetComponent<Tool>();
                    Debug.Log("1" + activeTool);
                }
                else
                {
                    slot.physicalTool.SetActive(true);
                    tool = slot.physicalTool.GetComponent<Tool>();
                    Debug.Log("2" + tool);
                    activeTool.transform.localPosition = tool.GetHandPosition();
                    activeTool.transform.localEulerAngles = tool.GetHandRotation();
                }
                tool.GetReferences(out refLeft, out refRight);
                if (refRight != null)
                {
                    rightIK.data.target = refRight;
                }
                if (refLeft != null)
                {
                    leftIK.data.target = refLeft;
                }
                rigBuilder.Build();
            }
        }
        else
        {
            rightIK.data.target = null;
            leftIK.data.target = null;
            if (activeTool != null)
               activeTool.SetActive(false);
            rigBuilder.Build();
            DisableSelection(slot);
        }
    }
    public void AlignActiveTool()
    {
        activeTool.transform.localPosition = activeTool.transform.GetComponent<Tool>().GetHandPosition();
        activeTool.transform.localEulerAngles = activeTool.transform.GetComponent<Tool>().GetHandRotation();
    }
    private void DisableSelection(Slot activeSlot)
    {
        Slot[] slots = { slot1, slot2, slot3, slot4, slot5 };
        foreach (Slot slot in slots)
        {
            if (slot != activeSlot && slot.currentTool != null)
            {
                slot.currentTool.GetComponent<ItemController>().Select(false);
            }
        }

    }
}