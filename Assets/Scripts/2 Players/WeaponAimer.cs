using UnityEngine;

public class WeaponAimer : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Pivote desde el que se calcula la dirección (por ejemplo el spawn de la bala). Si es null, se usa el parent.")]
    [SerializeField] private Transform pivot;

    [Tooltip("Sprite renderer de la visual. Si no se asigna, se busca en children.")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Movimiento")]
    [Tooltip("La visual se desplaza hacia el ratón esta distancia desde el pivot (0 = solo rotación).")]
    [SerializeField] private float followDistance = 0.25f;

    [Tooltip("Si es true la visual seguirá ligeramente al ratón.")]
    [SerializeField] private bool followMouse = true;

    [Header("Flip")]
    [Tooltip("Si se detecta el ratón a la izquierda, se invierte el sprite. Cambia a flipX si tu sprite queda al revés.")]
    [SerializeField] private bool flipOnLeft = true;

    void Reset()
    {
        // conveniencia al añadir el componente
        if (pivot == null && transform.parent != null) pivot = transform.parent;
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Start()
    {
        if (pivot == null && transform.parent != null) pivot = transform.parent;
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (!gameObject.activeInHierarchy) return; // solo cuando está visible/activa

        Camera cam = Camera.main;
        if (cam == null || pivot == null) return;

        Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        Vector2 dir = (mouseWorld - pivot.position);
        if (dir.sqrMagnitude < 0.0001f) return;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        if (followMouse)
        {
            transform.position = pivot.position + (Vector3)dir.normalized * followDistance;
        }
        else
        {
            // mantener en pivot si no se sigue al mouse
            transform.position = pivot.position;
        }

        if (spriteRenderer != null && flipOnLeft)
        {
            // Cuando el ángulo apunta hacia la izquierda (>90 o < -90) giramos el sprite
            bool pointingLeft = angle > 90f || angle < -90f;
            // Usa flipY por defecto; si el sprite queda boca abajo, cambia a flipX
            spriteRenderer.flipY = pointingLeft;
        }
    }
}