using UnityEngine;
using TMPro;

public class FPSDisplay : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI fpsText;
    [SerializeField]
    private float updateInterval = 0.5f;

    private float accumulatedTime = 0f;
    private int frames = 0;
    private float timeleft;
    private float fps = 0f;

    void Start()
    {
        if (fpsText == null)
        {
            Debug.LogError("FPS Text (TextMeshProUGUI) не призначено!");
            enabled = false; // Вимкнути скрипт, якщо немає Text
            return;
        }

        timeleft = updateInterval;
    }

    void Update()
    {
        timeleft -= Time.deltaTime;
        accumulatedTime += Time.timeScale / Time.deltaTime;
        frames++;

        if (timeleft <= 0.0)
        {
            fps = accumulatedTime / frames;

            string newText = string.Format("{0:0.0} FPS", fps);
            fpsText.text = newText;

            timeleft = updateInterval;
            accumulatedTime = 0.0f;
            frames = 0;
        }
    }
}