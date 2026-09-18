using UnityEngine;

public class DepositHealth : Health
{
    [SerializeField]
    private GameObject droppedItem;
    [SerializeField] GameObject particle;
    [SerializeField] GameObject breakParticle;

    // Update is called once per frame
    void Update()
    {

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
    public override void Death()
    {
        Instantiate(breakParticle, transform.position, Quaternion.identity);
        base.Death();
        int dropCount = Random.Range(3, 7);
        Drop(dropCount);
    }
    void Drop(int amount)
    {
        float range = 0.6f;

        for (int i = 0; i < amount; i++)
        {
            Vector3 offset = new Vector3(Random.Range(-range, range), 0, Random.Range(-range, range));
            Instantiate(droppedItem, transform.position + offset, Quaternion.identity);
        }
        Destroy(gameObject);
    }
    //override public void Death()
    //{
    //Debug.Log("Dead");
    //}
}
