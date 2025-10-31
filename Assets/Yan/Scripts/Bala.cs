using UnityEngine;

public class Bala : MonoBehaviour
{

    [SerializeField] public int daño;
    [SerializeField] public float velocidad;
    [SerializeField] public float tiempoVida;
    [SerializeField] private Vector2 direccionBala;
    public void SetDireccion()
    {
        direccionBala = direccionBala.normalized;
    }

    void Start()
    {
        Destroy(gameObject, tiempoVida);
    }
   
    // Update is called once per frame
    void Update()
    {
        transform.Translate(direccionBala * velocidad * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Alien"))
        {
            EnemyLife enemy = other.GetComponent<EnemyLife>();
            if (enemy != null)
            {
                enemy.RecibirDaño(daño);
            }
            Destroy(gameObject);
        }
    }

}