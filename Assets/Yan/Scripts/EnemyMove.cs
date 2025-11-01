using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float velocidadMovimiento;
    [SerializeField] private EnemyBehaviour enemyBehaviour;
    [SerializeField] private float centroMapa = 0f;

    private float direccion;


    private void Start()
    {
       enemyBehaviour = GetComponent<EnemyBehaviour>();
        DetectarDireccion();
    }
    private void FixedUpdate()
    {
        if (enemyBehaviour != null && enemyBehaviour.atacando)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        rb.linearVelocity = new Vector2(velocidadMovimiento * direccion, rb.linearVelocity.y);
        //DetectarBordePantalla();
    }
    /*
    private void DetectarBordePantalla()
    {
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        

        if ((pos.x < 0.05f || pos.x > 0.95f))
        {
            Girar();
            velocidadMovimiento *= -1;
            enBorde = true;
        }

        if (pos.x >= 0.05f && pos.x <= 0.95f)
        {
            enBorde = false;
            
        }
    }*/

    private void Girar()
    {
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }

    private void DetectarDireccion()
    { 
        if(transform.position.x < centroMapa)
        {
            direccion = 1f;
        }
        else
        {
            direccion = -1f;
            //girarSprie
        }
    }

}
