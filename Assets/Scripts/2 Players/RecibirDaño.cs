using System.Collections;
using UnityEngine;
using TMPro;

// recibir daño para jugadores
public class RecibirDaño : MonoBehaviour, IDaniable, IMorir
{
    [SerializeField] public float maxVida;
    private TextMeshProUGUI TextoUI;
    private float vidaActual;

    [SerializeField] private AudioClip clipMuerte;  

    private AudioSource sfxSource;
    private bool estaMuerto = false;

    public void Start()
    {
        vidaActual = maxVida;
        estaMuerto = false;

        if(TextoUI == null)
        {
            if(GetComponent<SoyRufino>() != null){
                HudRufino hud = FindFirstObjectByType<HudRufino>();
                if (hud != null)
                {
                    TextoUI = hud.GetComponent<TextMeshProUGUI>();
                }
            }
            if (GetComponent<SoyBizco>() != null)
            {
                HudBizco hud = FindFirstObjectByType<HudBizco>();
                if (hud != null)
                {
                    TextoUI = hud.GetComponent<TextMeshProUGUI>();
                }
            }
        }
        
        

        sfxSource = GetComponent<AudioSource>();
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
            sfxSource.loop = false;
        }

        if (TextoUI != null)
            TextoUI.text = vidaActual.ToString("0");
    }

    public void Update()
    {
        if (TextoUI != null)
            TextoUI.text = vidaActual.ToString("0");
    }

    public void RecibirDanio(float daño)
    {
        if (estaMuerto) return;

        vidaActual -= daño;

        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    public void Morir()
    {
        if (estaMuerto) return;
        estaMuerto = true;

        if (TextoUI != null)
        {
            TextoUI.text = "0";
        }

        if (clipMuerte != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clipMuerte);
            StartCoroutine(DelayedDestroy(clipMuerte.length));
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator DelayedDestroy(float delay)
    {
        yield return new WaitForSeconds(Mathf.Max(0.01f, delay));
        Destroy(gameObject);
    }
}
