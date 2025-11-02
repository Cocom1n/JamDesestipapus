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
    [SerializeField] private float fuerzaEmpujeHorizontal = 15f;
    [SerializeField] private float fuerzaEmpujeVertical = 2f;
    [SerializeField] private float stunDuration = 0.25f;

    [Header("Empuje a Aliados (Player/Oveja)")]
    [SerializeField] private LayerMask aliadosLayer; // 🆕 Layer para Player y Oveja
    [SerializeField] private float fuerzaEmpujeAliadosHorizontal = 10f; // 🆕 Empuje a aliados
    [SerializeField] private float fuerzaEmpujeAliadosVertical = 5f;

    [Header("Invencibilidad")]
    [SerializeField] private float duracionInvencibilidad = 0.5f;

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

    private HashSet<int> enemigosGolpeados = new HashSet<int>();
    private HashSet<int> aliadosEmpujados = new HashSet<int>(); // 🆕 Para evitar empujar múltiples veces

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
            DetectarYEmpujarAliados(); // 🆕 Detectar y empujar aliados

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

        enemigosGolpeados.Clear();
        aliadosEmpujados.Clear(); // 🆕 Limpiar tracking de aliados

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
                continue;

            enemigosGolpeados.Add(id);
            Debug.Log("💥 ¡Golpeó a enemigo: " + hit.gameObject.name + "!");

            Rigidbody2D enemyRb = hit.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
            {
                Vector2 empuje = new Vector2(
                    direccionEmbestida * fuerzaEmpujeHorizontal,
                    fuerzaEmpujeVertical
                );
                enemyRb.linearVelocity = empuje;

                IDesactivarMovimiento mover = hit.GetComponent<IDesactivarMovimiento>();
                if (mover != null)
                {
                    mover.DesactivarMovimiento(stunDuration);
                }
                else
                {
                    EnemyMove em = hit.GetComponent<EnemyMove>();
                    if (em != null)
                    {
                        em.DesactivarMovimiento(stunDuration);
                    }
                }
            }

            IDaniable daniable = hit.GetComponent<IDaniable>();
            if (daniable != null)
            {
                daniable.RecibirDanio(1f);
            }
        }
    }

    // 🆕 NUEVA FUNCIÓN: Detectar y empujar Player/Oveja
    void DetectarYEmpujarAliados()
    {
        // Detectar objetos en el layer de aliados (Player y Oveja)
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radioDeteccion, aliadosLayer);

        foreach (Collider2D hit in hits)
        {
            // Ignorar al propio perro (el que está embistiendo)
            if (hit.gameObject == gameObject)
                continue;

            int id = hit.gameObject.GetInstanceID();
            if (aliadosEmpujados.Contains(id))
                continue;

            aliadosEmpujados.Add(id);
            Debug.Log("🐑 ¡Empujó a aliado: " + hit.gameObject.name + "!");

            Rigidbody2D aliadoRb = hit.GetComponent<Rigidbody2D>();
            if (aliadoRb != null)
            {
                // Empuje más suave para aliados
                Vector2 empuje = new Vector2(
                    direccionEmbestida * fuerzaEmpujeAliadosHorizontal,
                    fuerzaEmpujeAliadosVertical
                );

                aliadoRb.linearVelocity = empuje;
                Debug.Log($"🚀 Empuje a aliado aplicado: {empuje}");
            }
        }
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