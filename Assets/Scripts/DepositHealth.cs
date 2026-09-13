using UnityEngine;

public class DepositHealth : Health
{
    [SerializeField]
    private GameObject droppedItem;

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        // Example: Check if the object entering has the "Player" tag
        if (other.CompareTag("Tool"))
        {
            Tool tool = other.GetComponent<Tool>();
            if (tool.canDamage)
            {
                TakeDamage(20);
                Debug.Log(curHealth);
                Debug.Log("Damaged" + " " + other.gameObject.name);
                tool.canDamage = false;
            }
        }
    }
    public override void Death()
    {
        base.Death();
        int dropCount = Random.Range(3, 7);
        Drop(dropCount);
    }
    void Drop(int amount)
    {
        float range = 0.3f;
        Vector3 offset = new Vector3(
    Random.Range(-range, range),
    0,
    Random.Range(-range, range)
);

        for (int i = 0; i < amount; i++)
        {
            Instantiate(droppedItem, transform.position + offset, Quaternion.identity);
        }
        Destroy(gameObject);
    }
    //override public void Death()
    //{
    //Debug.Log("Dead");
    //}
}
