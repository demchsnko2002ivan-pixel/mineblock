using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimalHealth : Health
{
    private AnimalAI ai;

    protected override void Start()
    {
        base.Start();
        ai = GetComponent<AnimalAI>();
    }
    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        ai.RunAway();
    }
    override protected void Death()
    {
        NavMeshAgent agent;
        if (TryGetComponent<NavMeshAgent>(out agent))
        {
            agent.enabled = false;
        }
        GetComponent<Rigidbody>().isKinematic = false;
        AnimalAI animalAI;
        if (TryGetComponent<AnimalAI>(out animalAI))
        {
            animalAI.enabled = false;
        }
        Animator animator;
        if (TryGetComponent<Animator>(out animator))
        {
            animator.enabled = false;
        }
        Ragdoll ragdoll;
        if (TryGetComponent<Ragdoll>(out ragdoll))
        {
            ragdoll.GoRagdoll(true);
        }
    }
}
