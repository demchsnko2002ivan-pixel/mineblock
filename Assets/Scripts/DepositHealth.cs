using System.Collections.Generic;
using UnityEngine;

public class DepositHealth : Health
{
    [SerializeField] GameObject droppedItem;
    [SerializeField] GameObject particle;
    [SerializeField] GameObject breakParticle;
    [System.Serializable]
    public struct ObjectData
    {
        public GameObject DropObject;
        public float Chance;
    }
    public List<ObjectData> objectDataList = new List<ObjectData>();

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
        for (int i = 0; i < amount; i++)
        {
            foreach (ObjectData data in objectDataList)
            {
                float r = Random.Range(0f, 1f);
                if (r < data.Chance)
                {
                    Drop(data.DropObject);
                }
            }
        }
        Destroy(gameObject);
    }
    private void Drop(GameObject drop)
    {
        float range = 0.6f;
        Vector3 offset = new Vector3(Random.Range(-range, range), 0, Random.Range(-range, range));
        Instantiate(drop, transform.position + offset, Quaternion.identity);
    }
}
