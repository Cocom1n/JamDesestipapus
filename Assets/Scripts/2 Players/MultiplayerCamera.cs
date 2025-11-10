using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))] // Asegura que este script esté en una cámara
public class MultiplayerCamera : MonoBehaviour
{
    [Header("Jugadores")]
    //[SerializeField] private Transform player1;
    //[SerializeField] private Transform player2;
    private List<Transform> jugadores = new List<Transform>();

    [Header("Tamaño limite de la camara")]
    [SerializeField] private float min = 6.5f;
    [SerializeField] private float max = 15f;
    [SerializeField] private float padding = 2f;


    private float suavizadoMovimiento = 0.3f;
    private float suavizadoZoom = 0.3f;
    private Vector3 velocidadMovimiento;
    private float velocidadZoom;
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();

        // Importante: Asegúrate de que la cámara sea Ortográfica
        if (!cam.orthographic)
        {
            Debug.LogError("La cámara NO es Ortográfica. Este script solo funciona con cámaras Ortográficas.");
        }
    }

    void LateUpdate()
    {
        ActualizarReferenciasDeJugadores();
        if (jugadores.Count == 0)
        {
            return;
        }

        if (jugadores.Count == 1)
        {
            Vector3 targetPosition = new Vector3(jugadores[0].position.x, jugadores[0].position.y, transform.position.z);
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocidadMovimiento, suavizadoMovimiento);

            cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, min, ref velocidadZoom, suavizadoZoom);
        }
        else
        {
            Move();
            Zoom();
        }

    }

    public void ActualizarReferenciasDeJugadores()
    {
        jugadores.Clear();
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject go in playerObjects)
        {
            jugadores.Add(go.transform);
        }
    }

    //se busca un puntomedio entre los dos jugadores y hace q la camara se mueva a ese punto 
    void Move()
    {
        Vector3 midpoint = (jugadores[0].position + jugadores[1].position) / 2f;
        Vector3 targetPosition = new Vector3(midpoint.x, midpoint.y, transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocidadMovimiento, suavizadoMovimiento);
    }

    //calcula el tamaño que nececita tener la camara en base a la distancia entre ambos jugadores
    //para q los dos puedan estar dentro de la camara
    void Zoom()
    {

        float distanciaY = Mathf.Abs(jugadores[0].position.y - jugadores[1].position.y) + padding;

        float distanciaX = Mathf.Abs(jugadores[0].position.x - jugadores[1].position.x) + padding;

        float tamanioNecesarioX = distanciaX / cam.aspect;

        float tamanioFinal = Mathf.Max(distanciaY * 0.5f, tamanioNecesarioX * 0.5f);
        tamanioFinal = Mathf.Clamp(tamanioFinal, min, max);

        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, tamanioFinal, ref velocidadZoom, suavizadoZoom);
    }
}