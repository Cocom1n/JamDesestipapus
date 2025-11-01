using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Config PJ")]
    [SerializeField] public string teclas;
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float acceleration = 10.0f;
    [SerializeField] private float deceleration = 10.0f;

    [Header("Animaci�n")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float umbralMovimiento = 0.1f; // Velocidad m�nima para considerar que est� caminando

    private Rigidbody2D rb;
    private Vector2 movementInput;
    private Vector2 currentVelocity;

    // Nombres de par�metros del Animator (aj�stalos seg�n tus animaciones)
    private const string ANIM_SPEED = "Speed"; // Float para velocidad
    private const string ANIM_IS_WALKING = "IsWalking"; // Bool para caminar

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Obtener componentes autom�ticamente si no est�n asignados
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (animator == null)
            Debug.LogWarning("No se encontr� Animator en " + gameObject.name);

        if (spriteRenderer == null)
            Debug.LogWarning("No se encontr� SpriteRenderer en " + gameObject.name);
    }

    void Update()
    {
        // Obtener entrada del jugador
        float horizontalInput = Input.GetAxis(teclas);

        // Almacenar la direcci�n del movimiento
        movementInput = new Vector2(horizontalInput, 0).normalized;

        // Aplicar movimiento
        Move(horizontalInput);

        // Actualizar animaciones
        ActualizarAnimacion();

        // Voltear sprite seg�n direcci�n
        VoltearSprite();
    }

    void Move(float horizontalInput)
    {
        Vector2 targetVelocity = movementInput * speed;
        float lerpFactor = 1f - Mathf.Exp(-acceleration * Time.deltaTime);
        currentVelocity = Vector2.Lerp(currentVelocity, targetVelocity, lerpFactor);

        if (movementInput == Vector2.zero)
        {
            float decelFactor = 1f - Mathf.Exp(-deceleration * Time.deltaTime);
            currentVelocity = Vector2.Lerp(currentVelocity, Vector2.zero, decelFactor);
        }

        rb.linearVelocity = new Vector2(currentVelocity.x, rb.linearVelocity.y);
    }

    void ActualizarAnimacion()
    {
        if (animator == null) return;

        // Calcular velocidad horizontal absoluta
        float velocidadAbsoluta = Mathf.Abs(currentVelocity.x);

        // Activar/desactivar animaci�n de caminar
        bool estaCaminando = velocidadAbsoluta > umbralMovimiento;
        animator.SetBool(ANIM_IS_WALKING, estaCaminando);

        // Opcional: pasar la velocidad normalizada (0-1) al Animator
        float velocidadNormalizada = Mathf.Clamp01(velocidadAbsoluta / speed);
        animator.SetFloat(ANIM_SPEED, velocidadNormalizada);
    }

    void VoltearSprite()
    {
        if (spriteRenderer == null) return;

        // Solo voltear si hay movimiento significativo
        if (Mathf.Abs(currentVelocity.x) > umbralMovimiento)
        {
            // Si se mueve a la izquierda, voltear
            if (currentVelocity.x < 0)
                spriteRenderer.flipX = true;
            // Si se mueve a la derecha, no voltear
            else if (currentVelocity.x > 0)
                spriteRenderer.flipX = false;
        }
    }

    // M�todo opcional para forzar direcci�n del sprite (�til para otras mec�nicas)
    public void ForzarDireccion(bool mirarIzquierda)
    {
        if (spriteRenderer != null)
            spriteRenderer.flipX = mirarIzquierda;
    }

    // Getters �tiles para otros scripts
    public bool EstaCaminando() => Mathf.Abs(currentVelocity.x) > umbralMovimiento;
    public Vector2 GetVelocidadActual() => currentVelocity;
    public float GetDireccion() => Mathf.Sign(currentVelocity.x); // -1 izquierda, 1 derecha
}