using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.AdaptivePerformance;
using UnityEngine.Animations.Rigging;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerAttackHandler : MonoBehaviour
{
    public static PlayerAttackHandler Instance;

    private RigBuilder _rigBuilder;
    public bool isAttacking = false;
    private bool canAttack = true;
    private Animator _animator;
    public static Action OnAttack;
    [SerializeField] float attackAnimLength = 1.3f;
    [SerializeField] Transform toolHolder;
    [SerializeField] Transform hand;


    private List<RigLayer> _layers = new List<RigLayer>();
    private void Awake()
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
    void Start()
    {
        _rigBuilder = GetComponent<RigBuilder>();
        _layers = _rigBuilder.layers;
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleAttacks();
    }

    private void HandleAttacks()
    {
        if (canAttack)
        {
            if (Input.GetMouseButtonDown(0) && !isAttacking) // Левая кнопка мыши
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); if (Physics.Raycast(ray, out RaycastHit hit, 5f)) { if (hit.collider.GetComponent<ItemPickup>() != null) { return; } } _animator.SetTrigger("attack"); // Переход в состояние Standing Melee Kick

                if (Hotbar.Instance.activeTool != null)
                {
                    Debug.Log("attack" + Hotbar.Instance.activeTool + "hand" + hand);
                    Hotbar.Instance.activeTool.transform.SetParent(hand);
                    HandleHandsIK(false);
                    StartCoroutine(AttackDelay());
                    isAttacking = true;
                }
            }
        }
    }
    public void EnableDisableAttacking(bool enabled)
    {
        canAttack = enabled;
    }
    private IEnumerator AttackDelay()
    {
        if (toolHolder != null && Hotbar.Instance.activeTool != null)
        {
            yield return new WaitForSeconds(attackAnimLength);
            HandleHandsIK(true);
            Hotbar.Instance.activeTool.transform.SetParent(toolHolder);
            isAttacking = false;
            Tool tool = Hotbar.Instance.activeTool.GetComponent<Tool>();
            Hotbar.Instance.activeTool.transform.localPosition = tool.GetHandPosition();
            Hotbar.Instance.activeTool.transform.localEulerAngles = tool.GetHandRotation();
        }
        else
        {
            Debug.Log("holder" + toolHolder + "active tool" + Hotbar.Instance.activeTool);
        }
    }
    private void HandleHandsIK(bool Active)
    {
        _rigBuilder.layers[0].active = Active;
    }
}
