using System.Collections;
using UnityEngine;

public class EnemyBehaviour : Enemy
{
    [SerializeField] private float rangoAtaque;
    //[SerializeField] private Coroutine ataqueRutina;
    //[SerializeField] private GameObject Player;
    [SerializeField] private Animator animator;
    [SerializeField] private float esperaAtaque;
    [SerializeField] private float ataqueTiempo;
    [SerializeField] private float danioAlien = 15f;
    private GameObject objetivoCercano;
    private CircleCollider2D rangoDeteccion;
    

    public bool atacando;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Player = GameObject.FindWithTag("Player");
        animator = GetComponent<Animator>();
        rangoDeteccion = gameObject.AddComponent<CircleCollider2D>();
        rangoDeteccion.isTrigger = true;
        rangoDeteccion.radius = rangoAtaque;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Oveja"))
        {
            IDaniable objetoDaniable = collision.GetComponent<IDaniable>();

            if (objetoDaniable != null)
            {
                objetoDaniable.RecibirDanio(danioAlien);
                EmpezarAtaque();
                Debug.Log($"Daño aplicado a {collision.gameObject.name}");
            }
        }
    }

    void EmpezarAtaque()
    {
        atacando = true;
        animator.SetTrigger("atacando");
        Invoke("terminarAtaque", ataqueTiempo);
    }

    void terminarAtaque()
    {
        atacando = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoAtaque);
    }


}
