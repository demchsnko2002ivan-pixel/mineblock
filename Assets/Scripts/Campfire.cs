using UnityEngine;

public class Campfire : InteractiveObject
{
    private bool isActive;

    public GameObject FireParticle;

    public float CookRadius = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      //  FireParticle.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void CheckObjects()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, CookRadius);

        foreach (Collider hit in colliders)
        {
            // Apply damage or knockback forces to the found items
            if(hit.CompareTag("Food"))
            {
                Food food = hit.gameObject.GetComponent<Food>();

                if (food)
                {
                    food.Cook();
                }
            }

            if(hit.CompareTag("Player"))
            {
                Debug.Log("DamagePlayer");
            }
        }
    }


    override public void Activate()
    {
        isActive = true;

        Debug.Log("Activated");

        FireParticle.SetActive(true);

        InvokeRepeating("CheckObjects", 0f, 0.5f);
    }

    public void OnTriggerStay(Collider other)
    {
        //if(other.CompareTag("Food"))
        //{

        //    Debug.Log("Try to Cook");
        //     Food food = other.gameObject.GetComponent<Food>();

        //       if(food)
        //        {
        //            food.Cook();
        //        }
        //}
    }

    void OnCollisionStay(Collision collisionInfo)
    {

        //if(collisionInfo.gameObject.CompareTag("Food"))
        //{
        //    Debug.Log("Try to Cook");
        //    Food food = collisionInfo.gameObject.GetComponent<Food>();

        //    if (food)
        //    {
        //        food.Cook();
        //    }
        //}
    }


}
