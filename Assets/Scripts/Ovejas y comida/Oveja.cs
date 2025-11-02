using UnityEngine;
using System.Collections;

public class Oveja : MonoBehaviour, IMorir, IDaniable, IDesactivarMovimiento
{
    Animator animator;
    Rigidbody2D rb;

    [Header("Detección de Abducción")]
    [SerializeField] private float velocidadMinimaAbduccion = 1f; // Velocidad hacia arriba para activar animación
    [SerializeField] private float velocidadMinimaDesactivar = 0.5f; // Velocidad mínima para mantener la animación
    [SerializeField] private float vidaMax;
    private float vidaActual;
    private bool estaAbducida = false;

    private float direccion;
    private bool aturdido = false;
    private Coroutine reenableCoroutine;

    [Header("Audio - Oveja")]
    [SerializeField] private AudioClip clipAparecer;        // sonido al aparecer
    [SerializeField] private AudioClip clipAbduccion;      // sonido mientras la abducen (loop recomendado)
    [SerializeField] private AudioClip clipRecibirDanio;   // sonido al recibir daño / morir
    [SerializeField][Range(0f, 1f)] private float volumenSFX = 1f;

    private AudioSource sfxSource;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.RegistrarOveja(this);

        vidaActual = vidaMax;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        animator.SetTrigger("Aparecer");
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Obtener o crear AudioSource para SFX
        sfxSource = GetComponent<AudioSource>();
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
            sfxSource.loop = false;
        }
        sfxSource.volume = volumenSFX;

        // Reproducir sonido de aparecer (una vez)
        if (clipAparecer != null)
            sfxSource.PlayOneShot(clipAparecer, volumenSFX);
    }

    private void Update()
    {
        if (rb != null)
        {
            // Activar animación si sube con suficiente velocidad
            if (!estaAbducida && rb.linearVelocity.y > velocidadMinimaAbduccion)
            {
                ActivarAnimacionAbduccion();
            }
            // Desactivar animación si la velocidad cae por debajo del umbral
            else if (estaAbducida && rb.linearVelocity.y < velocidadMinimaDesactivar)
            {
                DesactivarAnimacionAbduccion();
            }
        }
    }

    // --- IDesactivarMovimiento ---
    public void DesactivarMovimiento(float duracion)
    {
        // cancelar reenable previo
        if (reenableCoroutine != null)
        {
            StopCoroutine(reenableCoroutine);
            reenableCoroutine = null;
        }

        aturdido = true;
        reenableCoroutine = StartCoroutine(ReactivarDespues(duracion));
    }

    public void ReactivarMovimiento()
    {
        if (reenableCoroutine != null)
        {
            StopCoroutine(reenableCoroutine);
            reenableCoroutine = null;
        }

        aturdido = false;
    }

    private IEnumerator ReactivarDespues(float segundos)
    {
        yield return new WaitForSeconds(segundos);
        aturdido = false;
        reenableCoroutine = null;
    }

    private void ActivarAnimacionAbduccion()
    {
        estaAbducida = true;
        animator.SetBool("Abducida", true);

        // Iniciar sonido de abducción en loop
        if (clipAbduccion != null && sfxSource != null)
        {
            sfxSource.clip = clipAbduccion;
            sfxSource.loop = true;
            sfxSource.Play();
        }
    }

    private void DesactivarAnimacionAbduccion()
    {
        estaAbducida = false;
        animator.SetBool("Abducida", false);

        // Parar sonido de abducción
        if (sfxSource != null && sfxSource.clip == clipAbduccion)
        {
            sfxSource.Stop();
            sfxSource.clip = null;
            sfxSource.loop = false;
        }
    }

    public void Morir()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.EliminarOveja(this);

        // Reproducir sonido de muerte y destruir al terminar el clip
        if (clipRecibirDanio != null && sfxSource != null)
        {
            sfxSource.loop = false;
            sfxSource.Stop(); // parar cualquier loop (p.ej. abducción)
            sfxSource.PlayOneShot(clipRecibirDanio, volumenSFX);
            spriteRenderer.enabled = false; // ocultar sprite inmediatamente
            StartCoroutine(DelayedDestroy(clipRecibirDanio.length));
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator DelayedDestroy(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    // Este método puede ser llamado manualmente si lo necesitas
    public void Abducida()
    {
        ActivarAnimacionAbduccion();
    }

    public void RecibirDanio(float daño)
    {
        vidaActual -= daño;

        // Reproducir sonido de recibir daño (no interrumpe la abducción loop si existe)
        if (clipRecibirDanio != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clipRecibirDanio, volumenSFX);
        }

        if (vidaActual <= 0)
        {
            Morir();
        }
    }
}