using UnityEngine;

public class EnemyLife : MonoBehaviour
{
    [SerializeField] public int maxVida;
    [SerializeField] private int balasRecibidas;

    public void RecibirDa�o(int da�o)
    {
        balasRecibidas += da�o;
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
