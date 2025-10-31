using UnityEngine;

public class EnemyLife : MonoBehaviour
{
    [SerializeField] public int maxVida;
    [SerializeField] private int balasRecibidas;

    public void RecibirDaño(int daño)
    {
        balasRecibidas += daño;
        Debug.Log("Balas recibidas: " + balasRecibidas);

        if (balasRecibidas >= maxVida)
        {
            Morir();
        }
    }

    void Morir()
    {
        Debug.Log("Alien destruido");
        Destroy(gameObject);
    }


}
