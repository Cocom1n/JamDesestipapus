
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Envestida : MonoBehaviour
{
    [Header("Configuración de Embestida")]
    [SerializeField] private float velocidad = 15f;
    [SerializeField] private float duracion = 0.2f;
    [SerializeField] private float Cooldown = 1f;
    [SerializeField] private float radioDeteccion = 0.6f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Empuje de Enemigos")]
    [SerializeField] private float fuerzaEmpujeHorizontal = 15f; // ⭐ Fuerza lateral
    [SerializeField] private float fuerzaEmpujeVertical = 2f;    // ⭐ Fuerza hacia arriba (menor)
    [SerializeField] private float stunDuration = 0.25f;         // Tiempo que deshabilita movimiento del enemigo

    [Header("Invencibilidad")]
    [SerializeField] private float duracionInvencibilidad = 0.5f; // ⭐ Tiempo extra sin colisión

    [Header("Layers para Colisión")]
    [SerializeField] private string playerLayerName = "Player";
    [SerializeField] private string enemyLayerName = "Enemy";

    [Header("Referencias (Opcional)")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;
    private bool isDashing = false;
    private float tiempoRestante;
    private float timerCooldown;
    private int direccionEmbestida;
    private PlayerMovement playerMovement;
    private Animator animatorController;

    private int playerLayer;
    private int enemyLayerInt;
    private Coroutine invencibilidadCoroutine;

    // Evitar múltiples aplicaciones en la misma embestida
    private HashSet<int> enemigosGolpeados = new HashSet<int>();
    // Para controlar coroutines de reactivación por enemigo
    private Dictionary<int, Coroutine> reenableCoroutines = new Dictionary<int, Coroutine>();

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
        animatorController = GetComponentInChildren<Animator>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer == null)
            Debug.LogWarning("No se encontró SpriteRenderer en " + gameObject.name);

        playerLayer = LayerMask.NameToLayer(playerLayerName);
        enemyLayerInt = LayerMask.NameToLayer(enemyLayerName);

        if (playerLayer == -1)
            Debug.LogError($"⚠️ Layer '{playerLayerName}' no existe!");

        if (enemyLayerInt == -1)
            Debug.LogError($"⚠️ Layer '{enemyLayerName}' no existe!");
    }

    void Update()
    {
        if (timerCooldown > 0)
            timerCooldown -= Time.deltaTime;

        if (isDashing || timerCooldown > 0)
            return;

        if (Input.GetKeyDown(KeyCode.G))
            Envestir(-1);
        else if (Input.GetKeyDown(KeyCode.J))
            Envestir(1);
    }

    void FixedUpdate()
    {
        if (isDashing)
        {
            rb.linearVelocity = new Vector2(direccionEmbestida * velocidad, rb.linearVelocity.y);
            tiempoRestante -= Time.fixedDeltaTime;
            DetectarYDañarEnemigos();

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

        if (playerMovement != null)
            playerMovement.enabled = false;

        // Limpiar tracking de esta embestida
        enemigosGolpeados.Clear();

        if (invencibilidadCoroutine != null)
            StopCoroutine(invencibilidadCoroutine);

        invencibilidadCoroutine = StartCoroutine(InvencibilidadTemporal());

        VoltearSprite(direction);

        if (animatorController != null)
            animatorController.SetTrigger("Embestida");
    }

    void Detenerse()
    {
        isDashing = false;
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        if (playerMovement != null)
            playerMovement.enabled = true;

        // NO reactivamos colisiones aquí; la coroutine lo hace
    }

    IEnumerator InvencibilidadTemporal()
    {
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayerInt, true);
        Debug.Log("💨 Invencibilidad ACTIVADA");

        yield return new WaitForSeconds(duracionInvencibilidad);

        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayerInt, false);
        Debug.Log("✋ Invencibilidad DESACTIVADA");

        invencibilidadCoroutine = null;
    }

    void VoltearSprite(int direccion)
    {
        if (spriteRenderer == null) return;
        spriteRenderer.flipX = direccion < 0;
    }

    void DetectarYDañarEnemigos()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radioDeteccion, enemyLayer);

        foreach (Collider2D hit in hits)
        {
            int id = hit.gameObject.GetInstanceID();
            if (enemigosGolpeados.Contains(id))
                continue; // ya golpeado en esta embestida

            enemigosGolpeados.Add(id);

            Debug.Log("💥 ¡Golpeó a: " + hit.gameObject.name + "!");

            Rigidbody2D enemyRb = hit.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
            {
                Vector2 desired = new Vector2(direccionEmbestida * fuerzaEmpujeHorizontal, fuerzaEmpujeVertical);

                // Aplicar velocity directamente para efecto inmediato
                enemyRb.linearVelocity = desired;
                Debug.Log($"Velocidad aplicada al enemigo: {desired}");

                // Usar la interfaz si está presente
                IDesactivarMovimiento mover = hit.GetComponent<IDesactivarMovimiento>();
                if (mover != null)
                {
                    mover.DesactivarMovimiento(stunDuration);
                }
                else
                {
                    // fallback: si no implementa la interfaz, intentar buscar EnemyMove y desactivar (retrocompatibilidad)
                    EnemyMove em = hit.GetComponent<EnemyMove>();
                    if (em != null)
                    {
                        em.DesactivarMovimiento(stunDuration);
                    }
                }
            }
            else
            {
                Debug.LogWarning("No Rigidbody2D en enemigo: " + hit.gameObject.name);
            }

            // Aplicar daño (una sola vez por embestida)
            IDaniable daniable = hit.GetComponent<IDaniable>();
            if (daniable != null)
            {
                daniable.RecibirDanio(1f);
            }
        }
    }

    IEnumerator RestaurarMovimientoTemporal(EnemyMove em, int id, float delay)
    {
        yield return new WaitForSeconds(delay);

        // Si el enemy fue destruido, em será null; controlarlo
        if (em != null)
            em.enabled = true;

        // Limpiar el diccionario
        if (reenableCoroutines.ContainsKey(id))
            reenableCoroutines.Remove(id);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = isDashing ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radioDeteccion);

        if (isDashing)
        {
            Gizmos.color = Color.cyan;
            Vector3 direccion = new Vector3(direccionEmbestida, 0, 0);
            Gizmos.DrawRay(transform.position, direccion * 1.5f);
        }
    }

    public bool EstaDashing() => isDashing;
    public float GetCooldownRestante() => timerCooldown;
    public bool PuedeEnvestir() => !isDashing && timerCooldown <= 0f;
}