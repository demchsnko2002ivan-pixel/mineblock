using UnityEngine;

public class Food : MonoBehaviour
{
    private int CookStage=0;

    private float lastCookTime=0f;

    public int CookTime = 6;
    public bool CanBeCoocked = true;

    public GameObject CookedPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void Cook()
    {
        if (CanBeCoocked)
        {
            if (Time.time - lastCookTime > 0.5f)
            {
                CookStage++;
                lastCookTime = Time.time;
                Debug.Log("Try to Cook");
            }

            if (CookStage > CookTime)
            {
                FinishCooking();
            }
        }

    }

    private void FinishCooking()
    {
        if (CookedPrefab != null)
        {
            Instantiate(CookedPrefab, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning("Need Cooked Prefab!");
        }
    }
}
