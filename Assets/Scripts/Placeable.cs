using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Placeable : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private float maxPlaceDistance = 15f;
    [SerializeField] private float defaultDistance = 3.5f;
    [SerializeField] private LayerMask groundLayer = ~0;
    [SerializeField] private Vector3 positionOffset = Vector3.zero;
    [SerializeField] private bool alignWithSurfaceNormal = false;
    [SerializeField] private bool rotateWithPlayer = true;

    [Header("Placement Settings")]
    [SerializeField] private bool placeOnClick = true;
    [SerializeField] private bool allowManualRotation = true;
    [SerializeField] private KeyCode manualRotateKey = KeyCode.R;
    [SerializeField] private float rotationStepDegrees = 45f;

    public bool IsPlacing => isPlacing;

    private bool isPlacing = false;
    private float currentYRotationOffset = 0f;
    private bool hasValidPlacement = false;

    private Collider[] cachedColliders;
    private Rigidbody[] cachedRigidbodies;
    private ItemPickup cachedItemPickup;

    void Awake()
    {
        CacheComponents();
    }

    void Update()
    {
        if (!isPlacing)
        {
            return;
        }

        HandleManualRotation();
        UpdatePosition();
        HandlePlacementInput();
    }

    private void OnDisable()
    {
        if (isPlacing)
        {
            SetPlacingMode(false);
        }
    }

    private void CacheComponents()
    {
        if (cachedColliders == null || cachedColliders.Length == 0)
        {
            cachedColliders = GetComponentsInChildren<Collider>(true);
        }
        if (cachedRigidbodies == null || cachedRigidbodies.Length == 0)
        {
            cachedRigidbodies = GetComponentsInChildren<Rigidbody>(true);
        }
        if (cachedItemPickup == null)
        {
            cachedItemPickup = GetComponent<ItemPickup>();
        }
    }

    public void SetPlacingMode(bool active)
    {
        isPlacing = active;
        CacheComponents();

        if (active)
        {
            gameObject.SetActive(true);

            // Disable colliders on the preview to prevent self-collision during raycasts
            if (cachedColliders != null)
            {
                foreach (Collider col in cachedColliders)
                {
                    if (col != null)
                    {
                        col.enabled = false;
                    }
                }
            }

            // Set rigidbodies to kinematic so preview does not fall or trigger physics
            if (cachedRigidbodies != null)
            {
                foreach (Rigidbody rb in cachedRigidbodies)
                {
                    if (rb != null)
                    {
                        rb.isKinematic = true;
                        rb.useGravity = false;
                    }
                }
            }

            // Disable ItemPickup so interaction doesn't pick it up during preview
            if (cachedItemPickup != null)
            {
                cachedItemPickup.enabled = false;
            }

            // Disable InteractiveObject components during placement preview
            InteractiveObject[] previewInteractiveObjects = GetComponentsInChildren<InteractiveObject>(true);
            if (previewInteractiveObjects != null)
            {
                foreach (InteractiveObject interactiveObj in previewInteractiveObjects)
                {
                    if (interactiveObj != null)
                    {
                        interactiveObj.enabled = false;
                    }
                }
            }

            // Disable player attack so clicking doesn't trigger melee animations
            if (PlayerAttackHandler.Instance != null)
            {
                PlayerAttackHandler.Instance.EnableDisableAttacking(false);
            }
            if (PlayerController.Instance != null)
            {
                PlayerController.Instance.EnableDisableAttacking(false);
            }

            UpdatePosition();
        }
        else
        {
            // Restore player attack when placing mode ends
            if (PlayerAttackHandler.Instance != null)
            {
                PlayerAttackHandler.Instance.EnableDisableAttacking(true);
            }
            if (PlayerController.Instance != null)
            {
                PlayerController.Instance.EnableDisableAttacking(true);
            }
        }
    }

    private Camera GetCamera()
    {
        if (Camera.main != null)
        {
            return Camera.main;
        }
        if (Camera.current != null)
        {
            return Camera.current;
        }
        return FindFirstObjectByType<Camera>();
    }

    private Transform GetViewer()
    {
        Camera cam = GetCamera();
        if (cam != null)
        {
            return cam.transform;
        }
        if (PlayerController.Instance != null)
        {
            return PlayerController.Instance.transform;
        }
        return null;
    }

    private int GetEffectiveLayerMask()
    {
        int mask = groundLayer.value;

        // Ensure terrain (Default layer 0) and Ground (layer 3) are included
        int defaultBit = 1 << LayerMask.NameToLayer("Default");
        int groundBit = 1 << LayerMask.NameToLayer("Ground");
        if (mask == 0 || mask == groundBit)
        {
            mask |= defaultBit | groundBit;
        }

        // Always exclude Ignore Raycast, UI, and TransparentFX
        mask &= ~(1 << 2);
        mask &= ~(1 << 5);
        mask &= ~(1 << 1);

        int ragdollLayer = LayerMask.NameToLayer("Ragdoll");
        if (ragdollLayer != -1)
        {
            mask &= ~(1 << ragdollLayer);
        }

        return mask;
    }

    private bool IsOwnOrPlayerCollider(Collider col, Transform viewer)
    {
        if (col == null)
        {
            return true;
        }
        if (col.transform.IsChildOf(transform))
        {
            return true;
        }
        if (viewer != null && col.transform.root == viewer.root)
        {
            return true;
        }
        if (PlayerController.Instance != null && col.transform.root == PlayerController.Instance.transform.root)
        {
            return true;
        }
        return false;
    }

    public void UpdatePosition()
    {
        Transform viewer = GetViewer();
        if (viewer == null)
        {
            return;
        }

        Camera cam = GetCamera();
        Ray ray;
        if (cam != null && Cursor.lockState != CursorLockMode.Locked)
        {
            ray = cam.ScreenPointToRay(Input.mousePosition);
        }
        else
        {
            ray = new Ray(viewer.position, viewer.forward);
        }

        int mask = GetEffectiveLayerMask();
        RaycastHit hit;
        hasValidPlacement = false;

        // 1. Primary raycast along player/camera view direction
        if (Physics.Raycast(ray, out hit, maxPlaceDistance, mask, QueryTriggerInteraction.Ignore))
        {
            if (!IsOwnOrPlayerCollider(hit.collider, viewer))
            {
                hasValidPlacement = true;
            }
        }

        // 2. Fallback: Raycast downward from in front of the viewer to find ground
        if (!hasValidPlacement)
        {
            Vector3 forwardPoint = viewer.position + viewer.forward * defaultDistance;
            if (Physics.Raycast(forwardPoint + Vector3.up * 5f, Vector3.down, out hit, 20f, mask, QueryTriggerInteraction.Ignore))
            {
                if (!IsOwnOrPlayerCollider(hit.collider, viewer))
                {
                    hasValidPlacement = true;
                }
            }
        }

        if (hasValidPlacement)
        {
            transform.position = hit.point + positionOffset;

            if (alignWithSurfaceNormal)
            {
                transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal) * Quaternion.Euler(0f, currentYRotationOffset, 0f);
            }
            else if (rotateWithPlayer)
            {
                Vector3 lookDirection = viewer.forward;
                lookDirection.y = 0f;
                if (lookDirection.sqrMagnitude > 0.001f)
                {
                    transform.rotation = Quaternion.LookRotation(lookDirection) * Quaternion.Euler(0f, currentYRotationOffset, 0f);
                }
            }
            else if (allowManualRotation)
            {
                transform.rotation = Quaternion.Euler(0f, currentYRotationOffset, 0f);
            }
        }
        else
        {
            // 3. Fallback: keep preview directly in front of the viewer so it is always visible
            Vector3 forwardPoint = viewer.position + viewer.forward * defaultDistance;
            forwardPoint.y = viewer.position.y;
            transform.position = forwardPoint + positionOffset;

            if (rotateWithPlayer)
            {
                Vector3 lookDirection = viewer.forward;
                lookDirection.y = 0f;
                if (lookDirection.sqrMagnitude > 0.001f)
                {
                    transform.rotation = Quaternion.LookRotation(lookDirection) * Quaternion.Euler(0f, currentYRotationOffset, 0f);
                }
            }
        }
    }

    private void HandleManualRotation()
    {
        if (allowManualRotation && manualRotateKey != KeyCode.None && Input.GetKeyDown(manualRotateKey))
        {
            currentYRotationOffset = (currentYRotationOffset + rotationStepDegrees) % 360f;
        }
    }

    private void HandlePlacementInput()
    {
        if (!placeOnClick || !hasValidPlacement)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            Place();
        }
    }

    public void Place()
    {
        if (!isPlacing)
        {
            return;
        }

        isPlacing = false;

        // Restore colliders to normal active state
        if (cachedColliders != null)
        {
            foreach (Collider col in cachedColliders)
            {
                if (col != null)
                {
                    if (col.name != "TriggerBox")
                    {
                        col.enabled = true;
                        col.isTrigger = false;
                    }
                }
            }
        }

        // Maintain kinematic state for placed structures so they stay firmly grounded
        if (cachedRigidbodies != null)
        {
            foreach (Rigidbody rb in cachedRigidbodies)
            {
                if (rb != null)
                {
                    rb.isKinematic = true;
                    rb.useGravity = false;
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
            }
        }

        // Remove ItemPickup from the placed structure in world
        ItemPickup[] pickups = GetComponentsInChildren<ItemPickup>(true);
        if (pickups != null && pickups.Length > 0)
        {
            foreach (ItemPickup pickup in pickups)
            {
                if (pickup != null)
                {
                    Destroy(pickup);
                }
            }
        }
        else if (cachedItemPickup != null)
        {
            Destroy(cachedItemPickup);
        }

        // Activate InteractiveObject component if present (e.g. Campfire)
        InteractiveObject[] interactiveObjects = GetComponentsInChildren<InteractiveObject>(true);
        if (interactiveObjects != null && interactiveObjects.Length > 0)
        {
            foreach (InteractiveObject interactiveObj in interactiveObjects)
            {
                if (interactiveObj != null)
                {
                    interactiveObj.enabled = true;
                    interactiveObj.Activate();
                }
            }
        }

        // Update Hotbar, consume 1 item from inventory, and remove from hands
        if (Hotbar.Instance != null)
        {
            if (Hotbar.Instance.activeTool == gameObject)
            {
                Hotbar.Instance.activeTool = null;
            }

            Slot slot = Hotbar.Instance.selectedSlot;
            if (slot != null)
            {
                if (slot.physicalTool == gameObject)
                {
                    slot.physicalTool = null;
                }

                if (slot.currentTool != null)
                {
                    ItemController ic = slot.currentTool.GetComponent<ItemController>();
                    if (ic != null && ic.container != null)
                    {
                        ic.container.count--;
                        ic.UpdateUI();
                        if (ic.container.count <= 0)
                        {
                            InventoryManager.Instance.RemoveContainer(ic.container, false);
                            Destroy(ic.gameObject);
                            slot.ClearSlotData();
                        }
                    }
                }
            }

            // Remove item from hands and deselect hotbar slot
            Hotbar.Instance.SelectItem(null);
        }

        // Restore player interaction/attack on the next frame to prevent accidental attack swing on place click
        MonoBehaviour coroutineRunner = (MonoBehaviour)PlayerController.Instance ?? Hotbar.Instance;
        if (coroutineRunner != null)
        {
            coroutineRunner.StartCoroutine(RestoreInteractionRoutine());
        }
        else
        {
            RestoreInteraction();
        }

        enabled = false;
    }

    private static IEnumerator RestoreInteractionRoutine()
    {
        yield return null;
        RestoreInteraction();
    }

    private static void RestoreInteraction()
    {
        if (PlayerAttackHandler.Instance != null)
        {
            PlayerAttackHandler.Instance.EnableDisableAttacking(true);
        }
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.EnableDisableAttacking(true);
        }
    }
}
