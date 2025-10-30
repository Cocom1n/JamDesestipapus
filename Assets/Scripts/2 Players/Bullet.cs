using UnityEngine;

public class Bullet : MonoBehaviour
{

    void Start()
    {
        Destroy(gameObject, 2f);
    }

    //para cuando tenga q colicionar con algun enemigo :3
    /*
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.CompareTag("Enemigo"))
        {
            Destroy(gameObject);
        }
        
    }*/ 

}
