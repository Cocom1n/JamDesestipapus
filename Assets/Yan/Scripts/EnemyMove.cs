using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float velocidadMovimiento;
    [SerializeField] private bool enBorde = false;
    [SerializeField] private EnemyBehaviour enemyBehaviour;

    private void Start()
    {
       enemyBehaviour = GetComponent<EnemyBehaviour>();
    }
    private void FixedUpdate()
    {
        if (enemyBehaviour != null && enemyBehaviour.atacando)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        rb.linearVelocity = new Vector2(velocidadMovimiento, rb.linearVelocity.y);
        DetectarBordePantalla();
    }

    private void DetectarBordePantalla()
    {
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);

        if ((pos.x < 0.05f || pos.x > 0.95f) && !enBorde)
        {
            Girar();
            velocidadMovimiento *= -1;
            enBorde = true;
        }

        if (pos.x >= 0.05f && pos.x <= 0.95f)
        {
            enBorde = false;
            
        }
    }

    private void Girar()
    {
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }

}
