using System.Collections;
using UnityEngine;

public class Llama : BaseCombate
{
    Animator animator;
    [SerializeField] private Transform spawner;

    protected void Awake()
    {
        animator = GetComponentInParent<Animator>();
        if (animator == null)
            Debug.LogError("Animator no encontrado");
    }

    protected override void EjecuteAttack(Enemy enemigo)
    {
        animator.SetTrigger("atacando");
        StartCoroutine(DaniarEnemigo(enemigo));
    }

    private IEnumerator DaniarEnemigo(Enemy enemigo)
    {
        yield return new WaitForSeconds(1f);
        Debug.Log(Name + " escupe al enemigo" + enemigo.Name);
        spawner.GetComponent<SpawnDisparoBaba>()?.Disparar();
    }
}
