using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCombate : MonoBehaviour, IDetectEnemy
{
    [SerializeField] protected string myName;
    [SerializeField] protected float rangoAttack;
    [SerializeField] protected float timeAttack = 2f;

    public string Name => myName;

    float ultimoAttack = 0f;

    private List<Enemy> enemigosEnRango = new List<Enemy>();

    protected virtual void Update()
    {
        Enemy enemyCerca = BuscarEnemyCerca();
        if (enemyCerca != null)
        {
            DetectEnemy(enemyCerca);
        }
    }

    public virtual void DetectEnemy(Enemy enemigo)
    {
        transform.localScale = new Vector2(enemigo.transform.position.x > transform.position.x ? 1 : -1, transform.localScale.y);


        if (IsRangeEnemy(enemigo) && PuedeAtacar())
        {
            EjecuteAttack(enemigo);
            ultimoAttack = Time.time;
        }
    }

    protected bool IsRangeEnemy(Enemy enemigo)
    {
        float distance = Vector3.Distance(this.transform.position, enemigo.transform.position);
        return distance <= rangoAttack;
    }

    protected bool PuedeAtacar()
    {
        return Time.time - ultimoAttack >= timeAttack;
    }

    protected abstract void EjecuteAttack(Enemy enemigo);

    protected Enemy BuscarEnemyCerca()
    {
        Enemy cercano = null;
        float minDistance = Mathf.Infinity;

        foreach(var e in enemigosEnRango)
        {
            float distance = Vector3.Distance(this.transform.position, e.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                cercano = e;
            }
        }
        return cercano;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Enemy>(out Enemy enemigo))
        {
            if (!enemigosEnRango.Contains(enemigo))
                enemigosEnRango.Add(enemigo);
            Debug.Log(" entro un papu ");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<Enemy>(out Enemy enemigo))
        {
            enemigosEnRango.Remove(enemigo);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        var col = GetComponent<BoxCollider2D>();
        Gizmos.DrawWireCube(transform.position + (Vector3)col.offset, col.size);
    }
}
