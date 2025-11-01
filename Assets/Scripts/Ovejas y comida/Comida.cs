using UnityEngine;

public class Comida : MonoBehaviour
{
    private void Start()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.RegistrarComida(this);
    }

    // Cuando se consume o destruye
    public void Consumir()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.EliminarComida(this);

        Destroy(gameObject);
    }
}
