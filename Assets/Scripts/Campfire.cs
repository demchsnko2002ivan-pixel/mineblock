using UnityEngine;

public class Campfire : InteractiveObject
{
    private bool isActive;

    public GameObject FireParticle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      //  FireParticle.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    override public void Activate()
    {
        isActive = true;

        Debug.Log("Activated");

        FireParticle.SetActive(true);
    }
}
