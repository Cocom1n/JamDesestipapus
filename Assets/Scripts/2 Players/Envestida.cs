using UnityEngine;

public class Envestida : MonoBehaviour
{
    [Header("Configuración de Embestida")]
    [SerializeField] private float velocidad = 15f;
    [SerializeField] private float duracion = 0.2f;
    [SerializeField] private float Cooldown = 1f;
    [SerializeField] private float radioDeteccion = 0.6f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Referencias (Opcional)")]
    [SerializeField] private SpriteRenderer spriteRenderer; // Asigna si está en otro objeto

    private Rigidbody2D rb;
    private bool isDashing = false;
    private float tiempoRestante;
    private float timerCooldown;
    private int direccionEmbestida; // -1 izquierda, 1 derecha
    private PlayerMovement playerMovement;
    private Animator animatorController;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
        animatorController = GetComponentInChildren<Animator>();

        // Obtener SpriteRenderer automáticamente si no está asignado
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        // Si el SpriteRenderer está en un hijo
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer == null)
            Debug.LogWarning("No se encontró SpriteRenderer en " + gameObject.name);
    }

    void Update()
    {
        if (timerCooldown > 0)
            timerCooldown -= Time.deltaTime;

        if (isDashing || timerCooldown > 0)
            return;

        // Teclas para embestir
        if (Input.GetKeyDown(KeyCode.G))
            Envestir(-1); // Izquierda
        else if (Input.GetKeyDown(KeyCode.J))
            Envestir(1);  // Derecha
    }

    void FixedUpdate()
    {
        if (isDashing)
        {
            rb.linearVelocity = new Vector2(direccionEmbestida * velocidad, rb.linearVelocity.y);
            tiempoRestante -= Time.fixedDeltaTime;
            EmpujarEnemigos();

            if (tiempoRestante <= 0f)
                Detenerse();
        }
    }

    void Envestir(int direction)
    {
        isDashing = true;
        direccionEmbestida = direction;
        tiempoRestante = duracion;
        timerCooldown = Cooldown;

        // Desactivar movimiento normal
        if (playerMovement != null)
            playerMovement.enabled = false;

        // Voltear sprite según dirección
        VoltearSprite(direction);

        // Activar animación
        if (animatorController != null)
            animatorController.SetTrigger("Embestida");
    }

    void Detenerse()
    {
        isDashing = false;
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        // Reactivar movimiento normal
        if (playerMovement != null)
            playerMovement.enabled = true;
    }

    void VoltearSprite(int direccion)
    {
        if (spriteRenderer == null) return;

        // Voltear según dirección: -1 = izquierda (flipX true), 1 = derecha (flipX false)
        spriteRenderer.flipX = direccion < 0;

        // NOTA: Si tu sprite se voltea al revés, cambia a:
        // spriteRenderer.flipX = direccion > 0;
    }

    void EmpujarEnemigos()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radioDeteccion, enemyLayer);

        foreach (Collider2D hit in hits)
        {
            Debug.Log("¡Golpeó a: " + hit.gameObject.name + "!");

            // Empujar al enemigo
            Rigidbody2D enemyRb = hit.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
            {
                // Aplicar fuerza en la dirección de la embestida
                float fuerzaEmpuje = 10f; // Ajusta según prefieras
                enemyRb.AddForce(new Vector2(direccionEmbestida * fuerzaEmpuje, 2f), ForceMode2D.Impulse);
            }

            // Aplicar daño al enemigo
            IMorir enemigo = hit.GetComponent<IMorir>();
            if (enemigo != null)
            {
                enemigo.Morir();
            }

            // O si usas sistema de vida:
            // Vida vidaEnemigo = hit.GetComponent<Vida>();
            // if (vidaEnemigo != null)
            //     vidaEnemigo.RecibirDaño(1);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = isDashing ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radioDeteccion);

        // Dibujar dirección de la embestida
        if (isDashing)
        {
            Gizmos.color = Color.cyan;
            Vector3 direccion = new Vector3(direccionEmbestida, 0, 0);
            Gizmos.DrawRay(transform.position, direccion * 1.5f);
        }
    }

    // Getters útiles
    public bool EstaDashing() => isDashing;
    public float GetCooldownRestante() => timerCooldown;
    public bool PuedeEnvestir() => !isDashing && timerCooldown <= 0f;
}