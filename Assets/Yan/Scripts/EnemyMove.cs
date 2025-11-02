
using System.Collections;
using UnityEngine;

public class EnemyMove : MonoBehaviour, IDesactivarMovimiento
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float velocidadMovimiento;
    [SerializeField] private EnemyBehaviour enemyBehaviour;
    [SerializeField] private float centroMapa = 0f;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private float direccion;
    private bool aturdido = false;
    private Coroutine reenableCoroutine;

    private void Start()
    {
        // inicializaciones existentes...
        DetectarDireccion();
    }

    private void FixedUpdate()
    {
        // Si está ejecutando acción de ataque, mantener comportamiento actual
        if (enemyBehaviour != null && enemyBehaviour.atacando)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // Si está aturdido no sobrescribimos la velocidad X, dejamos que la física (o quien la haya puesto) la mantenga.
        if (aturdido)
        {
            // Opcional: puedes frenar la X si quieres: rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }

        rb.linearVelocity = new Vector2(velocidadMovimiento * direccion, rb.linearVelocity.y);
    }

    private void Girar()
    {
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }

    private void DetectarDireccion()
    {
        if (transform.position.x < centroMapa)
            direccion = 1f;
        else
            direccion = -1f;
    }

    public void CambiarDireccion()
    {
        direccion *= -1f;
        Girar();
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
}