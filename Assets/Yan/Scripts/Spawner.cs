using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject alien;
    [SerializeField] float alienIntervalo;
    [SerializeField] int alienMax;
    [SerializeField] int cuantosAliens;
    private Coroutine spawnRutina;
    private bool estaSpawneando = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    
    public void IniciarSpawn()
    {
        if (!estaSpawneando)
        {
            cuantosAliens = 0;
            estaSpawneando=true;
            spawnRutina = StartCoroutine(spawnAlien(alienIntervalo, alien));
        }
    }

    public void DetenerSpawn()
    {
        if (estaSpawneando && spawnRutina != null)
        {
            StopCoroutine(spawnAlien(alienIntervalo, alien));
            estaSpawneando = false;
        }
    }

    private IEnumerator spawnAlien(float interval, GameObject Alien)
    {
        while (cuantosAliens < alienMax)
        {
            yield return new WaitForSeconds(interval);
            Instantiate(Alien, transform.position, transform.rotation);
            cuantosAliens++;
        }
    }

    public void DestruAliensRestantes()
    {
        GameObject[] aliens = GameObject.FindGameObjectsWithTag("Alien");
        foreach (GameObject alien in aliens)
        {
            Destroy(alien);
        }
    }
}
