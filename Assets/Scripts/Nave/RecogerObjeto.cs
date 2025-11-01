using UnityEngine;
using UnityEngine.UIElements;

public class RecogerObjeto : MonoBehaviour
{
    [SerializeField] Vector2 cuadradoTam = new Vector2(2f, 1f);
    [SerializeField] float fuerza;
    [SerializeField] LayerMask layerObjeto;

    private void FixedUpdate()
    {
        LevantarObjeto();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(collision.gameObject);
        GameManager.Instance.OvejaMuerta();
    }
    void LevantarObjeto() 
    {
        Vector2 boxCenter = (Vector2)transform.position + Vector2.down * (cuadradoTam.y / 2);
        Collider2D[] colliders = Physics2D.OverlapBoxAll(boxCenter, cuadradoTam, 0f, layerObjeto);
        foreach (Collider2D col in colliders)
        {
            Rigidbody2D rb = col.attachedRigidbody;
            if (rb != null)
            {
                rb.AddForce(Vector2.up * fuerza, ForceMode2D.Force);
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Vector2 boxcenter = (Vector2)transform.position + Vector2.down * (cuadradoTam.y / 2);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(boxcenter, cuadradoTam);
    }
}