using System.Collections;
using UnityEngine;
using TMPro;

// recibir daño para jugadores
public class RecibirDaño : MonoBehaviour, IDaniable, IMorir
{
    [SerializeField] public float maxVida;
    [SerializeField] private TextMeshProUGUI TextoUI;
    private float vidaActual;

    [Header("Audio - Muerte Rufino")]
    [SerializeField] private AudioClip clipMuerte;  

    private AudioSource sfxSource;
    private bool estaMuerto = false;

    public void Start()
    {
        vidaActual = maxVida;

        // Asegurar AudioSource (si hay uno en el GameObject lo reutilizamos, si no lo añadimos)
        sfxSource = GetComponent<AudioSource>();
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
            sfxSource.loop = false;
        }
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

        // (Opcional) reproducir un SFX de "hit" aquí si lo agregas
        // if (clipHit != null) sfxSource.PlayOneShot(clipHit);

        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    public void Morir()
    {
        if (estaMuerto) return;
        estaMuerto = true;

        TextoUI.text = "0";

        if (clipMuerte != null && sfxSource != null)
        {
            // Reproducir clip de muerte y esperar a que termine antes de destruir
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
        // Protección mínima por si el clip tiene duración 0
        yield return new WaitForSeconds(Mathf.Max(0.01f, delay));
        Destroy(gameObject);
    }
}
