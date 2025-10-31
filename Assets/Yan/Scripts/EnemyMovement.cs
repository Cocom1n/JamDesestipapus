using Unity.VisualScripting;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float velMovimiento;
    [SerializeField] private float distancia;
    [SerializeField] private LayerMask piso;
    [SerializeField] private bool borde = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 nuevaPosicion = rb.position + Vector2.right * velMovimiento * Time.fixedDeltaTime;

        rb.MovePosition(nuevaPosicion);

        Vector2 origenRaycast = transform.position + transform.right * 0.01f;

        bool noPiso = !Physics2D.Raycast(origenRaycast, Vector2.down, distancia, piso);

        if (noPiso && !borde)
        {
            Girar();
            borde = true;
        }
        else if (!noPiso)
        {
            borde = false;
        }

    }

    private void Girar()
    {
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + 180, 0);
    }

    private void OnDrawGizmos()
    {
        Vector3 origenRaycast = transform.position + transform.right * 0.01f;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(origenRaycast, origenRaycast + Vector3.down * distancia);
    }
}

