using UnityEngine;
using UnityEngine.UI;

public class Parpadeo : MonoBehaviour
{
    [Header("Configuración de Colores")]
    [SerializeField] private Color color1 = Color.white;
    [SerializeField] private Color color2 = Color.red;

    [Header("Configuración de Velocidad")]
    [SerializeField] private float velocidadMinima = 0.5f; // Velocidad mínima
    [SerializeField] private float velocidadMaxima = 2f; // Velocidad máxima

    [SerializeField] private Image imagen;
    private float tiempo;
    private float velocidadActual;

    void Start()
    {
        if (imagen == null)
        {
            Debug.LogError("No se encontró el componente Image en " + gameObject.name);
            enabled = false;
        }

        // Establecer velocidad inicial aleatoria
        velocidadActual = Random.Range(velocidadMinima, velocidadMaxima);
    }

    void Update()
    {
        if (imagen == null) return;

        tiempo += Time.deltaTime * velocidadActual;

        // Cambio instantáneo
        if (Mathf.PingPong(tiempo, 1f) < 0.5f)
        {
            imagen.color = color1;
            // Cambiar velocidad en la mitad del ciclo
            if (tiempo % 2f < Time.deltaTime * velocidadActual)
            {
                velocidadActual = Random.Range(velocidadMinima, velocidadMaxima);
            }
        }
        else
        {
            imagen.color = color2;
        }
    }

    // Métodos opcionales para controlar desde otros scripts
    public void CambiarColor1(Color nuevoColor)
    {
        color1 = nuevoColor;
    }

    public void CambiarColor2(Color nuevoColor)
    {
        color2 = nuevoColor;
    }

    public void CambiarVelocidad(float nuevaVelocidadMin, float nuevaVelocidadMax)
    {
        velocidadMinima = nuevaVelocidadMin;
        velocidadMaxima = nuevaVelocidadMax;
        velocidadActual = Random.Range(velocidadMinima, velocidadMaxima);
    }

    public void DetenerTitilacion()
    {
        enabled = false;
    }

    public void ReanudarTitilacion()
    {
        enabled = true;
    }
}
