using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("Referencias de Armas")]
    [SerializeField] private DisparoEscopeta escopeta;
    [SerializeField] private DisparoLazo lazo;

    [Header("Visual de Armas (Opcional)")]
    [SerializeField] private GameObject escopetaVisual;
    [SerializeField] private GameObject lazoVisual;

    private enum TipoArma { Escopeta, Lazo }
    private TipoArma armaActual = TipoArma.Escopeta;

    void Start()
    {
        // Obtener referencias si no están asignadas
        if (escopeta == null)
            escopeta = GetComponent<DisparoEscopeta>();

        if (lazo == null)
            lazo = GetComponent<DisparoLazo>();

        ActualizarArma();
    }

    void Update()
    {
        // Cambiar arma con CTRL
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        {
            CambiarArma();
        }

        // Disparar con clic izquierdo (solo si el arma está activa)
        if (Input.GetMouseButtonDown(0))
        {
            if (armaActual == TipoArma.Escopeta && escopeta != null && escopeta.enabled)
            {
                escopeta.Disparar();
            }
            else if (armaActual == TipoArma.Lazo && lazo != null && lazo.enabled)
            {
                lazo.LanzarLazo();
            }
        }
    }

    void CambiarArma()
    {
        if (armaActual == TipoArma.Escopeta)
        {
            armaActual = TipoArma.Lazo;
            Debug.Log("🪢 Equipaste el LAZO");
        }
        else
        {
            armaActual = TipoArma.Escopeta;
            Debug.Log("🔫 Equipaste la ESCOPETA");
        }

        ActualizarArma();
    }

    void ActualizarArma()
    {
        // Activar/desactivar scripts de armas
        if (escopeta != null)
            escopeta.enabled = (armaActual == TipoArma.Escopeta);

        if (lazo != null)
            lazo.enabled = (armaActual == TipoArma.Lazo);

        // Activar/desactivar visuales
        if (escopetaVisual != null)
            escopetaVisual.SetActive(armaActual == TipoArma.Escopeta);

        if (lazoVisual != null)
            lazoVisual.SetActive(armaActual == TipoArma.Lazo);
    }

    public bool EsLazoEquipado() => armaActual == TipoArma.Lazo;
    public bool EsEscopetaEquipada() => armaActual == TipoArma.Escopeta;
}