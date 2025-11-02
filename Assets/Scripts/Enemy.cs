using UnityEngine;

// Clase que representa a todos los enemigos del juego

public class Enemy : MonoBehaviour
{
    [SerializeField] private string myName;
    [SerializeField] private float vida = 100f;

    public string Name => gameObject.name;

    public void RecibirDanio(int danio)
    {
        vida -= danio;
        Debug.Log($"{Name} recibe {danio} p de daño. Vida actual: {vida}");

        if (vida <= 0)
        {
            Morir();
        }
    }

    public void Empujar(Vector3 fuerza)
    {
        transform.position += fuerza;
        Debug.Log($"{Name} es empujado ");
    }

    public void Morir()
    {
        Debug.Log($"{Name} ha muelto pipip");
        Destroy(gameObject);
    }
}
