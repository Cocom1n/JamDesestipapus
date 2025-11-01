using DG.Tweening;
using System.Collections;
using UnityEngine;

public class EleccionAleatoria : MonoBehaviour
{
    public enum EstadoOvni
    {
        Idle,
        MoveToTarget,
        Attacking,
        Retreating,
        Dead
    }

    [Header("Objetivos y movimiento")]
    [SerializeField] private GameObject[] listaObjetos;
    [SerializeField] private float tiempoEspera = 5f;
    [SerializeField] private float duracionMovimiento = 1.5f;
    [SerializeField] private float duracionElevacion = 1f;
    [SerializeField] private float alturaSalida = 3f;
    [SerializeField] private float distanciaFueraCamara = 6f;

    private Camera cam;
    private RecogerObjeto succionador;
    private GameObject objetivoActual;
    private Animator animator;

    private EstadoOvni estadoActual = EstadoOvni.Idle;
    private float alturaY;
    private bool enProceso = false;

    private void Start()
    {
        cam = Camera.main;
        succionador = GetComponent<RecogerObjeto>();
        animator = GetComponentInChildren<Animator>();
        alturaY = transform.position.y;

        CambiarEstado(EstadoOvni.Idle);
    }

    private void Update()
    {
        if (!enProceso)
        {
            switch (estadoActual)
            {
                case EstadoOvni.Idle:
                    StartCoroutine(EstadoIdle());
                    break;
                case EstadoOvni.MoveToTarget:
                    StartCoroutine(EstadoMover());
                    break;
                case EstadoOvni.Attacking:
                    StartCoroutine(EstadoAtacar());
                    break;
                case EstadoOvni.Retreating:
                    StartCoroutine(EstadoRetirada());
                    break;
                case EstadoOvni.Dead:
                    // nada
                    break;
            }
        }
    }

    // -------------------
    // ESTADOS
    // -------------------

    private IEnumerator EstadoIdle()
    {
        enProceso = true;
        succionador.enabled = false;
        animator.SetBool("Levantando", false);

        yield return new WaitForSeconds(tiempoEspera);

        if (listaObjetos.Length > 0)
        {
            objetivoActual = listaObjetos[Random.Range(0, listaObjetos.Length)];
            CambiarEstado(EstadoOvni.MoveToTarget);
        }
        else
        {
            CambiarEstado(EstadoOvni.Idle);
        }

        enProceso = false;
    }

    private IEnumerator EstadoMover()
    {
        enProceso = true;

        if (objetivoActual == null)
        {
            CambiarEstado(EstadoOvni.Idle);
            enProceso = false;
            yield break;
        }

        Vector3 posInicio = ObtenerPosicionFueraCamara();
        posInicio.y = alturaY;
        transform.position = posInicio;

        Vector3 destino = new Vector3(objetivoActual.transform.position.x, alturaY, transform.position.z);

        yield return transform.DOMove(destino, duracionMovimiento)
            .SetEase(Ease.InOutSine)
            .WaitForCompletion();

        CambiarEstado(EstadoOvni.Attacking);
        enProceso = false;
    }

    private IEnumerator EstadoAtacar()
    {
        enProceso = true;

        animator.SetBool("Levantando", true);
        succionador.enabled = true;

        yield return new WaitUntil(() => objetivoActual == null); // espera a que el objeto se destruya

        succionador.enabled = false;
        animator.SetBool("Levantando", false);

        CambiarEstado(EstadoOvni.Retreating);
        enProceso = false;
    }

    private IEnumerator EstadoRetirada()
    {
        enProceso = true;

        Vector3 posSalida = ObtenerPosicionFueraCamara();
        posSalida.y += alturaSalida + alturaY;

        yield return transform.DOMove(posSalida, duracionElevacion)
            .SetEase(Ease.InOutSine)
            .WaitForCompletion();

        CambiarEstado(EstadoOvni.Idle);
        enProceso = false;
    }

    // -------------------
    // MÉTODOS AUXILIARES
    // -------------------

    private Vector3 ObtenerPosicionFueraCamara()
    {
        float lado = Random.value < 0.5f ? -1f : 1f;
        float anchoCam = cam.orthographicSize * cam.aspect;
        float posX = cam.transform.position.x + lado * (anchoCam + distanciaFueraCamara);
        float posY = alturaY;
        float posZ = transform.position.z;
        return new Vector3(posX, posY, posZ);
    }

    private void CambiarEstado(EstadoOvni nuevo)
    {
        estadoActual = nuevo;
    }

    public void Morir()
    {
        if (estadoActual == EstadoOvni.Dead) return;
        StopAllCoroutines();
        succionador.enabled = false;
        animator.SetTrigger("Morir");
        estadoActual = EstadoOvni.Dead;
        transform.DOKill();
        Destroy(gameObject, 2f);
    }
}
