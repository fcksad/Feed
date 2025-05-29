using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(Image))]
public class TooltipImageListener : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private ITooltipService _toolTipService;

    [Inject]
    public void Construct(ITooltipService tooltipService)
    {
        _toolTipService = tooltipService;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _toolTipService.Show("asdsad asdsadsa asdsadsa dasd sad asdsa dsad asd asd asd asd");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _toolTipService.Hide();
    }
}
