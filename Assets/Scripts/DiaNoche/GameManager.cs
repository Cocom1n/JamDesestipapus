using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    [SerializeField] private float duracionDia = 60f;
    [SerializeField] private float duracionNoche = 300f;
    [SerializeField] private GameObject panelDerrota;
    [SerializeField] private TextMeshProUGUI textoTiempo;
    [SerializeField] private TextMeshProUGUI textoEstado;
    [SerializeField] GameObject[] listaOvejas;

    [Header("colores de trancicion")]
    [SerializeField] private Light2D luzGlobal;
    [SerializeField] private Color colorDia = Color.white;
    [SerializeField] private Color colorT1 = new Color(0.2f, 0.2f, 0.4f);
    [SerializeField] private Color colorT2 = new Color(0.4f, 0.4f, 0.6f);
    [SerializeField] private Color colorT3 = new Color(0.7f, 0.7f, 0.8f);
    [SerializeField] private Color colorNoche = new Color(0.1f, 0.1f, 0.3f);

    private float tiempoRestante;
    private bool esNoche = false;
    private bool esDia = false;
    private bool juegoTerminado = false;
    private int ovejasVivas;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        IniciarDia();
    }

    // Update is called once per frame
    void Update()
    {
        if (juegoTerminado) return;

        tiempoRestante -= Time.deltaTime;
        textoTiempo.text =tiempoRestante.ToString("0");

        if(tiempoRestante<12f)
        {
            if (esNoche)
            {
                Debug.Log("trancicionando a dia");
                if (tiempoRestante <= 10f && tiempoRestante > 6f)
                {
                    Debug.Log("1 de 3 para ser de noche");
                    luzGlobal.color = colorT3;
                }
                    
                else if (tiempoRestante <= 6f && tiempoRestante > 3f)
                {
                    Debug.Log("2 de 3 para ser de noche");
                    luzGlobal.color = colorT2;
                }
                else if (tiempoRestante <= 3f && tiempoRestante > 0f)
                {
                    Debug.Log("3 de 3 para ser de noche");
                    luzGlobal.color = colorT1;
                }
            }
            if (esDia)
            {
                Debug.Log("trancicionando a noche");
                if (tiempoRestante <= 10f && tiempoRestante > 6f)
                {
                    Debug.Log("1 paso para la noche");
                    luzGlobal.color = colorT1;
                }
                else if (tiempoRestante <= 6f && tiempoRestante > 3f)
                {
                    Debug.Log("2 paso para la noche");
                    luzGlobal.color = colorT2;
                }
                else if (tiempoRestante <= 3f && tiempoRestante > 0f)
                {
                    Debug.Log("3 paso para la noche");
                    luzGlobal.color = colorT3;
                }
            }
        }
        


        if (tiempoRestante <= 0)
        {
            if (esNoche)
            {
                VerificarVictoria();
            }
            else
            {
                IniciarNoche();
            }
        }

    }

    void IniciarDia()
    {
        esNoche = false;
        esDia = true;
        tiempoRestante = duracionDia;
        ovejasVivas = listaOvejas.Length;
        textoEstado.text = "DÍA";
        luzGlobal.color = Color.white;
    }

    void IniciarNoche()
    {
        esNoche = true;
        esDia = false;
        tiempoRestante = duracionNoche;
        textoEstado.text = "NOCHE";
        luzGlobal.color = new Color(0.1f, 0.1f, 0.3f);
        //Aqui se deveran spawnear los enemigos cuando se hace de noche :D
    }

    void VerificarVictoria()
    {
        if (ovejasVivas > 0)
            IniciarDia();
        else
            Derrota();
    }

     public void OvejaMuerta()
    {
        ovejasVivas--;

        if (ovejasVivas <= 0)
        {
            Derrota();
        }
    }

    void Derrota()
    {
        juegoTerminado = true;
        Debug.Log("perdite");

        if (panelDerrota != null) panelDerrota.SetActive(true);
    }

}
