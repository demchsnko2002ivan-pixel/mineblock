using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField]
    private RectTransform map;
    [SerializeField]
    private RenderTexture renderTexture;
    void Start()
    {
        map.gameObject.SetActive(false);
        renderTexture.Release();
        renderTexture.width = Mathf.RoundToInt(map.rect.width);
        renderTexture.height = Mathf.RoundToInt(map.rect.height);
        renderTexture.Create();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.M))
        {
            map.gameObject.SetActive(!map.gameObject.activeSelf);
        }
    }
}
