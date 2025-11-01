using UnityEngine;
using UnityEngine.EventSystems;

public class Disparar : MonoBehaviour
{
    [Header("Config Bala")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawn;
    [SerializeField] private float bulletSpeed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            shoot();
            Debug.Log("disparopapu");
        }
            

    }

    void shoot() 
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        Vector2 direction = (mousePos - bulletSpawn.position).normalized;
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.Euler(0, 0, angle);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction * bulletSpeed;
       
    }
}
/*
private float getAngleMouse()
{
    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    Vector3 mouseDir = mousePos - transform.position;
    mouseDir.z = 0;
    float angle = (Vector3.SignedAngle(Vector3.right, mouseDir, Vector3.forward) + 360) % 360; ;
    return angle;

}*/

