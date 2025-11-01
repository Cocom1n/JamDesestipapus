using UnityEngine;

public class RecogerObjeto : MonoBehaviour
{
    [Header("area de deteccion")]
    [SerializeField] private Vector2 cuadradoTam = new Vector2(2f, 3f);
    [SerializeField] private float fuerzaAtraccion = 8f;
    [SerializeField] private LayerMask layerObjeto;

    private Rigidbody2D objetoDetectado;
    private bool detectando = false;

    public bool Detectando => detectando;

    private void FixedUpdate()
    {
        AtraerObjeto();
    }

    private void AtraerObjeto()
    {
        Vector2 boxCenter = (Vector2)transform.position + Vector2.down * (cuadradoTam.y / 2);
        Collider2D[] colliders = Physics2D.OverlapBoxAll(boxCenter, cuadradoTam, 0f, layerObjeto);

        if (colliders.Length > 0)
        {
            detectando = true;

            // Atraer TODOS los objetos detectados (por si hay varios)
            foreach (Collider2D col in colliders)
            {
                Rigidbody2D rb = col.attachedRigidbody;
                if (rb != null)
                {
                    // Aplicar fuerza hacia arriba (rayo tractor)
                    rb.AddForce(Vector2.up * fuerzaAtraccion, ForceMode2D.Force);
                }
            }
        }
        else
        {
            detectando = false;
            objetoDetectado = null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 boxCenter = (Vector2)transform.position + Vector2.down * (cuadradoTam.y / 2);
        Gizmos.color = detectando ? Color.green : Color.cyan;
        Gizmos.DrawWireCube(boxCenter, cuadradoTam);
    }
}