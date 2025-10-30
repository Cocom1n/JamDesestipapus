using UnityEngine;

public class Envestida : MonoBehaviour
{
    [SerializeField] private float velocidad = 15f;
    [SerializeField] private float duracion = 0.2f;
    [SerializeField] private float Cooldown = 1f;
    [SerializeField] private LayerMask enemyLayer;

    private Rigidbody2D rb;
    private bool isDashing = false;
    private float tiempoRestante;
    private float timerCooldown;
    private int teclasDireccion;

    private PlayerMovement playerMovement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
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
            rb.linearVelocity = new Vector2(teclasDireccion * velocidad, rb.linearVelocity.y);
            tiempoRestante -= Time.fixedDeltaTime;

            EmpujarEnemigos();

            if (tiempoRestante <= 0f)
                Detenerse();
        }
    }

    void Envestir(int direction)
    {
        isDashing = true;
        teclasDireccion = direction;
        tiempoRestante = duracion;
        timerCooldown = Cooldown;

        if (playerMovement != null)
            playerMovement.enabled = false;
    }

    void Detenerse() //detiene el dash y vuelve a funcionar el script de movimiento normal
    {
        isDashing = false;

        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        if (playerMovement != null)
            playerMovement.enabled = true;
    }

    void EmpujarEnemigos()
    {
        float detectionRadius = 0.6f;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius, enemyLayer);

        foreach (Collider2D hit in hits)
        {
            Debug.Log("doblo y choclo");
            //cuando colicione agregar en el enemigo que se mueva nose
            //agregar el daño a los aliensitos aqui tmb papus
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.6f);
    }
}
