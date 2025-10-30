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
        Move(horizontalInput);

        // Almacenar la dirección del movimiento
        movementInput = new Vector2(horizontalInput, 0).normalized;
    }

    void Move(float horizontalInput)
    {
        Vector2 targetVelocity = movementInput * speed;
        float lerpFactor = 1f - Mathf.Exp(-acceleration * Time.deltaTime);

        currentVelocity = Vector2.Lerp(currentVelocity, targetVelocity, lerpFactor);

        
        if (movementInput == Vector2.zero) //si no se preciona ninguna tecla lo hace frenar hasta q se quede quietesito
        {
            float decelFactor = 1f - Mathf.Exp(-deceleration * Time.deltaTime);
            currentVelocity = Vector2.Lerp(currentVelocity, Vector2.zero, decelFactor);
        }

        rb.linearVelocity = new Vector2(currentVelocity.x, rb.linearVelocity.y);

    }
}
