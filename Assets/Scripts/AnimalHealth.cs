using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.ParticleSystem;

public class AnimalHealth : Health
{
    private AnimalAI ai;
    [SerializeField] GameObject particle;
    [SerializeField] GameObject breakParticle;
    [SerializeField] GameObject droppedItem;
    public int maxDrop;
    public int minDrop;

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
    private void OnTriggerEnter(Collider other)
    {
        // Example: Check if the object entering has the "Player" tag
        if (other.CompareTag("Tool"))
        {
            Vector3 hitPoint = other.ClosestPoint(transform.position);
            Tool tool = other.GetComponent<Tool>();
            if (tool.canDamage)
            {
                Instantiate(particle, hitPoint, Quaternion.identity);
                TakeDamage(20);
                Debug.Log(curHealth);
                Debug.Log("Damaged" + " " + other.gameObject.name);
                tool.canDamage = false;
            }
        }
    }
    IEnumerator DeathDelay()
    {
        yield return new WaitForSeconds(2f);
        Instantiate(breakParticle, transform.position, Quaternion.identity);
        Drop(Random.Range(maxDrop,minDrop));
        Destroy(gameObject, 1f);
    }
    override public void Death()
    {
        StartCoroutine(DeathDelay());
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
    void Drop(int amount)
    {
        float range = 0.6f;

        for (int i = 0; i < amount; i++)
        {
            if (droppedItem != null)
            {
                Vector3 offset = new Vector3(Random.Range(-range, range), 0, Random.Range(-range, range));
                Instantiate(droppedItem, transform.position + offset, Quaternion.identity);
            }
        }
        Destroy(gameObject);
    }
}
