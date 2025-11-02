using DG.Tweening;
using UnityEngine;

public class BotonAnimacionSecuencia : MonoBehaviour
{
    private RectTransform rectTransform;
    private Vector2 posicionFinal;
    [SerializeField] float puntoMasBajo;
    [SerializeField] float duracionSubida;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        posicionFinal = rectTransform.anchoredPosition;
    }

    void OnEnable()
    {
        // Empieza desde más abajo
        rectTransform.anchoredPosition = posicionFinal + new Vector2(0, puntoMasBajo);
        rectTransform.localScale = Vector3.zero;

        // Creamos la secuencia
        Sequence anim = DOTween.Sequence();

        // Paso 1: sube a su posición
        anim.Append(rectTransform.DOAnchorPos(posicionFinal, duracionSubida).SetEase(Ease.OutBack));

        // Paso 2 (al mismo tiempo): escala y hace rebote el tamaño
        anim.Join(rectTransform.DOScale(1.1f, duracionSubida).SetEase(Ease.OutElastic));

        // Paso 3: vuelve al tamaño normal
        anim.Append(rectTransform.DOScale(1f, 0.2f));
    }
}
