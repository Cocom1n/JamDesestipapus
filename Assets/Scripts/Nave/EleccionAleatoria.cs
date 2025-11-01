using DG.Tweening;
using System.Collections;
using UnityEngine;

public class EleccionAleatoria : MonoBehaviour, IDaniable, IMorir
{
    public enum EstadoOvni
    {
        Idle,
        MoveToTarget,
        Attacking,
        Retreating,
        Dead
    }

    [Header("Puntos de referencia")]
    [SerializeField] private Transform puntoInicio; // Donde aparece el OVNI
    [SerializeField] private Transform puntoSalida; // Donde se va después de capturar

    [Header("Movimiento y tiempos")]
    [SerializeField] private float tiempoEspera = 5f;
    [SerializeField] private float duracionMovimiento = 1.5f;
    [SerializeField] private float duracionElevacion = 1f;

    [Header("Movimiento Senoidal")]
    [SerializeField] private float amplitudSenoidal = 0.3f; // Cuánto se mueve arriba/abajo
    [SerializeField] private float frecuenciaSenoidal = 2f; // Qué tan rápido oscila

    [Header("Captura")]
    [SerializeField] private float alturaCaptura = 2f;
    [SerializeField] private float tiempoMaximoCaptura = 8f;
    [SerializeField] private float distanciaRaycast = 10f; // Distancia para detectar techos
    [SerializeField] private LayerMask layerObstaculos; // Layer de techos/obstáculos

    [Header("Sistema de Vida")]
    [SerializeField] private float vidaMaxima = 100f;
    private float vidaActual;

    private Camera cam;
    private RecogerObjeto succionador;
    private Oveja objetivoActual;
    private Animator animator;

    private EstadoOvni estadoActual = EstadoOvni.Idle;
    private float alturaBase; // Altura base para el movimiento senoidal
    private bool enProceso = false;
    private float tiempoSenoidal = 0f;

    public Oveja ObjetivoActual => objetivoActual;

    private void Start()
    {
        cam = Camera.main;
        succionador = GetComponent<RecogerObjeto>();
        animator = GetComponentInChildren<Animator>();

        // Si no hay puntos asignados, usar posiciones por defecto
        if (puntoInicio == null)
        {
            GameObject inicio = new GameObject("PuntoInicio_OVNI");
            inicio.transform.position = transform.position;
            puntoInicio = inicio.transform;
        }

        if (puntoSalida == null)
        {
            GameObject salida = new GameObject("PuntoSalida_OVNI");
            salida.transform.position = transform.position + Vector3.right * 10f;
            puntoSalida = salida.transform;
        }

        alturaBase = puntoInicio.position.y;
        transform.position = puntoInicio.position;
        CambiarEstado(EstadoOvni.Idle);
        vidaActual = vidaMaxima;
    }

    private void Update()
    {
        if (estadoActual == EstadoOvni.Dead) return;

        // Aplicar movimiento senoidal cuando está en movimiento o atacando
        if (estadoActual == EstadoOvni.MoveToTarget || estadoActual == EstadoOvni.Attacking)
        {
            AplicarMovimientoSenoidal();
        }

        // Si el objetivo murió o desapareció
        if (objetivoActual == null && estadoActual == EstadoOvni.Attacking)
        {
            succionador.enabled = false;
            animator.SetBool("Levantando", false);
            CambiarEstado(EstadoOvni.Idle);
        }

        if (!enProceso)
        {
            switch (estadoActual)
            {
                case EstadoOvni.Idle:
                    StartCoroutine(EstadoIdle());
                    break;
                case EstadoOvni.MoveToTarget:
                    StartCoroutine(EstadoMover());
                    break;
                case EstadoOvni.Attacking:
                    StartCoroutine(EstadoAtacar());
                    break;
                case EstadoOvni.Retreating:
                    StartCoroutine(EstadoRetirada());
                    break;
            }
        }
    }

    // -------------------
    // MOVIMIENTO SENOIDAL
    // -------------------

