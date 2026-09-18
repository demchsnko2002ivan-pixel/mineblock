using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    [SerializeField] private float lifetime = 2.0f;

    private void Start()
    {
        // Destroys this GameObject after 'lifetime' seconds
        Destroy(gameObject, lifetime);
    }
}

