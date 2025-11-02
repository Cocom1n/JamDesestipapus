using System.Collections;
using UnityEngine;

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
    private void Start()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.RegistrarOveja(this);
        vidaActual = vidaMax;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        animator.SetTrigger("Aparecer");
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
        //Debug.Log("Oveja siendo abducida!");
    }

    private void DesactivarAnimacionAbduccion()
    {
        estaAbducida = false;
        animator.SetBool("Abducida", false);
        //Debug.Log("Oveja ya no está siendo abducida");
    }

    public void Morir()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.EliminarOveja(this);
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
        if (vidaActual <= 0)
        {
            Morir();
        }
    }
}