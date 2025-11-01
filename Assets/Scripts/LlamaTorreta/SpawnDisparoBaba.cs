using UnityEngine;

public class SpawnDisparoBaba : MonoBehaviour
{
    public GameObject proyectilPrefab;
    public Transform spawnPosition;
    public float velocity;
    public float algulo;


    public void Disparar()
    {
        GameObject proj = Instantiate(proyectilPrefab, spawnPosition.position, Quaternion.identity);

        Vector2 direccion = new Vector2(1f, Mathf.Tan(algulo * Mathf.Deg2Rad)).normalized; ;

        direccion.x *= Mathf.Sign(transform.parent.localScale.x);

        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
        rb.linearVelocity = direccion * velocity;
    }
}
