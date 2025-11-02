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
    [SerializeField] private Transform puntoInicio;
    [SerializeField] private Transform puntoSalida;

    [Header("Movimiento y tiempos")]
    [SerializeField] private float tiempoEspera = 5f;
    [SerializeField] private float duracionMovimiento = 1.5f;
    [SerializeField] private float duracionElevacion = 1f;

    [Header("Movimiento Senoidal")]
    [SerializeField] private float amplitudSenoidal = 0.3f;
    [SerializeField] private float frecuenciaSenoidal = 2f;

    [Header("Captura")]
    [SerializeField] private float alturaCaptura = 2f;
    [SerializeField] private float tiempoMaximoCaptura = 8f;
    [SerializeField] private float distanciaRaycast = 10f;
    [SerializeField] private LayerMask layerObstaculos;

    [Header("Sistema de Vida")]
    [SerializeField] private float vidaMaxima = 100f;
    private float vidaActual;

    [Header("🔊 Sistema de Sonidos")]
    [SerializeField] private AudioSource audioSourceLevitacion;   // Para sonido constante de movimiento
    [SerializeField] private AudioSource audioSourceAbduccion;    // Para sonido de abducción
    [SerializeField] private AudioSource audioSourceExplosion;    // Para sonido de explosión

    [Header("Clips de Audio")]
    [SerializeField] private AudioClip clipLevitacion;
    [SerializeField] private AudioClip clipAbduccion;
    [SerializeField] private AudioClip clipExplosion;

    [Header("Configuración de Volumen")]
    [SerializeField] private float volumenLevitacion = 0.5f;
    [SerializeField] private float volumenAbduccion = 0.7f;
    [SerializeField] private float volumenExplosion = 1f;

    private Camera cam;
    private RecogerObjeto succionador;
    private Oveja objetivoActual;
    private Animator animator;

    private EstadoOvni estadoActual = EstadoOvni.Idle;
    private float alturaBase;
    private bool enProceso = false;
    private float tiempoSenoidal = 0f;

    public Oveja ObjetivoActual => objetivoActual;

    private void Start()
    {
        cam = Camera.main;
        succionador = GetComponent<RecogerObjeto>();
        animator = GetComponentInChildren<Animator>();

        ConfigurarAudioSources();

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

    // 🆕 CONFIGURAR AUDIO SOURCES
    private void ConfigurarAudioSources()
    {
        // Si no hay AudioSources asignados, crearlos automáticamente
        if (audioSourceLevitacion == null)
        {
            audioSourceLevitacion = gameObject.AddComponent<AudioSource>();
        }
        if (audioSourceAbduccion == null)
        {
            audioSourceAbduccion = gameObject.AddComponent<AudioSource>();
        }
        if (audioSourceExplosion == null)
        {
            audioSourceExplosion = gameObject.AddComponent<AudioSource>();
        }

        // Configurar AudioSource de Levitación (loop constante)
        audioSourceLevitacion.clip = clipLevitacion;
        audioSourceLevitacion.loop = true;
        audioSourceLevitacion.volume = volumenLevitacion;
        audioSourceLevitacion.playOnAwake = false;

        // Configurar AudioSource de Abducción (loop durante abducción)
        audioSourceAbduccion.clip = clipAbduccion;
        audioSourceAbduccion.loop = true;
        audioSourceAbduccion.volume = volumenAbduccion;
        audioSourceAbduccion.playOnAwake = false;

        // Configurar AudioSource de Explosión (one-shot)
        audioSourceExplosion.clip = clipExplosion;
        audioSourceExplosion.loop = false;
        audioSourceExplosion.volume = volumenExplosion;
        audioSourceExplosion.playOnAwake = false;
    }

    private void Update()
    {
        if (estadoActual == EstadoOvni.Dead) return;

        if (estadoActual == EstadoOvni.MoveToTarget || estadoActual == EstadoOvni.Attacking)
        {
            AplicarMovimientoSenoidal();
        }

        if (objetivoActual == null && estadoActual == EstadoOvni.Attacking)
        {
            succionador.enabled = false;
            animator.SetBool("Levantando", false);
            DetenerSonidoAbduccion(); // 🆕
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
    // 🔊 MÉTODOS DE SONIDO
    // -------------------

    private void ReproducirSonidoLevitacion()
    {
        if (audioSourceLevitacion != null && clipLevitacion != null && !audioSourceLevitacion.isPlaying)
        {
            audioSourceLevitacion.Play();
            Debug.Log("🎵 Sonido de levitación iniciado");
        }
    }

    private void DetenerSonidoLevitacion()
    {
        if (audioSourceLevitacion != null && audioSourceLevitacion.isPlaying)
        {
            audioSourceLevitacion.Stop();
            Debug.Log("🔇 Sonido de levitación detenido");
        }
    }

    private void ReproducirSonidoAbduccion()
    {
        if (audioSourceAbduccion != null && clipAbduccion != null && !audioSourceAbduccion.isPlaying)
        {
            audioSourceAbduccion.Play();
            Debug.Log("🎵 Sonido de abducción iniciado");
        }
    }

    private void DetenerSonidoAbduccion()
    {
        if (audioSourceAbduccion != null && audioSourceAbduccion.isPlaying)
        {
            audioSourceAbduccion.Stop();
            Debug.Log("🔇 Sonido de abducción detenido");
        }
    }

    private void ReproducirSonidoExplosion()
    {
        if (audioSourceExplosion != null && clipExplosion != null)
        {
            audioSourceExplosion.Play();
            Debug.Log("💥 Sonido de explosión reproducido");
        }
    }

    private void DetenerTodosLosSonidos()
    {
        DetenerSonidoLevitacion();
        DetenerSonidoAbduccion();
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
        RaycastHit2D hit = Physics2D.Raycast(
            posicionOveja,
            Vector2.up,
            distanciaRaycast,
            layerObstaculos
        );

        bool espacioLibre = hit.collider == null;

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

        DetenerTodosLosSonidos(); // 🆕 Detener sonidos al entrar en Idle

        transform.position = puntoInicio.position;
        alturaBase = puntoInicio.position.y;
        tiempoSenoidal = 0f;

        yield return new WaitForSeconds(tiempoEspera);

        var ovejas = GameManager.Instance.GetOvejasVivas();
        if (ovejas.Count > 0)
        {
            Oveja ovejaSeleccionada = null;
            int intentos = 0;
            int maxIntentos = ovejas.Count * 2;

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
                enProceso = false;
            }
        }

        enProceso = false;
    }

    private IEnumerator EstadoMover()
    {
        enProceso = true;

        ReproducirSonidoLevitacion(); // 🆕 Iniciar sonido de levitación

        if (objetivoActual == null)
        {
            CambiarEstado(EstadoOvni.Idle);
            enProceso = false;
            yield break;
        }

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

        float tiempoTranscurrido = 0f;
        Vector3 inicio = transform.position;
        inicio.y = alturaBase;

        while (tiempoTranscurrido < duracionMovimiento)
        {
            tiempoTranscurrido += Time.deltaTime;
            float t = tiempoTranscurrido / duracionMovimiento;

            float x = Mathf.Lerp(inicio.x, destino.x, Mathf.SmoothStep(0, 1, t));
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

        ReproducirSonidoAbduccion(); // 🆕 Iniciar sonido de abducción
        // El sonido de levitación sigue sonando

        float tiempoTranscurrido = 0f;

        while (objetivoActual != null && tiempoTranscurrido < tiempoMaximoCaptura)
        {
            tiempoTranscurrido += Time.deltaTime;

            if (!TieneEspacioLibre(objetivoActual.transform.position))
            {
                Debug.Log("¡La oveja se refugió bajo un techo!");
                break;
            }

            if (objetivoActual.transform.position.y >= transform.position.y - alturaCaptura)
            {
                CapturarOveja();
                enProceso = false;
                yield break;
            }

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
        DetenerSonidoAbduccion(); // 🆕 Detener abducción si falló

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
        DetenerSonidoAbduccion(); // 🆕 Detener sonido de abducción

        CambiarEstado(EstadoOvni.Retreating);
    }

    private IEnumerator EstadoRetirada()
    {
        enProceso = true;

        // El sonido de levitación sigue mientras se retira
        ReproducirSonidoLevitacion();

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
        DetenerSonidoAbduccion(); // 🆕
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

        // 🆕 SONIDOS AL MORIR
        DetenerTodosLosSonidos();
        ReproducirSonidoExplosion();

        Destroy(gameObject, 2f);
    }

    public void RecibirDanio(float cantidad)
    {
        if (estadoActual == EstadoOvni.Dead) return;

        vidaActual -= cantidad;
        Debug.Log($"OVNI recibió {cantidad} de daño. Vida restante: {vidaActual}");

        GetComponentInChildren<SpriteRenderer>()?.DOColor(Color.red, 0.1f).SetLoops(2, LoopType.Yoyo);

        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    // -------------------
    // CLEANUP
    // -------------------

    private void OnDestroy()
    {
        DetenerTodosLosSonidos();
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