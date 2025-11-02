using UnityEngine;

public class LazoProjectile : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float velocidadAtraccion = 8f;
    [SerializeField] private float distanciaCaptura = 0.5f;
    [SerializeField] private float tiempoVidaMaximo = 3f; // 🆕 Tiempo antes de autodestruirse

    [Header("Visual")]
    [SerializeField] private LineRenderer cuerda;
    [SerializeField] private Color colorCuerda = Color.yellow;
    [SerializeField] private float anchoCuerda = 0.05f;

    private Vector2 direccion;
    private float velocidad;
    private float rangoMaximo;
    private Transform jugador;
    private DisparoLazo disparoLazoRef; // 🆕 Referencia al script que lo creó
    private Rigidbody2D rb;

    private GameObject ovejaCapturada;
    private bool haCapturado = false;
    private Vector3 puntoInicial;
    private float distanciaRecorrida = 0f;
    private float tiempoVida = 0f; // 🆕 Contador de tiempo

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        ConfigurarLineRenderer();
    }

    void ConfigurarLineRenderer()
    {
        if (cuerda == null)
        {
            cuerda = gameObject.AddComponent<LineRenderer>();
        }

        cuerda.startWidth = anchoCuerda;
        cuerda.endWidth = anchoCuerda;
        cuerda.material = new Material(Shader.Find("Sprites/Default"));
        cuerda.startColor = colorCuerda;
        cuerda.endColor = colorCuerda;
        cuerda.sortingOrder = 10;
    }

    public void Inicializar(Vector2 dir, float vel, float rango, Transform player, DisparoLazo disparoLazo)
    {
        direccion = dir;
        velocidad = vel;
        rangoMaximo = rango;
        jugador = player;
        disparoLazoRef = disparoLazo;
        puntoInicial = transform.position;

        rb.linearVelocity = direccion * velocidad;
    }

    void Update()
    {
        // 🆕 Incrementar tiempo de vida
        tiempoVida += Time.deltaTime;

        // 🆕 Autodestruirse después del tiempo máximo
        if (tiempoVida >= tiempoVidaMaximo)
        {
            Debug.Log("⏱️ Lazo expiró por tiempo");
            Destruir();
            return;
        }

        // Calcular distancia recorrida
        distanciaRecorrida = Vector3.Distance(puntoInicial, transform.position);

        // Si excede el rango máximo y no capturó nada, destruir
        if (distanciaRecorrida >= rangoMaximo && !haCapturado)
        {
            Debug.Log("📏 Lazo alcanzó rango máximo");
            Destruir();
            return;
        }

        // Si capturó una oveja, atraerla
        if (haCapturado && ovejaCapturada != null)
        {
            AtraerOveja();
            DibujarCuerda();

            // Si la oveja llegó al jugador, soltarla
            float distancia = Vector3.Distance(ovejaCapturada.transform.position, jugador.position);
            if (distancia <= distanciaCaptura)
            {
                SoltarOveja();
                Destruir();
            }
        }
        else if (haCapturado && ovejaCapturada == null)
        {
            // 🆕 Si la oveja fue destruida mientras era capturada
            Debug.Log("⚠️ La oveja capturada fue destruida");
            Destruir();
        }
        else
        {
            DibujarCuerda();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Solo capturar si no ha capturado nada aún
        if (haCapturado) return;

        // Verificar si es una oveja
        if (collision.CompareTag("Oveja"))
        {
            CapturarOveja(collision.gameObject);
        }
    }

    void CapturarOveja(GameObject oveja)
    {
        ovejaCapturada = oveja;
        haCapturado = true;

        // Detener el movimiento del lazo
        rb.linearVelocity = Vector2.zero;

        // 🆕 Desactivar el movimiento de la oveja si tiene script
        EnemyMove enemyMove = oveja.GetComponent<EnemyMove>();
        if (enemyMove != null)
        {
            enemyMove.enabled = false;
        }

        Debug.Log($"🐑 ¡Capturaste a {oveja.name}!");
    }

    void AtraerOveja()
    {
        if (ovejaCapturada == null || jugador == null) return;

        // Dirección hacia el jugador
        Vector2 direccionJugador = (jugador.position - ovejaCapturada.transform.position).normalized;

        // Mover la oveja hacia el jugador
        Rigidbody2D ovejaRb = ovejaCapturada.GetComponent<Rigidbody2D>();
        if (ovejaRb != null)
        {
            ovejaRb.linearVelocity = direccionJugador * velocidadAtraccion;
        }
        else
        {
            // Si no tiene Rigidbody, mover directamente
            ovejaCapturada.transform.position = Vector3.MoveTowards(
                ovejaCapturada.transform.position,
                jugador.position,
                velocidadAtraccion * Time.deltaTime
            );
        }

        // Mover el lazo junto con la oveja
        transform.position = ovejaCapturada.transform.position;
    }

    void SoltarOveja()
    {
        if (ovejaCapturada == null) return;

        // Detener la oveja
        Rigidbody2D ovejaRb = ovejaCapturada.GetComponent<Rigidbody2D>();
        if (ovejaRb != null)
        {
            ovejaRb.linearVelocity = Vector2.zero;
        }

        // 🆕 Reactivar el movimiento de la oveja
        EnemyMove enemyMove = ovejaCapturada.GetComponent<EnemyMove>();
        if (enemyMove != null)
        {
            enemyMove.enabled = true;
        }

        Debug.Log($"🐑 Soltaste a {ovejaCapturada.name}");
        ovejaCapturada = null;
    }

    void DibujarCuerda()
    {
        if (cuerda == null || jugador == null) return;

        cuerda.positionCount = 2;
        cuerda.SetPosition(0, jugador.position);
        cuerda.SetPosition(1, transform.position);
    }

    void Destruir()
    {
        // Soltar oveja si estaba capturada
        if (ovejaCapturada != null)
        {
            SoltarOveja();
        }

        // 🆕 Notificar al DisparoLazo que este lazo fue destruido
        if (disparoLazoRef != null)
        {
            disparoLazoRef.OnLazoDestruido();
        }

        Destroy(gameObject);
    }

    void OnDestroy()
    {
        // 🆕 Por si el objeto se destruye de otra forma (ej: cambio de escena)
        if (disparoLazoRef != null && ovejaCapturada != null)
        {
            SoltarOveja();
            disparoLazoRef.OnLazoDestruido();
        }
    }
}