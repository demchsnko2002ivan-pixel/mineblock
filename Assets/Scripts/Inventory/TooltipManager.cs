using UnityEngine;
using TMPro;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance;

    [SerializeField]
    private TextMeshProUGUI tooltipText;
    [SerializeField]
    private GameObject tooltip;
    [SerializeField]
    Vector3 offset;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            tooltip.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowTooltip(string text)
    {
        tooltipText.text = text;
        tooltip.SetActive(true);
    }

    public void HideTooltip()
    {
        tooltip.SetActive(false);
        tooltipText.text = string.Empty;
    }

    private void Update()
    {
        tooltip.transform.position = Input.mousePosition + offset;
    }
}