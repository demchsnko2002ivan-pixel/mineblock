using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private string content;
    private float timer;
    private bool pointerStaying;
    private void Update()
    {
        if (pointerStaying)
        {
            timer += Time.deltaTime;
            if (timer >= 1)
            {
                TooltipManager.Instance.ShowTooltip(content);
                timer = 0;
            }
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerStaying = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        pointerStaying = false;
        TooltipManager.Instance.HideTooltip();
        timer = 0;
    }
}
