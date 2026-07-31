using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : Health
{
    private Animator animator;
    [SerializeField]
    private GameObject penyok;
    [SerializeField]
    private GameObject logs;
    protected override void Start()
    {
        animator = GetComponent<Animator>();
        base.Start();
    }
    protected override void Death()
    {
        animator.SetTrigger("fall");
        Destroy(gameObject, 1f);
        GameObject _penyok = Instantiate(penyok, transform.position, Quaternion.identity);
        _penyok.transform.localEulerAngles = transform.localEulerAngles;
        _penyok.transform.SetParent(transform.parent);
        _penyok.transform.localScale = transform.localScale;
        Drop(1);
    }
    void Drop(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Instantiate(logs, transform.position, Quaternion.identity);
        }
    }
}
