using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject alien;
    [SerializeField] float alienIntervalo;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(spawnAlien(alienIntervalo, alien));
    }
    private IEnumerator spawnAlien(float interval, GameObject Alien)
    {
        yield return new WaitForSeconds(interval);
        GameObject newAlien = Instantiate(Alien);
        StartCoroutine(spawnAlien(interval, Alien));
    }
}
