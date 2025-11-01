using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject alien;
    [SerializeField] float alienIntervalo;
    [SerializeField] int alienMax;
    [SerializeField] int cuantosAliens;
    [SerializeField] Coroutine spawnRutina;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnRutina = StartCoroutine(spawnAlien(alienIntervalo, alien));
    }
    private IEnumerator spawnAlien(float interval, GameObject Alien)
    {
            while (cuantosAliens < alienMax)
            {
                yield return new WaitForSeconds(interval);
                Instantiate(Alien);
                cuantosAliens++;
            }

        }
    }
