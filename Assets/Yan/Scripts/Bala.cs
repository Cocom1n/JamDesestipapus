using UnityEngine;

public class Bala : MonoBehaviour
{
    [Header("Configuración de Daño")]
    [SerializeField] private float danio = 10f;
    [SerializeField] private bool destruirAlImpactar = true;

    [Header("Tiempo de Vida")]
    [SerializeField] private float tiempoDeVida = 5f;

    private void Start()
    {
        // Destruir la bala después de un tiempo para no saturar la escena
        Destroy(gameObject, tiempoDeVida);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Intentar obtener el componente IDaniable
        IDaniable objetoDaniable = collision.GetComponent<IDaniable>();

        if (objetoDaniable != null)
        {
            objetoDaniable.RecibirDanio(danio);
            Debug.Log($"Daño aplicado a {collision.gameObject.name}");

            if (destruirAlImpactar)
            {
                Destroy(gameObject);
            }
        }
        if (collision.CompareTag("Piso"))
        {
            Destroy(gameObject);
        }
    }

    // Alternativa usando OnCollisionEnter2D si usas colliders normales
    /*
    private void OnCollisionEnter2D(Collision2D collision)
    {
        IDaniable objetoDaniable = collision.gameObject.GetComponent<IDaniable>();

        if (objetoDaniable != null)
        {
            objetoDaniable.RecibirDanio(danio);
            Debug.Log($"Daño aplicado a {collision.gameObject.name}");
        }

        if (destruirAlImpactar)
        {
            Destroy(gameObject);
        }
    }*/
}