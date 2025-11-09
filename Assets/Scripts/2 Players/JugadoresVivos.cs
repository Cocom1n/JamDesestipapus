using UnityEngine;

public class JugadoresVivos : MonoBehaviour
{
    [SerializeField] private GameObject rufino;
    [SerializeField] private GameObject bisco;
    [SerializeField] private GameObject panelDerrota;

    void Update()
    {
        if (rufino == null && bisco == null) {
            if (panelDerrota != null)
                panelDerrota.SetActive(true);
        }
    }
}
