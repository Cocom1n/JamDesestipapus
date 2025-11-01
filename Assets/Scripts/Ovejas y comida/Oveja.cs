using UnityEngine;

public class Oveja : MonoBehaviour, IMorir
{
    Animator animator;
    Rigidbody2D rb;

    [Header("Detecci�n de Abducci�n")]
    [SerializeField] private float velocidadMinimaAbduccion = 1f; // Velocidad hacia arriba para activar animaci�n
    [SerializeField] private float velocidadMinimaDesactivar = 0.5f; // Velocidad m�nima para mantener la animaci�n

    private bool estaAbducida = false;

    private void Start()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.RegistrarOveja(this);

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        animator.SetTrigger("Aparecer");
    }

    private void Update()
    {
        if (rb != null)
        {
            // Activar animaci�n si sube con suficiente velocidad
            if (!estaAbducida && rb.linearVelocity.y > velocidadMinimaAbduccion)
            {
                ActivarAnimacionAbduccion();
            }
            // Desactivar animaci�n si la velocidad cae por debajo del umbral
            else if (estaAbducida && rb.linearVelocity.y < velocidadMinimaDesactivar)
            {
                DesactivarAnimacionAbduccion();
            }
        }
    }

    private void ActivarAnimacionAbduccion()
    {
        estaAbducida = true;
        animator.SetBool("Abducida", true);
        Debug.Log("Oveja siendo abducida!");
    }

    private void DesactivarAnimacionAbduccion()
    {
        estaAbducida = false;
        animator.SetBool("Abducida", false);
        Debug.Log("Oveja ya no est� siendo abducida");
    }

    public void Morir()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.EliminarOveja(this);
        Destroy(gameObject);
    }

    // Este m�todo puede ser llamado manualmente si lo necesitas
    public void Abducida()
    {
        ActivarAnimacionAbduccion();
    }
}