using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PricelFollow : MonoBehaviour
{
    public static PricelFollow Instance { get; private set; }
    [SerializeField]
    private Canvas canvas;
    private bool isFollowing = false;
    private Vector2 startPosition;
    private RectTransform rt;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        rt = GetComponent<RectTransform>();
        startPosition = rt.localPosition;
    }
    public void Enable()
    {
        isFollowing = true;
    }
    public void Disable()
    {
        isFollowing = false;
        rt.localPosition = startPosition;
    }
    void Update()
    {
        if (isFollowing)
        {
            Vector2 _mousePosition = Input.mousePosition;
            Vector2 _localPoint;
             // RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), _mousePosition, Camera.main, out _localPoint);
            rt.position = _mousePosition;
        }
    }
}
