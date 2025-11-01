using UnityEngine;
using TMPro;
//recibir daño para jugadores owo
public class RecibirDaño : MonoBehaviour, IDaniable, IMorir
{
    [SerializeField] public float maxVida;
    [SerializeField] private TextMeshProUGUI TextoUI;
    private float vidaActual;

   
    public void Start()
    {
        vidaActual = maxVida;
    }
    public void Update()
    {
        TextoUI.text = vidaActual.ToString("0");
    }
    public void RecibirDanio(float daño)
    {
        vidaActual -= daño;
        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    public void Morir()
    {
        TextoUI.text = "0";
        Destroy(gameObject);
    }
}
