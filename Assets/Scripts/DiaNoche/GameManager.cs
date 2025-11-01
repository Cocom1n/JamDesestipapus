using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Ciclo día/noche")]
    [SerializeField] private float duracionDia = 60f;
    [SerializeField] private float duracionNoche = 300f;
    [SerializeField] private Light2D luzGlobal;
    [SerializeField] private Color colorDia = Color.white;
    [SerializeField] private Color colorT1 = new Color(0.2f, 0.2f, 0.4f);
    [SerializeField] private Color colorT2 = new Color(0.4f, 0.4f, 0.6f);
    [SerializeField] private Color colorT3 = new Color(0.7f, 0.7f, 0.8f);
    [SerializeField] private Color colorNoche = new Color(0.1f, 0.1f, 0.3f);

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI textoTiempo;
    [SerializeField] private GameObject imagenSol;
    [SerializeField] private GameObject imagenLuna;
    [SerializeField] private TextMeshProUGUI textoMultiplicadorOvejas;
    [SerializeField] private GameObject panelDerrota;

    [Header("Prefabs")]
    [SerializeField] private GameObject prefabOveja;
    [SerializeField] private GameObject prefabComida;

    [Header("SpawnersEnemigos")]
    [SerializeField] private Spawner[] spawners;

    private List<Oveja> ovejasVivas = new List<Oveja>();
    private List<Comida> comidasVivas = new List<Comida>();

    private float tiempoRestante;
    private bool esNoche = false;
    private bool esDia = false;
    private bool juegoTerminado = false;

    // ------------------------------

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        IniciarDia();
    }

    private void Update()
    {
        if (juegoTerminado) return;

        tiempoRestante -= Time.deltaTime;
        textoTiempo.text = tiempoRestante.ToString("0");

        ActualizarContadorOvejas();

        TransicionesDeColor();

        if (tiempoRestante <= 0)
        {
            if (esNoche)
                VerificarFinDeNoche();
            else
                IniciarNoche();
        }
    }

    // ------------------------------
    // ACTUALIZAR UI DE OVEJAS
    // ------------------------------

    private void ActualizarContadorOvejas()
    {
        if (textoMultiplicadorOvejas != null)
        {
            int cantidad = ovejasVivas.Count;
            textoMultiplicadorOvejas.text = $"x{cantidad}";

            if (cantidad <= 1)
                textoMultiplicadorOvejas.color = Color.red;
            else if (cantidad <= 3)
                textoMultiplicadorOvejas.color = Color.yellow;
            else
                textoMultiplicadorOvejas.color = Color.white;
        }
    }

    // ------------------------------
    // CICLO DÍA/NOCHE
    // ------------------------------

    public List<Oveja> GetOvejasVivas()
    {
        return ovejasVivas;
    }

    void IniciarDia()
    {
        esDia = true;
        esNoche = false;
        tiempoRestante = duracionDia;
        luzGlobal.color = colorDia;
        imagenSol.SetActive(true);
        imagenLuna.SetActive(false);

        foreach(Spawner spawner in spawners)
        {
            if(spawner != null)
            {
                spawner.DetenerSpawn();
                spawner.DestruAliensRestantes();
            }
        }
            

        DuplicarEntidades();
        ActualizarContadorOvejas();
    }

    void IniciarNoche()
    {
        esDia = false;
        esNoche = true;
        tiempoRestante = duracionNoche;
        luzGlobal.color = colorNoche;
        imagenSol.SetActive(false);
        imagenLuna.SetActive(true);

        foreach (Spawner spawner in spawners)
        {
            if (spawner != null)
            {
                spawner.IniciarSpawn();
            }
        }
    }

    void VerificarFinDeNoche()
    {
        if (ovejasVivas.Count > 0)
            IniciarDia();
        else
            Derrota();
    }

    // ------------------------------
    // REGISTRO DE ENTIDADES
    // ------------------------------

    public void RegistrarOveja(Oveja oveja)
    {
        if (!ovejasVivas.Contains(oveja))
        {
            ovejasVivas.Add(oveja);
            ActualizarContadorOvejas(); 
        }
    }

    public void EliminarOveja(Oveja oveja)
    {
        if (ovejasVivas.Contains(oveja))
        {
            ovejasVivas.Remove(oveja);
            ActualizarContadorOvejas(); 
        }

        if (ovejasVivas.Count <= 0)
            Derrota();
    }

    public void RegistrarComida(Comida comida)
    {
        if (!comidasVivas.Contains(comida))
            comidasVivas.Add(comida);
    }

    public void EliminarComida(Comida comida)
    {
        if (comidasVivas.Contains(comida))
            comidasVivas.Remove(comida);
    }

    // ------------------------------
    // DUPLICACIÓN
    // ------------------------------

    private void DuplicarEntidades()
    {
        List<Oveja> nuevasOvejas = new List<Oveja>();
        foreach (var oveja in ovejasVivas.ToArray())
        {
            if (oveja != null)
            {
                var nueva = Instantiate(prefabOveja, oveja.transform.position + Vector3.up * 1.5f, Quaternion.identity);
                nuevasOvejas.Add(nueva.GetComponent<Oveja>());
            }
        }
        ovejasVivas.AddRange(nuevasOvejas);

        List<Comida> nuevasComidas = new List<Comida>();
        foreach (var comida in comidasVivas.ToArray())
        {
            if (comida != null)
            {
                var nueva = Instantiate(prefabComida, comida.transform.position + Vector3.up * 1f, Quaternion.identity);
                nuevasComidas.Add(nueva.GetComponent<Comida>());
            }
        }
        comidasVivas.AddRange(nuevasComidas);
    }

    // ------------------------------
    // DERROTA
    // ------------------------------

    private void Derrota()
    {
        if (juegoTerminado) return;

        juegoTerminado = true;
        Debug.Log("Perdiste. Todas las ovejas murieron.");

        if (panelDerrota != null)
            panelDerrota.SetActive(true);
    }

    // ------------------------------
    // COLOR TRANSITION
    // ------------------------------

    private void TransicionesDeColor()
    {
        if (tiempoRestante < 12f)
        {
            if (esNoche)
            {
                if (tiempoRestante <= 10f && tiempoRestante > 6f)
                    luzGlobal.color = colorT3;
                else if (tiempoRestante <= 6f && tiempoRestante > 3f)
                    luzGlobal.color = colorT2;
                else if (tiempoRestante <= 3f)
                    luzGlobal.color = colorT1;
            }
            else if (esDia)
            {
                if (tiempoRestante <= 10f && tiempoRestante > 6f)
                    luzGlobal.color = colorT1;
                else if (tiempoRestante <= 6f && tiempoRestante > 3f)
                    luzGlobal.color = colorT2;
                else if (tiempoRestante <= 3f)
                    luzGlobal.color = colorT3;
            }
        }
    }
}