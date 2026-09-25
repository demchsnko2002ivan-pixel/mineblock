using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class TreeHealth : Health
{
    private Animator animator;
    [SerializeField]
    private GameObject penyok;
    [SerializeField]
    private GameObject logs;
    [SerializeField]
    private GameObject particle;
    [SerializeField] GameObject breakParticle;
    protected override void Start()
    {
        animator = GetComponent<Animator>();
        base.Start();
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

        GameObject _penyok = Instantiate(penyok, transform.position, Quaternion.identity);
        _penyok.transform.localEulerAngles = transform.localEulerAngles;
        //_penyok.transform.SetParent(transform.parent);
        _penyok.transform.localScale = transform.localScale;
        if (animator != null)
        {
            int fall = Random.Range(1, 4);
            if (fall == 1)
            {
                animator.SetTrigger("Fall");
                Debug.Log("Fall");
            }
            else if (fall == 2)
            {
                animator.SetTrigger("Fall2");
                Debug.Log("Fall2");
            }
            else if (fall == 3)
            {
                animator.SetTrigger("Fall3");
                Debug.Log("Fall3");
            }
        }
        yield return new WaitForSeconds(2f);
        Instantiate(breakParticle, transform.position, Quaternion.identity);
        Drop(2);
        Destroy(gameObject, 1f);
    }
    public override void Death()
    {
        StartCoroutine(DeathDelay());
    }

    void Drop(int amount)
    {
        float range = 0.6f;

        for (int i = 0; i < amount; i++)
        {
            Vector3 offset = new Vector3(Random.Range(-range, range), 0, Random.Range(-range, range));
            Instantiate(logs, transform.position + offset, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
