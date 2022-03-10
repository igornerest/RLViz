using UnityEngine;

public class TooltipSystem : MonoBehaviour
{
    [SerializeField] private Tooltip tooltip;

    private static TooltipSystem current;

    public void Awake()
    {
        current = this;
    }

    public static void Show(string content)
    {
        current.tooltip.SetText(content);
        current.tooltip.gameObject.SetActive(true);
    }

    public static void Hide()
    {
        current.tooltip.gameObject.SetActive(false);
    }
}
