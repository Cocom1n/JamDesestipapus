using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
public class BotonColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image imagen;
    [SerializeField] private Color colorHover = Color.cyan;
    private Color colorOriginal;

    private void Start()
    {
        colorOriginal = imagen.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        imagen.DOColor(colorHover, 0.2f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        imagen.DOColor(colorOriginal, 0.2f);
    }
}
