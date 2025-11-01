using System.Collections;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] private float rangoAtaque;
    [SerializeField] private Coroutine ataqueRutina;
    [SerializeField] private GameObject Player;
    [SerializeField] private Animator animator;
    [SerializeField] private float esperaAtaque;
    [SerializeField] private float ataqueTiempo;
    public bool atacando;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Player = GameObject.FindWithTag("Player");
        animator = GetComponent<Animator>();
        ataqueRutina = StartCoroutine(Ataque());
    }

    IEnumerator Ataque()
    {
        while (true)
        {
            float distance = Vector3.Distance(transform.position, Player.transform.position);

            if (distance <= rangoAtaque && !atacando)
            {
                atacando = true;
                animator.SetTrigger("atacando");
                yield return new WaitForSeconds(esperaAtaque);
                atacando = false;
            }

            yield return null;

        }
    }

    void EmpezarAtaque()
    {
        atacando = true;
        animator.SetTrigger("atacando");
        Invoke("terminarAtaque", ataqueTiempo);
    }

    void terminarAtaque()
    {
        atacando = false;
    }


}
