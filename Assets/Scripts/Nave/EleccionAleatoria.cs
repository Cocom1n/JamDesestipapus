using DG.Tweening;
using System.Collections;
using UnityEngine;

public class EleccionAleatoria : MonoBehaviour
{
    [SerializeField] GameObject[] listaObjetos;
    [SerializeField] float tiempoEspera = 5f;
    [SerializeField] float duracionMovimiento = 1.5f;
    [SerializeField] float duracionElevacion = 1f;
    [SerializeField] float alturaSalida = 3f;
    [SerializeField] float distanciaFueraCamara = 6f;

    private GameObject objetivoActual;
    private Camera cam;
    private RecogerObjeto succionador;
    private float alturaY;
    private void Start()
    {
        cam = Camera.main;
        succionador = GetComponent<RecogerObjeto>(); 
        alturaY = transform.position.y;
        StartCoroutine(RutinaNave());
    }

    IEnumerator RutinaNave()
    {
        while (true)
        {
            if (listaObjetos.Length == 0)
                yield break;
            succionador.enabled = false;


            Vector3 posInicio = ObtenerPosicionFueraCamara();
            posInicio.y = alturaY;
            transform.position = posInicio;


            int indice = Random.Range(0, listaObjetos.Length);
            objetivoActual = listaObjetos[indice];

            if (objetivoActual == null)
            {
                yield return new WaitForSeconds(tiempoEspera);
                continue;
            }


            Vector3 destino = new Vector3(objetivoActual.transform.position.x, alturaY, transform.position.z);
            yield return transform.DOMove(destino, duracionMovimiento).SetEase(Ease.InOutSine).WaitForCompletion();

            Debug.Log($"Nave posicionada sobre {objetivoActual.name}");

            succionador.enabled = true;

            yield return new WaitUntil(() => objetivoActual == null);

            Debug.Log("Objeto destruido");

            succionador.enabled = false;

            Vector3 posSalida = ObtenerPosicionFueraCamara();
            posSalida.y += alturaSalida + alturaY;

            yield return transform.DOMove(posSalida, duracionElevacion).SetEase(Ease.InOutSine).WaitForCompletion();
            yield return new WaitForSeconds(tiempoEspera);
        }
    }

    Vector3 ObtenerPosicionFueraCamara()
    {
        float lado = Random.value < 0.5f ? -1f : 1f;
        float anchoCam = cam.orthographicSize * cam.aspect;
        float posX = cam.transform.position.x + lado * (anchoCam + distanciaFueraCamara);
        float posY = alturaY;
        float posZ = transform.position.z;
        return new Vector3(posX, posY, posZ);
    }
}