using System;
using System.Collections;
using UnityEngine;

public class Jefe : MonoBehaviour
{
    [SerializeField] Transform jugador;
    [SerializeField] Animator animator;

    Rigidbody2D rb;
    bool mirandoDer = true;

    [Header("Vida")]
    [SerializeField] float vidaJefe = 200f;
    float vidaActual;

    [Header("Carga")]
    [SerializeField] float velocidadCarga = 12f;
    [SerializeField] float tiempoEntreCargas = 2f;
    bool estaCargando = false;

    [SerializeField] float fuerzaImpulsoJugador = 10f;
    CaidaPicos caidaPicos;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        vidaActual = vidaJefe;
        StartCoroutine(CicloDeAtaque());
        caidaPicos = FindObjectOfType<CaidaPicos>();
    }

    IEnumerator CicloDeAtaque()
    {
        while (vidaActual > 0)
        {
            yield return new WaitForSeconds(tiempoEntreCargas);
            yield return CargarHastaLimite();
        }
    }
    IEnumerator CargarHastaLimite()
    {
        if (jugador == null) yield break;

        MirarDerecha();

        estaCargando = true;

        float direccionX = Mathf.Sign(jugador.position.x - transform.position.x);
        float altura = transform.position.y;

        if (animator != null) 
        {
            //maru agrega la animacion de carga aqui 
        }

        while (estaCargando)
        {
            rb.linearVelocity = new Vector2(direccionX * velocidadCarga, rb.linearVelocity.y);
            transform.position = new Vector3(transform.position.x, altura, transform.position.z);
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
    }

    public void MirarDerecha()
    {
        if (jugador == null) return;

        if ((jugador.position.x > transform.position.x && !mirandoDer) ||
            (jugador.position.x < transform.position.x && mirandoDer))
        {
            mirandoDer = !mirandoDer;
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + 180, 0);
        }
    }

    public void RecibirDanio(float cantidad)
    {
        vidaActual -= cantidad;
        if (vidaActual <= 0f)
        {
            Debug.Log("Muerte del jefe");
            enabled = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.transform.CompareTag("Player"))
        {
            Debug.Log("jugador perdio vida");

            IDaniable daniable = col.transform.GetComponent<IDaniable>();
            if (daniable != null)
            {
                daniable.RecibirDanio(20);
            }

            Rigidbody2D rbJugador = col.transform.GetComponent<Rigidbody2D>();
            if (rbJugador != null)
            {
                rbJugador.linearVelocity = new Vector2(rbJugador.linearVelocity.x, 0); 
                rbJugador.AddForce(Vector2.up * fuerzaImpulsoJugador, ForceMode2D.Impulse);
                //aqui tambien agregar animacion de salto del jugador
            }
        }
        else if (col.transform.CompareTag("LimiteMapa"))
        {
            estaCargando = false;
            if (caidaPicos != null)
            {
                caidaPicos.ActivarCaidaPicos();
            }
            Debug.Log("reproducir evento");
        }
    }
}