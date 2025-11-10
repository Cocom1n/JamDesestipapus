using UnityEngine;
using TMPro; // Si quieres mostrar UI

public class ResureccionPachamama : MonoBehaviour
{
    [SerializeField] private GameObject rufinoPrefab;
    [SerializeField] private GameObject bizcoPrefab;
    [SerializeField] private MultiplayerCamera camarita;
    private int pagoPorResureccion = 2; // Cantidad de zanahorias necesarias
   
    private bool jugadorDentro;

    void Start()
    {
    }

    void Update()
    {
        if (jugadorDentro && Input.GetKeyDown(KeyCode.E))
        {
            IntentarRevivir();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorDentro = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorDentro = false;
        }
    }

    private void IntentarRevivir()
    {

        // Verificar si algún jugador está muerto
        bool rufinoEstaVivo = (FindFirstObjectByType<SoyRufino>() != null);
        bool bizcoEstaVivo = (FindFirstObjectByType<SoyBizco>() != null);

        if (rufinoEstaVivo && bizcoEstaVivo)
        {
            //se podria mostrar un textito en pantalla o que la pachamama se tire algun dialogo sjsjs
            Debug.Log("Ambos jugadores están vivos. No hay nadie que revivir.");
            return;
        }

        int comidaDisponible = GameManager.Instance.GetCantidadComida();

        if (comidaDisponible >= pagoPorResureccion)
        {
            // Consumir la comida
            if (comidaDisponible>=pagoPorResureccion)
            {
                GameManager.Instance.ConsumirComida(pagoPorResureccion);
                RevivirJugador(rufinoEstaVivo, bizcoEstaVivo);
            }
        }
        else
        {
            Debug.Log($"No tienes suficiente comida. Necesitas: {pagoPorResureccion}, Tienes: {comidaDisponible}");
        }
    }

    private void RevivirJugador(bool rufinoYaEstaVivo, bool bizcoYaEstaVivo)
    {
        Vector3 posicionRevivir = transform.position + Vector3.up * 2f;

        // Revivir Rufino si está muerto
        if (!rufinoYaEstaVivo)
        {
            Instantiate(rufinoPrefab, posicionRevivir, Quaternion.identity);
            Debug.Log("¡Rufino ha sido revivido!");
        }

        // Revivir Bizco si está muerto
        if (!bizcoYaEstaVivo)
        {
            Instantiate(bizcoPrefab, posicionRevivir, Quaternion.identity);
            Debug.Log("¡Bizco ha sido revivido!");
        }
        camarita.ActualizarReferenciasDeJugadores();
    }


}