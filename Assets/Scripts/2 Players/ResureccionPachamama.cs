using UnityEngine;
using TMPro; // Si quieres mostrar UI

public class ResureccionPachamama : MonoBehaviour
{
    [Header("Referencias Jugadores")]
    [SerializeField] private GameObject rufinoReal;
    [SerializeField] private GameObject bizcoReal;
    [SerializeField] private GameObject rufinoPrefab;
    [SerializeField] private GameObject bizcoPrefab;

    [Header("Configuración Resurrección")]
    [SerializeField] private int pagoPorResureccion = 10; // Cantidad de zanahorias necesarias
    /*
    [Header("UI (Opcional)")]
    [SerializeField] private GameObject textoInteraccion; // Panel o texto que aparece al acercarse
    [SerializeField] private TextMeshProUGUI textoInfo; // Texto que muestra la info
    */
    private bool jugadorDentro;
    private GameObject rufinoActual;
    private GameObject bizcoActual;

    void Start()
    {
        /*
        if (textoInteraccion != null)
            textoInteraccion.SetActive(false);*/
    }

    void Update()
    {
        if (jugadorDentro && Input.GetKeyDown(KeyCode.E))
        {
            IntentarRevivir();
        }

        ActualizarTextoUI();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorDentro = true;
            /*
            if (textoInteraccion != null)
                textoInteraccion.SetActive(true);*/
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorDentro = false;
            //if (textoInteraccion != null)
            //    textoInteraccion.SetActive(false);
        }
    }

    private void IntentarRevivir()
    {

        // Verificar si algún jugador está muerto
        bool hayJugadorMuerto = (rufinoReal == null || bizcoReal == null);

        if (!hayJugadorMuerto)
        {
            Debug.Log("Ambos jugadores están vivos. No hay nadie que revivir.");
            return;
        }

        // Verificar si hay suficiente comida
        int comidaDisponible = GameManager.Instance.GetCantidadComida();

        if (comidaDisponible >= pagoPorResureccion)
        {
            // Consumir la comida
            if (comidaDisponible>=pagoPorResureccion)
            {
                GameManager.Instance.ConsumirComida(pagoPorResureccion);
                RevivirJugador();
            }
        }
        else
        {
            Debug.Log($"No tienes suficiente comida. Necesitas: {pagoPorResureccion}, Tienes: {comidaDisponible}");
        }
    }

    private void RevivirJugador()
    {
        Vector3 posicionRevivir = transform.position + Vector3.up * 2f;

        // Revivir Rufino si está muerto
        if (rufinoReal == null)
        {
            Instantiate(rufinoPrefab, posicionRevivir, Quaternion.identity);
            Debug.Log("¡Rufino ha sido revivido!");
        }

        // Revivir Bizco si está muerto
        if (bizcoReal == null)
        {
            Instantiate(bizcoPrefab, posicionRevivir, Quaternion.identity);
            Debug.Log("¡Bizco ha sido revivido!");
        }
    }

    private void ActualizarTextoUI()
    {
        if (/*textoInfo != null && */jugadorDentro)
        {
            if (GameManager.Instance != null)
            {
                int comidaActual = GameManager.Instance.GetCantidadComida();
                bool hayMuerto = (rufinoActual == null || bizcoActual == null);
                /*
                if (hayMuerto)
                {
                    textoInfo.text = $"[E] Revivir compañero\nCosto: {pagoPorResureccion} zanahorias\nTienes: {comidaActual}";

                    // Cambiar color según si puede o no revivir
                    if (comidaActual >= pagoPorResureccion)
                        textoInfo.color = Color.green;
                    else
                        textoInfo.color = Color.red;
                }
                else
                {
                    textoInfo.text = "Ambos jugadores están vivos";
                    textoInfo.color = Color.white;
                }*/
            }
        }
    }

    // Llamar este método cuando un jugador muera para actualizar la referencia

}