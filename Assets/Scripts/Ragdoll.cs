using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    private Rigidbody[] rigidbodies;
    private Animator animator;
    private BoxCollider coll;
    void Start()
    {
        animator = GetComponent<Animator>();
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        coll = GetComponent<BoxCollider>();
        GoRagdoll(false);
    }
    public void GoRagdoll(bool enable)
    {
        animator.enabled = !enable;
        coll.enabled = !enable;
        foreach (var rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = !enable;
        }
    }
}