    private void AplicarMovimientoSenoidal()
    {
        tiempoSenoidal += Time.deltaTime * frecuenciaSenoidal;
        float offsetY = Mathf.Sin(tiempoSenoidal) * amplitudSenoidal;

        Vector3 pos = transform.position;
        pos.y = alturaBase + offsetY;
        transform.position = pos;
    }

    // -------------------
    // DETECCIÓN DE TECHO
    // -------------------

    private bool TieneEspacioLibre(Vector3 posicionOveja)
    {
        // Raycast desde la oveja hacia arriba
        RaycastHit2D hit = Physics2D.Raycast(
            posicionOveja,
            Vector2.up,
            distanciaRaycast,
            layerObstaculos
        );

        // Si NO hay obstáculo, hay espacio libre
        bool espacioLibre = hit.collider == null;

        // Debug visual
        if (espacioLibre)
            Debug.DrawRay(posicionOveja, Vector2.up * distanciaRaycast, Color.green, 0.5f);
        else
            Debug.DrawRay(posicionOveja, Vector2.up * hit.distance, Color.red, 0.5f);

        return espacioLibre;
    }

    // -------------------
    // ESTADOS
    // -------------------

    private IEnumerator EstadoIdle()
    {
        enProceso = true;
        succionador.enabled = false;
        animator.SetBool("Levantando", false);

        // Volver al punto de inicio
        transform.position = puntoInicio.position;
        alturaBase = puntoInicio.position.y;
        tiempoSenoidal = 0f;

        yield return new WaitForSeconds(tiempoEspera);

        // Buscar ovejas válidas (con espacio libre)
        var ovejas = GameManager.Instance.GetOvejasVivas();
        if (ovejas.Count > 0)
        {
            // Intentar encontrar una oveja sin techo
            Oveja ovejaSeleccionada = null;
            int intentos = 0;
            int maxIntentos = ovejas.Count * 2; // Revisar varias veces

            while (ovejaSeleccionada == null && intentos < maxIntentos)
            {
                Oveja candidata = ovejas[Random.Range(0, ovejas.Count)];

                if (TieneEspacioLibre(candidata.transform.position))
                {
                    ovejaSeleccionada = candidata;
                    break;
                }

                intentos++;
            }

            if (ovejaSeleccionada != null)
            {
                objetivoActual = ovejaSeleccionada;
                CambiarEstado(EstadoOvni.MoveToTarget);
            }
            else
            {
                Debug.Log("No hay ovejas con espacio libre para abducir");
                // Reintentar después
                enProceso = false;
            }
        }

        enProceso = false;
    }

    private IEnumerator EstadoMover()
    {
        enProceso = true;

        if (objetivoActual == null)
        {
            CambiarEstado(EstadoOvni.Idle);
            enProceso = false;
            yield break;
        }

        // Verificar de nuevo que sigue teniendo espacio libre
        if (!TieneEspacioLibre(objetivoActual.transform.position))
        {
            Debug.Log("El objetivo entró a una zona con techo");
            objetivoActual = null;
            CambiarEstado(EstadoOvni.Idle);
            enProceso = false;
            yield break;
        }

        Vector3 destino = new Vector3(
            objetivoActual.transform.position.x,
            alturaBase,
            transform.position.z
        );

        // Mover sin DOTween para mantener control del senoidal
        float tiempoTranscurrido = 0f;
        Vector3 inicio = transform.position;
        inicio.y = alturaBase; // Mantener altura base

        while (tiempoTranscurrido < duracionMovimiento)
        {
            tiempoTranscurrido += Time.deltaTime;
            float t = tiempoTranscurrido / duracionMovimiento;

            // Movimiento horizontal con ease
            float x = Mathf.Lerp(inicio.x, destino.x, Mathf.SmoothStep(0, 1, t));

            // Actualizar altura base (sin senoidal, eso se aplica en Update)
            alturaBase = Mathf.Lerp(inicio.y, destino.y, t);

            Vector3 nuevaPos = new Vector3(x, alturaBase, transform.position.z);
            transform.position = nuevaPos;

            yield return null;
        }

        CambiarEstado(EstadoOvni.Attacking);
        enProceso = false;
    }

