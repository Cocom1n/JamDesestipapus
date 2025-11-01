using UnityEngine;

public class EnemyLife : MonoBehaviour, IDaniable, IMorir
{
    [SerializeField] public float maxVida;
    private float vidaActual;

    public void Start()
    {
        vidaActual = maxVida;
    }
    public void RecibirDanio(float daño)
    {
        vidaActual-=daño;
        if(vidaActual<=0)
        {
            Morir();
        }
    }

    public void Morir()
    {
        Debug.Log("Alien destruido");
        Destroy(gameObject);
    }


}
