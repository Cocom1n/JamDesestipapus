using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Config pj")]
    [SerializeField] public string teclas;
    [SerializeField] private float speed = 5.0f;  // Velocidad máxima
    [SerializeField] private float acceleration = 10.0f;  // Qué tan rápido alcanza la velocidad máxima
    [SerializeField] private float deceleration = 10.0f;  // Qué tan rápido se detiene

    private Rigidbody2D rb;
    private Vector2 movementInput;
    private Vector2 currentVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Conseguimos el Rigidbody2D del jugador
    }

    void Update()
    {
        // Obtener entrada del jugador
        float horizontalInput = Input.GetAxis(teclas);
        //float horizontalInput = Input.GetAxis("Horizontal");
        Move(horizontalInput);

        // Almacenar la dirección del movimiento
        movementInput = new Vector2(horizontalInput, 0).normalized;
    }

    void Move(float horizontalInput)
    {
        // Si hay movimiento de entrada, aceleramos hacia la velocidad deseada
        if (movementInput != Vector2.zero)
        {
            currentVelocity = Vector2.MoveTowards(currentVelocity, movementInput * speed, acceleration * Time.deltaTime);
        }
        else
        {
            // Si no hay entrada, desaceleramos
            currentVelocity = Vector2.MoveTowards(currentVelocity, Vector2.zero, deceleration * Time.deltaTime);
        }

        // Aplicar la velocidad horizontal al Rigidbody2D (sin afectar la velocidad en Y)
        rb.linearVelocity = new Vector2(currentVelocity.x, rb.linearVelocity.y);
    }
}
