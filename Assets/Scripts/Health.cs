using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField]
    protected int maxHealth = 30;
    protected int curHealth;
    protected bool death = false;
    [SerializeField]
    public Tool.Gather gatherable;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        curHealth = maxHealth;
    }
    public virtual void TakeDamage(int damage)
    {
        curHealth -= damage;
        Debug.Log(curHealth.ToString());
        if (curHealth <= 0 && !death)
        {
            death = true;
            Death();
        }
    }
    protected virtual void Death()
    {

    }
}
