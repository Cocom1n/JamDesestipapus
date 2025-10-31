using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            Vector2 shootDir = firePoint.right;
            bullet.GetComponent<Bala>().SetDireccion();
        }
    }
}
