using UnityEngine;

public class DisparoEscopeta : MonoBehaviour
{
    [Header("Configuración de Bala")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawn;
    [SerializeField] private float bulletSpeed = 20f;
    float hola;

    [Header("Cooldown (Opcional)")]
    [SerializeField] private float cooldownDisparo = 0.3f;
    private float tiempoUltimoDisparo;

    [Header("Audio - Escopeta")]
    [SerializeField] private AudioClip clipDisparo;
    [SerializeField] private AudioSource sfxSource; // opcional: arrastra un AudioSource o se creará uno automáticamente
    [SerializeField][Range(0f, 1f)] private float sfxVolume = 1f;
    void Start()
    {
        // asegurar AudioSource
        if (sfxSource == null)
        {
            sfxSource = gameObject.GetComponent<AudioSource>();
            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false;
                sfxSource.loop = false;
            }
        }
    }

    public void Disparar()
    {
        // Verificar cooldown
        if (Time.time - tiempoUltimoDisparo < cooldownDisparo)
            return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        Vector2 direction = (mousePos - bulletSpawn.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.Euler(0, 0, angle);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * bulletSpeed;
        }

        tiempoUltimoDisparo = Time.time;
        Debug.Log(" Disparo de escopeta");

        // Reproducir sonido de disparo (PlayOneShot permite solapar SFX)
        if (clipDisparo != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clipDisparo, sfxVolume);
        }
    }

    public bool PuedeDisparar()
    {
        return Time.time - tiempoUltimoDisparo >= cooldownDisparo;
    }
}