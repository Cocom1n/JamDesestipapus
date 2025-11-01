using UnityEngine;

public class Baba : MonoBehaviour
{
    public int danio = 10;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Enemy>(out Enemy enemigo))
        {
            enemigo.RecibirDanio(danio);
            enemigo.Empujar(transform.forward * 2f);
            enemigo.Morir();
            Destroy(gameObject);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Piso"))
        {
            Destroy(gameObject);
        }
    }
}
