using System.Collections;
using UnityEngine;

public class CaidaPicos : MonoBehaviour
{
    [SerializeField] GameObject[] picos;
    [SerializeField] float tiempoEntrePicos; 
    [SerializeField]float tiempoDeCaida;
    bool ordenAleatorio = true;

    public bool caidaIniciada = false;

    Vector3[] posicionesOriginales;

    void Start()
    {
        posicionesOriginales = new Vector3[picos.Length];
        for (int i = 0; i < picos.Length; i++)
        {
            if (picos[i] != null)
                posicionesOriginales[i] = picos[i].transform.position;
        }
    }

    public void IniciarEventoCaida()
    {
        if (caidaIniciada) return;

        caidaIniciada = true;
        StartCoroutine(CaerUnoPorUno());
    }

    IEnumerator CaerUnoPorUno()
    {
        int[] indices = new int[picos.Length];
        for (int i = 0; i < indices.Length; i++) indices[i] = i;
        if (ordenAleatorio)
        {
            for (int i = 0; i < indices.Length; i++)
            {
                int randomIndex = Random.Range(0, indices.Length);
                int temp = indices[i];
                indices[i] = indices[randomIndex];
                indices[randomIndex] = temp;
            }
        }
        for (int i = 0; i < indices.Length; i++)
        {
            int idx = indices[i];
            if (picos[idx] == null) continue;

            var rb = picos[idx].GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.bodyType = RigidbodyType2D.Dynamic;

            yield return new WaitForSeconds(tiempoEntrePicos);
        }

        yield return new WaitForSeconds(tiempoDeCaida);

        for (int i = 0; i < picos.Length; i++)
        {
            if (picos[i] == null) continue;

            var rb = picos[i].GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0;
                rb.bodyType = RigidbodyType2D.Kinematic;
            }

            picos[i].transform.position = posicionesOriginales[i];
        }

        caidaIniciada = false;
    }
}