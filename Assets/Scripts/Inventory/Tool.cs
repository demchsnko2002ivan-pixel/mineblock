using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;
using static Tool;
using System.Runtime.ExceptionServices;

public class Tool : MonoBehaviour
{
    [SerializeField] Transform referenceRight;
    [SerializeField] Transform referenceLeft;
    [SerializeField] HoldingType holdingType;
    [SerializeField] int damage = 10;
    [SerializeField] Vector3 handPosition;
    [SerializeField] Vector3 handRotation;
    [SerializeField] bool throwable = false;
    [SerializeField] Gather canGather;

    [Header("Contact settings")]
    
    [SerializeField] Vector3 contactOffset;
    [SerializeField] float contactRadius = 0.1f;
    public enum HoldingType
    {
        rightHand, leftHand, bothHands
    }
    [Flags]
    public enum Gather
    {
        none = 0, wood = 1<<0, ore = 1<<1, entities = 1<<2, all = ~0
    }

    void Start()
    {
        
    }
    void Update()
    {
        
    }
    private void OnEnable()
    {
        PlayerAttackHandler.OnAttack += Attack;
    }
    private void OnDisable()
    {
        PlayerAttackHandler.OnAttack -= Attack;
    }
    private void Attack()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.TransformPoint(contactOffset), contactRadius);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Gatherable"))
            {
                Health health = collider.GetComponent<Health>();
                if (health.gatherable.HasFlag(canGather))
                {
                    health.TakeDamage(damage);
                    Debug.Log("Hit");
                }
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.TransformPoint(contactOffset), contactRadius);
    }
    public void GetReferences(out Transform refLeft, out Transform refRight)
    {
        refLeft = referenceLeft;
        refRight = referenceRight;
    }
    public Vector3 GetHandPosition() => handPosition;
    public Vector3 GetHandRotation() => handRotation;
}