    private IEnumerator EstadoAtacar()
    {
        enProceso = true;

        if (objetivoActual == null)
        {
            CambiarEstado(EstadoOvni.Idle);
            enProceso = false;
            yield break;
        }

        animator.SetBool("Levantando", true);
        succionador.enabled = true;

        float tiempoTranscurrido = 0f;

        while (objetivoActual != null && tiempoTranscurrido < tiempoMaximoCaptura)
        {
            tiempoTranscurrido += Time.deltaTime;

            // Verificar si entró a una zona con techo
            if (!TieneEspacioLibre(objetivoActual.transform.position))
            {
                Debug.Log("¡La oveja se refugió bajo un techo!");
                break;
            }

            // Verificar altura de captura
            if (objetivoActual.transform.position.y >= transform.position.y - alturaCaptura)
            {
                CapturarOveja();
                enProceso = false;
                yield break;
            }

            // Reposicionar si pierde detección
            if (!succionador.Detectando)
            {
                alturaBase = transform.position.y;
                Vector3 nuevaPos = new Vector3(
                    objetivoActual.transform.position.x,
                    alturaBase,
                    transform.position.z
                );
                transform.position = nuevaPos;
            }

            yield return null;
        }

        succionador.enabled = false;
        animator.SetBool("Levantando", false);

        // Si no capturó, reintentar
        if (objetivoActual != null)
        {
            CambiarEstado(EstadoOvni.MoveToTarget);
        }
        else
        {
            CambiarEstado(EstadoOvni.Idle);
        }

        enProceso = false;
    }

    private void CapturarOveja()
    {
        if (objetivoActual == null) return;

        Debug.Log($"¡Oveja capturada por el OVNI!");

        objetivoActual.Morir();
        objetivoActual = null;

        succionador.enabled = false;
        animator.SetBool("Levantando", false);
        CambiarEstado(EstadoOvni.Retreating);
    }

    private IEnumerator EstadoRetirada()
    {
        enProceso = true;

        Vector3 destino = puntoSalida.position;

        yield return transform.DOMove(destino, duracionElevacion)
            .SetEase(Ease.InOutSine)
            .WaitForCompletion();

        CambiarEstado(EstadoOvni.Idle);
        enProceso = false;
    }

    // -------------------
    // MÉTODOS AUXILIARES
    // -------------------

    public void ForzarRetirada()
    {
        if (estadoActual == EstadoOvni.Dead) return;

        StopAllCoroutines();
        succionador.enabled = false;
        animator.SetBool("Levantando", false);
        enProceso = false;
        CambiarEstado(EstadoOvni.Retreating);
        StartCoroutine(EstadoRetirada());
    }

    private void CambiarEstado(EstadoOvni nuevo)
    {
        estadoActual = nuevo;
    }

    public void Morir()
    {
        if (estadoActual == EstadoOvni.Dead) return;
        StopAllCoroutines();
        succionador.enabled = false;
        animator.SetTrigger("Morir");
        estadoActual = EstadoOvni.Dead;
        transform.DOKill();
        Destroy(gameObject, 2f);
    }

    public void RecibirDanio(float cantidad)
    {
        if (estadoActual == EstadoOvni.Dead) return;

        vidaActual -= cantidad;
        Debug.Log($"OVNI recibió {cantidad} de daño. Vida restante: {vidaActual}");

        // Efecto visual opcional (parpadeo)
         GetComponentInChildren<SpriteRenderer>()?.DOColor(Color.red, 0.1f).SetLoops(2, LoopType.Yoyo);

        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    // -------------------
    // GIZMOS DE DEBUG
    // -------------------

    private void OnDrawGizmos()
    {
        if (puntoInicio != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(puntoInicio.position, 0.5f);
            Gizmos.DrawLine(puntoInicio.position, puntoInicio.position + Vector3.down * 2f);
        }

        if (puntoSalida != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(puntoSalida.position, 0.5f);
            Gizmos.DrawLine(puntoSalida.position, puntoSalida.position + Vector3.up * 2f);
        }
    }
}