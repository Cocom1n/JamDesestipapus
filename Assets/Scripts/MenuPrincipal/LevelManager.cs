using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{

    public void BotonIniciar()
    {
        SceneManager.LoadScene(0);
    }
    public void BotonSalir()
    {
        Application.Quit();
    }
    public void BotonvolVolverMenu()
    {
        SceneManager.LoadScene(1);
    }
}