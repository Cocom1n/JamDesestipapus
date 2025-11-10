using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicaDia;
    [SerializeField] private AudioSource musicaNoche;

    [Header("Configuración de Volumen")]
    [SerializeField] private float volumenMaximo = 0.5f;
    [SerializeField] private float duracionFade = 3f; // Duración del fade en segundos

    [Header("Modo de Transición")]
    [SerializeField] private bool usarFadeSuave = true; // true = suave, false = brusco

    private bool esNocheActual = false;
    private Coroutine fadeCoroutine;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Opcional: mantener entre escenas
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        ConfigurarAudioSources();
        IniciarMusicaDia();
    }

    void ConfigurarAudioSources()
    {
        // Configurar música de día
        if (musicaDia != null)
        {
            musicaDia.loop = true;
            musicaDia.volume = 0f;
        }

        // Configurar música de noche
        if (musicaNoche != null)
        {
            musicaNoche.loop = true;
            musicaNoche.volume = 0f;
        }
    }

    // ============================
    // MÉTODOS PÚBLICOS (llamados desde GameManager)
    // ============================

    public void IniciarMusicaDia()
    {
        if (esNocheActual || musicaDia == null) return;

        if (usarFadeSuave)
        {
            CambiarMusicaConFade(musicaDia, musicaNoche);
        }
        else
        {
            CambiarMusicaBrusco(musicaDia, musicaNoche);
        }

        esNocheActual = false;
    }

    public void IniciarMusicaNoche()
    {
        if (!esNocheActual && musicaNoche == null) return;

        if (usarFadeSuave)
        {
            CambiarMusicaConFade(musicaNoche, musicaDia);
        }
        else
        {
            CambiarMusicaBrusco(musicaNoche, musicaDia);
        }

        esNocheActual = true;
    }

    // ============================
    // TRANSICIÓN SUAVE (CROSSFADE)
    // ============================

    void CambiarMusicaConFade(AudioSource musicaNueva, AudioSource musicaVieja)
    {
        // Detener fade anterior si existe
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(CrossfadeMusica(musicaNueva, musicaVieja));
    }

    IEnumerator CrossfadeMusica(AudioSource musicaNueva, AudioSource musicaVieja)
    {
        // Iniciar la nueva música si no está sonando
        if (!musicaNueva.isPlaying)
        {
            musicaNueva.volume = 0f;
            musicaNueva.Play();
        }

        float tiempoTranscurrido = 0f;
        float volumenInicialVieja = musicaVieja != null ? musicaVieja.volume : 0f;
        float volumenInicialNueva = musicaNueva.volume;

        // Fade gradual
        while (tiempoTranscurrido < duracionFade)
        {
            tiempoTranscurrido += Time.deltaTime;
            float t = tiempoTranscurrido / duracionFade;

            // Fade out música vieja
            if (musicaVieja != null && musicaVieja.isPlaying)
            {
                musicaVieja.volume = Mathf.Lerp(volumenInicialVieja, 0f, t);
            }

            // Fade in música nueva
            musicaNueva.volume = Mathf.Lerp(volumenInicialNueva, volumenMaximo, t);

            yield return null;
        }

        // Asegurar volúmenes finales
        musicaNueva.volume = volumenMaximo;

        if (musicaVieja != null)
        {
            musicaVieja.volume = 0f;
            musicaVieja.Stop();
        }

        fadeCoroutine = null;
    }

    // ============================
    // TRANSICIÓN BRUSCA
    // ============================

    void CambiarMusicaBrusco(AudioSource musicaNueva, AudioSource musicaVieja)
    {
        // Detener música vieja
        if (musicaVieja != null && musicaVieja.isPlaying)
        {
            musicaVieja.Stop();
            musicaVieja.volume = 0f;
        }

        // Iniciar música nueva
        if (musicaNueva != null)
        {
            musicaNueva.volume = volumenMaximo;
            musicaNueva.Play();
        }
    }

    // ============================
    // MÉTODOS ADICIONALES
    // ============================

    public void DetenerTodasLasMusicas()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        if (musicaDia != null)
        {
            musicaDia.Stop();
            musicaDia.volume = 0f;
        }

        if (musicaNoche != null)
        {
            musicaNoche.Stop();
            musicaNoche.volume = 0f;
        }
    }

    public void SetVolumenMaximo(float nuevoVolumen)
    {
        volumenMaximo = Mathf.Clamp01(nuevoVolumen);

        // Ajustar volumen actual si hay música sonando
        if (esNocheActual && musicaNoche != null && musicaNoche.isPlaying)
        {
            musicaNoche.volume = volumenMaximo;
        }
        else if (!esNocheActual && musicaDia != null && musicaDia.isPlaying)
        {
            musicaDia.volume = volumenMaximo;
        }
    }

    public void SetDuracionFade(float nuevaDuracion)
    {
        duracionFade = Mathf.Max(0.1f, nuevaDuracion);
    }

    public void SetUsarFadeSuave(bool usarFade)
    {
        usarFadeSuave = usarFade;
    }
}