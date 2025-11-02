using UnityEngine;

public class DisparoLazo : MonoBehaviour
{
    [Header("Configuración de Lazo")]
    [SerializeField] private GameObject lazoPrefab;
    [SerializeField] private Transform lazoSpawn;
    [SerializeField] private float lazoSpeed = 15f;
    [SerializeField] private float lazoRange = 8f;

    [Header("Cooldown")]
    [SerializeField] private float cooldownLazo = 1f;
    private float tiempoUltimoLanzamiento;

    [Header("Límite de Lazos Activos")]
    [SerializeField] private int maxLazosActivos = 1;
    private int lazosActivos = 0;

    public void LanzarLazo()
    {
        // Verificar cooldown
        if (Time.time - tiempoUltimoLanzamiento < cooldownLazo)
        {
            Debug.Log("⏳ Lazo en cooldown");
            return;
        }

        // Verificar límite de lazos activos
        if (lazosActivos >= maxLazosActivos)
        {
            Debug.Log("⚠️ Ya hay un lazo activo");
            return;
        }

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        Vector2 direction = (mousePos - lazoSpawn.position).normalized;

        GameObject lazo = Instantiate(lazoPrefab, lazoSpawn.position, Quaternion.identity);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        lazo.transform.rotation = Quaternion.Euler(0, 0, angle);

        // Configurar el lazo
        LazoProjectile lazoScript = lazo.GetComponent<LazoProjectile>();
        if (lazoScript != null)
        {
            lazoScript.Inicializar(direction, lazoSpeed, lazoRange, transform, this);
            lazosActivos++;
        }

        tiempoUltimoLanzamiento = Time.time;
        Debug.Log("🪢 Lanzaste el lazo");
    }

    // Llamado por LazoProjectile cuando se destruye
    public void OnLazoDestruido()
    {
        lazosActivos--;
        if (lazosActivos < 0) lazosActivos = 0;
    }

    public bool PuedeLanzar()
    {
        return Time.time - tiempoUltimoLanzamiento >= cooldownLazo && lazosActivos < maxLazosActivos;
    }

    public int GetLazosActivos() => lazosActivos;
}