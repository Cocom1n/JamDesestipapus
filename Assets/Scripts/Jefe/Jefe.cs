using System;
using System.Collections;
using UnityEngine;

public class Jefe : MonoBehaviour, IDaniable, IMorir
{
    [SerializeField] private Transform jugador;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer; // 🆕 Para voltear sprite

    private Rigidbody2D rb;
    private float direccionActual = 1f; // 🆕 1 = derecha, -1 = izquierda

    [Header("Vida")]
    [SerializeField] private float vidaJefe = 200f;
    private float vidaActual;

    [Header("Carga/Embestida")]
    [SerializeField] private float velocidadCarga = 12f;
    [SerializeField] private float tiempoEntreCargas = 2f;
    [SerializeField] private float tiempoEsperaTrasCarga = 1f; // 🆕 Tiempo de espera después de chocar
    private bool estaCargando = false;

    [Header("Interacción con Jugador")]
    [SerializeField] private float fuerzaImpulsoJugador = 10f;
    [SerializeField] private float danioAlJugador = 20f;

    [Header("Referencias")]
    [SerializeField] private CaidaPicos caidaPicos;

    private AudioSource audioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        // 🆕 Obtener SpriteRenderer si no está asignado
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }
        }

        vidaActual = vidaJefe;

        // 🆕 Determinar dirección inicial (hacia el centro de la pantalla)
        Vector3 centroMapa = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
        direccionActual = transform.position.x < centroMapa.x ? 1f : -1f;
        ActualizarSprite();

        StartCoroutine(CicloDeAtaque());
    }

    IEnumerator CicloDeAtaque()
    {
        while (vidaActual > 0)
        {
            yield return new WaitForSeconds(tiempoEntreCargas);
            yield return Embestir();
        }
    }

    IEnumerator Embestir()
    {
        estaCargando = true;
        float altura = transform.position.y; // Mantener altura constante

        // Activar animación de corrida
        if (animator != null)
        {
            animator.SetBool("corriendo", true);
        }

        Debug.Log($"🐂 Jefe embiste hacia la {(direccionActual > 0 ? "DERECHA" : "IZQUIERDA")}");

        // Moverse en la dirección actual hasta chocar con límite
        while (estaCargando)
        {
            rb.linearVelocity = new Vector2(direccionActual * velocidadCarga, 0f);

            // Mantener altura constante
            transform.position = new Vector3(transform.position.x, altura, transform.position.z);

            yield return null;
        }

        // Detener movimiento
        rb.linearVelocity = Vector2.zero;

        // Desactivar animación de corrida
        if (animator != null)
        {
            animator.SetBool("corriendo", false);
        }

        // 🆕 Esperar un momento antes de la siguiente carga
        yield return new WaitForSeconds(tiempoEsperaTrasCarga);
    }

    // 🆕 Método llamado cuando llega al límite
    void OnLlegadaALimite()
    {
        estaCargando = false;

        // Cambiar dirección
        direccionActual *= -1f;
        ActualizarSprite();

        // Activar evento de picos si existe
        if (caidaPicos != null)
        {
            caidaPicos.ActivarCaidaPicos();
        }

        // Reproducir sonido
        if (audioSource != null)
        {
            audioSource.Play();
        }

        Debug.Log(" ¡Chocó con el límite!");
    }

    // 🆕 Actualizar sprite según dirección
    void ActualizarSprite()
    {
        if (spriteRenderer == null) return;

        // Si va hacia la derecha (direccionActual > 0), no voltear
        // Si va hacia la izquierda (direccionActual < 0), voltear
        spriteRenderer.flipX = direccionActual > 0;
    }

    public void RecibirDanio(float cantidad)
    {
        vidaActual -= cantidad;
        Debug.Log($"💔 Jefe recibió {cantidad} de daño. Vida actual: {vidaActual}");

        if (vidaActual <= 0f)
        {
            Morir();
        }
    }

    public void Morir()
    {
        Debug.Log("💀 Muerte del jefe");

        // Detener todo
        StopAllCoroutines();
        estaCargando = false;
        rb.linearVelocity = Vector2.zero;

        if (animator != null)
        {
            animator.SetBool("corriendo", false);
            // Puedes agregar: animator.SetTrigger("muerte");
        }

        enabled = false;
       //Aquí puedes agregar: Destroy(gameObject, 2f);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        // Colisión con jugador
        if (col.transform.CompareTag("Player"))
        {
            Debug.Log(" ¡Jefe golpeó al jugador!");

            // Aplicar daño
            IDaniable daniable = col.transform.GetComponent<IDaniable>();
            if (daniable != null)
            {
                daniable.RecibirDanio(danioAlJugador);
            }

            // Empujar jugador hacia arriba
            Rigidbody2D rbJugador = col.transform.GetComponent<Rigidbody2D>();
            if (rbJugador != null)
            {
                rbJugador.linearVelocity = new Vector2(rbJugador.linearVelocity.x, 0);
                rbJugador.AddForce(Vector2.up * fuerzaImpulsoJugador, ForceMode2D.Impulse);
            }
        }
        // Colisión con límite del mapa
        else if (col.transform.CompareTag("LimiteMapa"))
        {
            OnLlegadaALimite();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 direccionFlecha = new Vector3(direccionActual, 0, 0);
        Gizmos.DrawRay(transform.position, direccionFlecha * 2f);
    }
}