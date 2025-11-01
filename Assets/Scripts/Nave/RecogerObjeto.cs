using UnityEngine;

public class RecogerObjeto : MonoBehaviour
{
    [Header("Configuración del área de detección")]
    [SerializeField] private Vector2 cuadradoTam = new Vector2(2f, 1f);
    [SerializeField] private float fuerza = 5f;
    [SerializeField] private LayerMask layerObjeto;

    [Header("Animación")]
    private Animator animator;
    private bool estaLevantando = false;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void FixedUpdate()
    {
        LevantarObjeto();
    }

    private void LevantarObjeto()
    {
        Vector2 boxCenter = (Vector2)transform.position + Vector2.down * (cuadradoTam.y / 2);
        Collider2D[] colliders = Physics2D.OverlapBoxAll(boxCenter, cuadradoTam, 0f, layerObjeto);

        if (colliders.Length > 0)
        {
            if (!estaLevantando)
            {
                animator.SetBool("Levantando", true);
                estaLevantando = true;
            }

            foreach (Collider2D col in colliders)
            {
                Rigidbody2D rb = col.attachedRigidbody;
                if (rb != null)
                {
                    rb.AddForce(Vector2.up * fuerza, ForceMode2D.Force);
                }
            }
        }
        else
        {
            if (estaLevantando)
            {
                animator.SetBool("Levantando", false);
                estaLevantando = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(collision.gameObject);
        GameManager.Instance.OvejaMuerta();
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 boxCenter = (Vector2)transform.position + Vector2.down * (cuadradoTam.y / 2);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(boxCenter, cuadradoTam);
    }
}
