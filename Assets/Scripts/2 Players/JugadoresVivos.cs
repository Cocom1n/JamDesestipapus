using UnityEngine;

public class JugadoresVivos : MonoBehaviour
{
    [SerializeField] private GameObject panelDerrota;

    void Update()
    {
        GameObject[] jugadoresVivos = GameObject.FindGameObjectsWithTag("Player");

        if (jugadoresVivos.Length == 0)
        {
            if (panelDerrota != null)
                panelDerrota.SetActive(true);
        }
    }
}
