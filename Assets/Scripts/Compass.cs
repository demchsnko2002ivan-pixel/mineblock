using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : MonoBehaviour
{
    [SerializeField]
    private Transform player;
    private RectTransform compass;
    void Start()
    {
        compass = GetComponent<RectTransform>();
    }

    void LateUpdate()
    {
        float playerYRotation = player.eulerAngles.y;
        compass.rotation = Quaternion.Euler(0.0f, 0.0f, playerYRotation);
    }
}
