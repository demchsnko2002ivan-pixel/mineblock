using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolControler : MonoBehaviour
{
    [SerializeField]
    private int damage = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (PlayerAttackHandler.Instance.isAttacking)
        {
            if (other.CompareTag("Animal"))
            {
                other.transform.GetComponent<Health>().TakeDamage(damage);
                Debug.Log("Hit");
            }
            else if (other.CompareTag("Tree"))
            {
                other.transform.GetComponent<Health>().TakeDamage(damage);
                Debug.Log("Tree hit");
            }
        }
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
