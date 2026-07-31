using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MinimapController : MonoBehaviour
{
    // Сколько раз в секунду должна обновляться миникарта.
    // 1f/5f означает 1 раз в 5 секунд.
    // Попробуйте начать с 1f/0.5f (2 раза в секунду) или 1f/1f (1 раз в секунду).
    public float updateInterval = 0.5f; // Интервал в секундах

    private Camera minimapCamera;
    private float nextUpdateTime = 0f;
    [SerializeField]
    private bool attachToPlayer = false;
    private Transform player;
    [SerializeField]
    private RectTransform image;

    void Start()
    {
        player = PlayerController.Instance.transform;
        if (attachToPlayer)
        {
            transform.position = player.position + Vector3.up * 20;
        }
        // Получаем компонент Camera
        minimapCamera = GetComponent<Camera>();

        // ОЧЕНЬ ВАЖНО: Отключаем автоматический рендеринг камеры
        minimapCamera.enabled = false;

        // Вызываем первый рендеринг сразу,y чтобы карта не была пустой
        RenderMinimap();
    }

    void Update()
    {
        // Проверяем, пришло ли время для следующего обновления
        if (Time.time >= nextUpdateTime)
        {
            // Устанавливаем время следующего обновления
            nextUpdateTime = Time.time + updateInterval;

            // Запускаем рендеринг миникарты
            RenderMinimap();
        }
    }
    private void LateUpdate()
    {
        if (attachToPlayer)
        {
            transform.position = player.position + Vector3.up * 20;
            float playerYRotation = player.eulerAngles.y;
            image.rotation = Quaternion.Euler(0.0f, 0.0f, playerYRotation);
        }
    }

    private void RenderMinimap()
    {
        // Выполняем рендеринг сцены с точки зрения этой камеры ОДИН раз
        minimapCamera.Render();

        // Опционально: можно добавить логику, чтобы рендерить только 
        // когда игрок движется, но это усложнит код.
    }
}